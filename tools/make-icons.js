/* Boi Impact — gera os ícones do PWA (192 e 512) a partir de uma imagem PNG.
 * Uso:  node tools/make-icons.js  caminho/da/imagem.png
 * Sem dependências externas (usa só Node + zlib).
 * Suporta PNG 8-bit, color type 2 (RGB) ou 6 (RGBA), não-entrelaçado.
 * A imagem é recortada no centro (quadrado) e redimensionada (bilinear).
 */
const zlib = require('zlib');
const fs = require('fs');
const path = require('path');

/* ---------- PNG encode ---------- */
function crc32(buf){let c=~0>>>0;for(let i=0;i<buf.length;i++){c^=buf[i];for(let k=0;k<8;k++)c=(c>>>1)^(0xEDB88320&(-(c&1)));}return (~c)>>>0;}
function chunk(type,data){const t=Buffer.from(type,'ascii');const len=Buffer.alloc(4);len.writeUInt32BE(data.length,0);const crc=Buffer.alloc(4);crc.writeUInt32BE(crc32(Buffer.concat([t,data])),0);return Buffer.concat([len,t,data,crc]);}
function encodePNG(w,h,rgba){
  const sig=Buffer.from([137,80,78,71,13,10,26,10]);
  const ihdr=Buffer.alloc(13);ihdr.writeUInt32BE(w,0);ihdr.writeUInt32BE(h,4);ihdr[8]=8;ihdr[9]=6;
  const stride=w*4+1;const raw=Buffer.alloc(stride*h);
  for(let y=0;y<h;y++){raw[y*stride]=0;rgba.copy(raw,y*stride+1,y*w*4,(y+1)*w*4);}
  const idat=zlib.deflateSync(raw,{level:9});
  return Buffer.concat([sig,chunk('IHDR',ihdr),chunk('IDAT',idat),chunk('IEND',Buffer.alloc(0))]);
}

/* ---------- PNG decode ---------- */
function decodePNG(buf){
  const sig=[137,80,78,71,13,10,26,10];
  for(let i=0;i<8;i++)if(buf[i]!==sig[i])throw new Error('Não é um PNG válido.');
  let p=8,w=0,h=0,bitDepth=0,colorType=0,interlace=0;const idat=[];
  while(p<buf.length){
    const len=buf.readUInt32BE(p);const type=buf.toString('ascii',p+4,p+8);const data=buf.slice(p+8,p+8+len);p+=12+len;
    if(type==='IHDR'){w=data.readUInt32BE(0);h=data.readUInt32BE(4);bitDepth=data[8];colorType=data[9];interlace=data[12];}
    else if(type==='IDAT')idat.push(data);
    else if(type==='IEND')break;
  }
  if(bitDepth!==8)throw new Error('Só suporto PNG de 8 bits por canal. Reexporte a imagem como PNG 8-bit.');
  if(interlace!==0)throw new Error('PNG entrelaçado não é suportado. Reexporte sem entrelaçamento (Adam7 off).');
  if(colorType!==2&&colorType!==6)throw new Error('Use PNG RGB ou RGBA.');
  const channels=colorType===6?4:3;const bpp=channels;const stride=w*bpp;
  const raw=zlib.inflateSync(Buffer.concat(idat));
  const out=Buffer.alloc(stride*h);
  const pa=(i)=>i<0?0:out[i];
  for(let y=0;y<h;y++){
    const ft=raw[y*(stride+1)];const rowOff=y*(stride+1)+1;const o=y*stride;
    for(let x=0;x<stride;x++){
      const rawV=raw[rowOff+x];const a=x>=bpp?out[o+x-bpp]:0;const b=y>0?out[o-stride+x]:0;const c=(y>0&&x>=bpp)?out[o-stride+x-bpp]:0;
      let v;
      switch(ft){case 0:v=rawV;break;case 1:v=rawV+a;break;case 2:v=rawV+b;break;case 3:v=rawV+((a+b)>>1);break;
        case 4:{const pp=a+b-c,da=Math.abs(pp-a),db=Math.abs(pp-b),dc=Math.abs(pp-c);v=rawV+((da<=db&&da<=dc)?a:(db<=dc?b:c));break;}
        default:throw new Error('Filtro PNG desconhecido: '+ft);}
      out[o+x]=v&255;
    }
  }
  // to RGBA
  const rgba=Buffer.alloc(w*h*4);
  for(let i=0,j=0;i<w*h;i++){const s=i*bpp;rgba[j++]=out[s];rgba[j++]=out[s+1];rgba[j++]=out[s+2];rgba[j++]=channels===4?out[s+3]:255;}
  return {w,h,rgba};
}

/* ---------- center-crop to square + bilinear resize ---------- */
function resizeSquare(src,N){
  const {w,h,rgba}=src;const side=Math.min(w,h);const ox=((w-side)/2)|0,oy=((h-side)/2)|0;
  const dst=Buffer.alloc(N*N*4);
  for(let dy=0;dy<N;dy++)for(let dx=0;dx<N;dx++){
    const sx=ox+(dx+0.5)/N*side-0.5, sy=oy+(dy+0.5)/N*side-0.5;
    const x0=Math.max(0,Math.min(w-1,Math.floor(sx))),y0=Math.max(0,Math.min(h-1,Math.floor(sy)));
    const x1=Math.min(w-1,x0+1),y1=Math.min(h-1,y0+1);const fx=sx-x0,fy=sy-y0;
    const o=(dy*N+dx)*4;
    for(let c=0;c<4;c++){
      const p00=rgba[(y0*w+x0)*4+c],p10=rgba[(y0*w+x1)*4+c],p01=rgba[(y1*w+x0)*4+c],p11=rgba[(y1*w+x1)*4+c];
      const top=p00+(p10-p00)*fx, bot=p01+(p11-p01)*fx;dst[o+c]=Math.round(top+(bot-top)*fy);
    }
  }
  return dst;
}

/* ---------- main ---------- */
const srcPath=process.argv[2];
if(!srcPath){console.error('Uso: node tools/make-icons.js <imagem.png>');process.exit(1);}
const src=decodePNG(fs.readFileSync(srcPath));
const dir=path.join(__dirname,'..','icons');
fs.mkdirSync(dir,{recursive:true});
fs.writeFileSync(path.join(dir,'icon-512.png'),encodePNG(512,512,resizeSquare(src,512)));
fs.writeFileSync(path.join(dir,'icon-192.png'),encodePNG(192,192,resizeSquare(src,192)));
console.log(`OK! Ícones gerados a partir de ${srcPath} (origem ${src.w}x${src.h}) -> icons/icon-192.png e icons/icon-512.png`);
