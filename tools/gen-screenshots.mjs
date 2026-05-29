/**
 * Gera screenshots estilo Play Store para o Viva Parintins!!!
 * Cada screenshot é 1080×1920px (portrait) com sobreposição de texto promocional.
 * Uso: node tools/gen-screenshots.mjs
 */
import { chromium } from 'playwright';
import { writeFileSync, mkdirSync } from 'fs';

mkdirSync('screenshots', { recursive: true });

const W = 1080, H = 1920;

// ── Paleta e fontes ──────────────────────────────────────────────────────────
const GOLD   = '#c8a96e';
const RED    = '#e2231a';
const BLUE   = '#1565c0';
const DARK   = '#0a0d18';
const WHITE  = '#f0e8d0';

// ── Cada screenshot é um HTML completo renderizado pelo headless Chrome ──────
const screens = [

  /* ── 1. TELA INICIAL — batalha dos dois bois ── */
  {
    name: '01-intro-batalha',
    label: 'Tela Inicial',
    html: `<!DOCTYPE html><html><head><meta charset="utf-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box;}
  body{width:${W}px;height:${H}px;overflow:hidden;background:#0a0d18;font-family:'Georgia',serif;color:#f0e8d0;}
  /* fundo estrelado */
  .bg{position:absolute;inset:0;background:
    radial-gradient(ellipse at 20% 50%,rgba(180,10,10,.45) 0%,transparent 55%),
    radial-gradient(ellipse at 80% 50%,rgba(10,30,160,.45) 0%,transparent 55%),
    radial-gradient(ellipse at 50% 0%,rgba(200,169,110,.08) 0%,transparent 60%),
    #0a0d18;}
  /* estrelas */
  .stars{position:absolute;inset:0;}
  /* linha divisória dourada */
  .divide{position:absolute;left:50%;top:5%;bottom:5%;width:3px;transform:translateX(-50%);
    background:linear-gradient(180deg,transparent,#c8a96e 15%,#ffe15a 50%,#c8a96e 85%,transparent);
    box-shadow:0 0 24px #c8a96e,0 0 60px rgba(200,169,110,.4);}
  /* cabeçalho */
  .head{position:absolute;top:0;left:0;right:0;padding:60px 40px 30px;text-align:center;z-index:10;}
  .title{font-size:80px;font-weight:900;letter-spacing:2px;line-height:1;
    background:linear-gradient(180deg,#ffe15a,#c8a96e 50%,#a07830);
    -webkit-background-clip:text;-webkit-text-fill-color:transparent;}
  .subtitle{font-size:26px;letter-spacing:6px;color:#c8a96e;margin-top:10px;opacity:.8;}
  .countdown{display:inline-flex;gap:16px;margin-top:20px;background:rgba(200,169,110,.1);
    border:1px solid rgba(200,169,110,.4);border-radius:16px;padding:14px 28px;}
  .cbox{text-align:center;}
  .cnum{font-size:42px;font-weight:900;color:#ffe15a;line-height:1;}
  .clbl{font-size:14px;letter-spacing:3px;opacity:.5;margin-top:3px;}
  /* painéis boi */
  .panels{position:absolute;top:340px;left:0;right:0;bottom:360px;display:flex;}
  .panel{flex:1;display:flex;flex-direction:column;align-items:center;padding:30px 24px;gap:14px;}
  .boi-emoji{font-size:110px;line-height:1;filter:drop-shadow(0 0 30px currentColor);}
  .boi-name{font-size:38px;font-weight:900;letter-spacing:1px;}
  .boi-tema{font-size:18px;font-style:italic;opacity:.7;text-align:center;max-width:280px;line-height:1.4;}
  .cast{width:100%;display:flex;flex-direction:column;gap:10px;margin-top:8px;}
  .cast-item{display:flex;align-items:center;gap:12px;padding:10px 14px;border-radius:12px;font-size:17px;}
  .gar .cast-item{background:rgba(226,35,26,.12);border:1px solid rgba(226,35,26,.25);}
  .cap .cast-item{background:rgba(21,101,192,.12);border:1px solid rgba(21,101,192,.25);}
  .cast-emoji{font-size:22px;width:28px;text-align:center;}
  .cast-name{font-weight:700;font-size:16px;}
  .cast-role{font-size:12px;opacity:.5;margin-top:1px;}
  /* rodapé botões */
  .footer{position:absolute;bottom:0;left:0;right:0;padding:30px 32px 50px;display:flex;gap:20px;}
  .btn-gar{flex:1;background:linear-gradient(180deg,#e2231a,#8b0000);
    border:2px solid rgba(255,100,100,.5);border-radius:20px;padding:30px 20px;text-align:center;
    font-size:24px;font-weight:900;letter-spacing:1px;color:#fff;
    box-shadow:0 8px 30px rgba(226,35,26,.5);}
  .btn-cap{flex:1;background:linear-gradient(180deg,#1565c0,#0a2a6b);
    border:2px solid rgba(100,150,255,.5);border-radius:20px;padding:30px 20px;text-align:center;
    font-size:24px;font-weight:900;letter-spacing:1px;color:#fff;
    box-shadow:0 8px 30px rgba(21,101,192,.5);}
  .btn-big{font-size:30px;display:block;margin-bottom:6px;}
  .vs{position:absolute;left:50%;top:50%;transform:translate(-50%,-50%);
    background:#0a0d18;border:2px solid #c8a96e;border-radius:50%;width:90px;height:90px;
    display:flex;align-items:center;justify-content:center;z-index:20;
    font-size:28px;font-weight:900;color:#ffe15a;box-shadow:0 0 30px rgba(200,169,110,.5);}
</style></head><body>
<div class="bg"></div>
<div class="divide"></div>

<div class="head">
  <div class="title">🐂 VIVA PARINTINS!!!</div>
  <div class="subtitle">FESTIVAL FOLCLÓRICO 2026</div>
  <div class="countdown">
    <div class="cbox"><div class="cnum">28</div><div class="clbl">DIAS</div></div>
    <div class="cbox"><div class="cnum">14</div><div class="clbl">HORAS</div></div>
    <div class="cbox"><div class="cnum">32</div><div class="clbl">MIN</div></div>
  </div>
</div>

<div class="panels">
  <div class="panel gar">
    <div class="boi-emoji" style="color:#e2231a">🐂❤️</div>
    <div class="boi-name" style="color:#ff6b6b">BOI GARANTIDO</div>
    <div class="boi-tema">"Portal do Encantamento"</div>
    <div class="cast">
      <div class="cast-item"><span class="cast-emoji">💃</span><div><div class="cast-name">Isabelle Nogueira</div><div class="cast-role">Cunhã-Poranga</div></div></div>
      <div class="cast-item"><span class="cast-emoji">🎤</span><div><div class="cast-name">David Assayag</div><div class="cast-role">Levantador de Toadas</div></div></div>
      <div class="cast-item"><span class="cast-emoji">🪶</span><div><div class="cast-name">Adriano Paketá</div><div class="cast-role">Pajé</div></div></div>
      <div class="cast-item"><span class="cast-emoji">👑</span><div><div class="cast-name">Lívia Cristina</div><div class="cast-role">Rainha do Folclore</div></div></div>
      <div class="cast-item"><span class="cast-emoji">🤠</span><div><div class="cast-name">João Paulo Faria</div><div class="cast-role">Amo do Boi</div></div></div>
    </div>
  </div>
  <div class="panel cap">
    <div class="boi-emoji" style="color:#5aa0ff">🐃⭐</div>
    <div class="boi-name" style="color:#7ab8ff">BOI CAPRICHOSO</div>
    <div class="boi-tema">"Brinquedo que Canta seu Chão"</div>
    <div class="cast">
      <div class="cast-item"><span class="cast-emoji">🎤</span><div><div class="cast-name">Patrick Araujo</div><div class="cast-role">Levantador de Toadas</div></div></div>
      <div class="cast-item"><span class="cast-emoji">🌙</span><div><div class="cast-name">Erick Beltrão</div><div class="cast-role">Pajé</div></div></div>
      <div class="cast-item"><span class="cast-emoji">🦜</span><div><div class="cast-name">Marcyele Albuquerque</div><div class="cast-role">Cunhã-Poranga</div></div></div>
      <div class="cast-item"><span class="cast-emoji">👑</span><div><div class="cast-name">Cleise Simas</div><div class="cast-role">Rainha do Folclore</div></div></div>
      <div class="cast-item"><span class="cast-emoji">🎙️</span><div><div class="cast-name">Edmundo Oran</div><div class="cast-role">Apresentador</div></div></div>
    </div>
  </div>
</div>
<div class="vs">⚔️ VS</div>

<div class="footer">
  <div class="btn-gar"><span class="btn-big">❤️</span>JOGAR GARANTIDO</div>
  <div class="btn-cap"><span class="btn-big">⭐</span>JOGAR CAPRICHOSO</div>
</div>
</body></html>`
  },

  /* ── 2. MAPA DE FASES — trilha do festival ── */
  {
    name: '02-mapa-fases',
    label: 'Mapa de Fases',
    html: `<!DOCTYPE html><html><head><meta charset="utf-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box;}
  body{width:${W}px;height:${H}px;overflow:hidden;background:#0a0d18;font-family:'Georgia',serif;color:#f0e8d0;}
  .bg{position:absolute;inset:0;background:
    radial-gradient(ellipse at 50% 0%,rgba(200,169,110,.12) 0%,transparent 50%),
    radial-gradient(ellipse at 20% 100%,rgba(226,35,26,.2) 0%,transparent 40%),
    radial-gradient(ellipse at 80% 80%,rgba(21,101,192,.2) 0%,transparent 40%),#0a0d18;}
  header{position:relative;z-index:2;padding:50px 40px 20px;text-align:center;}
  .title{font-size:52px;font-weight:900;color:#ffe15a;line-height:1;}
  .sub{font-size:20px;letter-spacing:3px;color:#c8a96e;margin-top:8px;}
  .tug{display:flex;height:14px;border-radius:7px;overflow:hidden;margin:20px 40px 0;box-shadow:0 0 20px rgba(0,0,0,.5);}
  .tug-gar{background:linear-gradient(90deg,#e2231a,#ff6b6b);flex:52}
  .tug-cap{background:linear-gradient(90deg,#5aa0ff,#1565c0);flex:48}
  .tug-labels{display:flex;justify-content:space-between;padding:6px 40px;font-size:18px;font-weight:700;}
  .path{position:relative;z-index:2;padding:20px 40px;display:flex;flex-direction:column;gap:0;}
  .night-label{font-size:20px;letter-spacing:3px;color:#c8a96e;text-align:center;padding:14px 0 10px;
    border-bottom:1px solid rgba(200,169,110,.2);margin-bottom:6px;}
  .phase{display:flex;align-items:center;gap:20px;padding:16px 20px;border-radius:16px;
    background:rgba(255,255,255,.04);border:1px solid rgba(200,169,110,.15);margin-bottom:8px;}
  .phase.done{background:rgba(200,169,110,.1);border-color:rgba(200,169,110,.35);}
  .phase.done .phase-icon{animation:none;}
  .phase-icon{font-size:36px;width:52px;text-align:center;}
  .phase-info{flex:1;}
  .phase-name{font-size:20px;font-weight:700;color:#ffe15a;}
  .phase-q{font-size:14px;opacity:.6;margin-top:2px;}
  .phase-type{font-size:12px;color:#c8a96e;margin-top:3px;letter-spacing:1px;}
  .phase-stars{font-size:20px;color:#ffe15a;letter-spacing:2px;}
  .connector{width:4px;height:14px;background:repeating-linear-gradient(180deg,#c8a96e 0 5px,transparent 5px 10px);
    margin:0 auto;opacity:.6;}
</style></head><body>
<div class="bg"></div>
<header>
  <div class="title">🎪 MAPA DO FESTIVAL</div>
  <div class="sub">PARINTINS — 13 FASES · 3 NOITES</div>
  <div class="tug"><div class="tug-gar"></div><div class="tug-cap"></div></div>
  <div class="tug-labels"><span style="color:#ff6b6b">❤️ Garantido 52k</span><span style="color:#7ab8ff">48k ⭐ Caprichoso</span></div>
</header>
<div class="path">
  <div class="night-label">🌙 NOITE 1 — A LENDA DO BOI</div>
  <div class="phase done"><div class="phase-icon">🏹</div><div class="phase-info"><div class="phase-name">Porto dos Caboclos</div><div class="phase-q">Pai Francisco</div><div class="phase-type">❓ Quiz da Torcida</div></div><div class="phase-stars">★★★</div></div>
  <div class="connector"></div>
  <div class="phase done"><div class="phase-icon">🤰</div><div class="phase-info"><div class="phase-name">Casa da Ribeira</div><div class="phase-q">Mãe Catirina</div><div class="phase-type">🤫 Regra do Contrário</div></div><div class="phase-stars">★★☆</div></div>
  <div class="connector"></div>
  <div class="phase"><div class="phase-icon">🐂</div><div class="phase-info"><div class="phase-name">Curral do Boi</div><div class="phase-q">Boi-Bumbá</div><div class="phase-type">🥁 Ritmo de Toada</div></div><div class="phase-stars">☆☆☆</div></div>
  <div class="connector"></div>
  <div class="night-label">🌟 NOITE 2 — OS DESTAQUES</div>
  <div class="phase" style="opacity:.5"><div class="phase-icon">🔒</div><div class="phase-info"><div class="phase-name">Orla do Amazonas</div><div class="phase-q">Rainha do Folclore</div><div class="phase-type">🥁 Ritmo de Toada</div></div><div class="phase-stars"></div></div>
  <div class="connector" style="opacity:.3"></div>
  <div class="phase" style="opacity:.5"><div class="phase-icon">🔒</div><div class="phase-info"><div class="phase-name">Praça da Catedral</div><div class="phase-q">Cunhã-Poranga</div><div class="phase-type">🥁 Ritmo de Toada</div></div><div class="phase-stars"></div></div>
  <div class="connector" style="opacity:.3"></div>
  <div class="night-label" style="opacity:.5">🏆 NOITE 3 — HONRA E BATALHA</div>
</div>
</body></html>`
  },

  /* ── 3. MINI-GAME RITMO ── */
  {
    name: '03-ritmo-toada',
    label: 'Ritmo de Toada',
    html: `<!DOCTYPE html><html><head><meta charset="utf-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box;}
  body{width:${W}px;height:${H}px;overflow:hidden;background:#0a0d18;font-family:'Georgia',serif;color:#f0e8d0;}
  .bg{position:absolute;inset:0;
    background:radial-gradient(ellipse at 50% 30%,rgba(226,35,26,.25) 0%,transparent 55%),#0a0d18;}
  header{position:relative;z-index:2;padding:60px 40px 20px;display:flex;justify-content:space-between;align-items:center;}
  .mg-title{font-size:46px;font-weight:900;color:#ffe15a;}
  .mg-score{font-size:80px;font-weight:900;color:#ffe15a;text-shadow:0 0 30px rgba(255,225,80,.6);}
  .combo{font-size:32px;color:#ff9944;text-align:center;margin:0 40px 20px;}
  /* barra de progresso */
  .prog-wrap{margin:0 40px;height:12px;background:rgba(255,255,255,.1);border-radius:6px;overflow:hidden;}
  .prog-fill{height:100%;width:40%;background:linear-gradient(90deg,#e2231a,#ff6b6b);border-radius:6px;
    box-shadow:0 0 12px rgba(226,35,26,.6);}
  /* lane de notas */
  .lane{position:relative;z-index:2;margin:30px 40px 0;height:900px;background:rgba(255,255,255,.03);
    border:1px solid rgba(200,169,110,.15);border-radius:20px;overflow:hidden;}
  .hitline{position:absolute;bottom:80px;left:20px;right:20px;height:4px;
    background:rgba(255,225,80,.4);box-shadow:0 0 16px rgba(255,225,80,.5);border-radius:2px;}
  .note{position:absolute;left:50%;transform:translateX(-50%);font-size:52px;
    filter:drop-shadow(0 0 14px rgba(226,35,26,.9));}
  .judge{position:absolute;top:40%;left:50%;transform:translateX(-50%);
    font-size:80px;font-weight:900;color:#ffe15a;text-shadow:0 0 30px #ffe15a;opacity:.9;white-space:nowrap;}
  /* botão toque */
  .tap-btn{position:relative;z-index:2;margin:28px 60px 0;padding:40px;border-radius:24px;
    background:linear-gradient(180deg,#e2231a,#8b0000);
    border:2px solid rgba(255,100,100,.5);text-align:center;font-size:36px;font-weight:900;
    box-shadow:0 12px 40px rgba(226,35,26,.6),inset 0 1px 0 rgba(255,255,255,.1);}
  /* pontos flutuantes */
  .float-pts{position:absolute;font-size:32px;font-weight:900;color:#ffe15a;
    text-shadow:0 0 16px #ffe15a;opacity:.8;}
  .badge{position:absolute;top:20px;right:20px;background:rgba(200,169,110,.15);
    border:1px solid rgba(200,169,110,.4);border-radius:12px;padding:8px 18px;font-size:18px;color:#c8a96e;}
</style></head><body>
<div class="bg"></div>
<header>
  <div>
    <div class="mg-title">🥁 Curral do Boi</div>
    <div style="font-size:18px;opacity:.6;margin-top:4px;">Ritmo de Toada — Noite 1</div>
  </div>
  <div class="mg-score">1.850</div>
</header>
<div class="combo">🔥 Combo x7 — PERFEITO!</div>
<div class="prog-wrap"><div class="prog-fill"></div></div>
<div class="lane">
  <div class="hitline"></div>
  <!-- notas em posições diferentes -->
  <div class="note" style="top:60px">❤️</div>
  <div class="note" style="top:210px">❤️</div>
  <div class="note" style="top:370px">❤️</div>
  <div class="note" style="top:520px;font-size:38px;opacity:.6">❤️</div>
  <!-- julgamento -->
  <div class="judge">PERFEITO!</div>
  <!-- pontos flutuando -->
  <div class="float-pts" style="top:48%;left:55%">+150</div>
  <div class="float-pts" style="top:55%;left:30%;opacity:.5;font-size:24px">+100</div>
  <div class="badge">Noite 1 / 3</div>
</div>
<div class="tap-btn">❤️ TOCAR NA BATIDA</div>
</body></html>`
  },

  /* ── 4. GALERIA DE PERSONAGENS ── */
  {
    name: '04-galeria-personagens',
    label: 'Galeria de Lendas',
    html: `<!DOCTYPE html><html><head><meta charset="utf-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box;}
  body{width:${W}px;height:${H}px;overflow:hidden;background:#0a0d18;font-family:'Georgia',serif;color:#f0e8d0;}
  .bg{position:absolute;inset:0;
    background:radial-gradient(ellipse at 50% 0%,rgba(200,169,110,.1) 0%,transparent 50%),#0a0d18;}
  header{position:relative;z-index:2;padding:56px 40px 24px;}
  .title{font-size:52px;font-weight:900;color:#ffe15a;}
  .sub{font-size:18px;color:#c8a96e;margin-top:6px;letter-spacing:2px;}
  .tabs{display:flex;gap:16px;margin-top:20px;}
  .tab{padding:10px 28px;border-radius:30px;font-size:18px;font-weight:700;border:1px solid;}
  .tab.all{background:rgba(200,169,110,.2);border-color:rgba(200,169,110,.5);color:#c8a96e;}
  .tab.gar{background:rgba(226,35,26,.15);border-color:rgba(226,35,26,.4);color:#ff8080;}
  .tab.cap{background:rgba(21,101,192,.15);border-color:rgba(21,101,192,.4);color:#7ab8ff;}
  /* grid de cards */
  .grid{position:relative;z-index:2;padding:20px 30px;display:grid;grid-template-columns:repeat(3,1fr);gap:18px;}
  .card{border-radius:18px;padding:20px 14px;text-align:center;position:relative;overflow:hidden;
    border:1px solid;}
  .card.gar{background:linear-gradient(180deg,rgba(226,35,26,.15),rgba(100,5,5,.3));border-color:rgba(226,35,26,.35);}
  .card.cap{background:linear-gradient(180deg,rgba(21,101,192,.15),rgba(5,5,80,.3));border-color:rgba(21,101,192,.35);}
  .card.leg{border-color:rgba(255,225,80,.7)!important;box-shadow:0 0 20px rgba(255,225,80,.25);}
  .card-emoji{font-size:52px;line-height:1;display:block;margin-bottom:6px;}
  .card-name{font-size:14px;font-weight:700;line-height:1.2;margin-bottom:4px;}
  .card-real{font-size:11px;opacity:.55;margin-bottom:8px;}
  .stars{font-size:16px;color:#ffe15a;letter-spacing:1px;}
  .holo{position:absolute;inset:0;border-radius:inherit;
    background:linear-gradient(125deg,rgba(255,0,128,.08) 0%,rgba(255,255,0,.08) 25%,rgba(0,255,128,.08) 50%,rgba(0,200,255,.08) 75%,rgba(128,0,255,.08) 100%);
    background-size:200% 200%;}
  .stats{display:flex;justify-content:center;gap:8px;margin-top:8px;}
  .stat{font-size:11px;opacity:.6;}
  /* card destaque aberto */
  .card-detail{position:absolute;z-index:10;left:30px;right:30px;top:900px;
    background:rgba(10,13,24,.97);border:1px solid rgba(200,169,110,.5);border-radius:24px;
    padding:36px 28px;box-shadow:0 0 60px rgba(0,0,0,.8),0 0 30px rgba(200,169,110,.15);}
  .cd-emoji{font-size:90px;text-align:center;display:block;margin-bottom:14px;}
  .cd-role{font-size:18px;color:#c8a96e;letter-spacing:2px;text-align:center;}
  .cd-name{font-size:36px;font-weight:900;text-align:center;color:#ffe15a;margin:4px 0 6px;}
  .cd-real{font-size:20px;text-align:center;opacity:.7;margin-bottom:14px;}
  .cd-stars{text-align:center;font-size:32px;color:#ffe15a;margin-bottom:16px;}
  .cd-stats{display:grid;grid-template-columns:1fr 1fr;gap:10px;margin-bottom:16px;}
  .cd-stat{background:rgba(255,255,255,.05);border-radius:10px;padding:10px 14px;font-size:16px;}
  .cd-stat span{color:#c8a96e;font-size:12px;display:block;margin-bottom:2px;}
  .cd-lore{font-size:15px;opacity:.7;line-height:1.5;text-align:center;}
</style></head><body>
<div class="bg"></div>
<header>
  <div class="title">🌟 LENDAS DE PARINTINS</div>
  <div class="sub">26 PERSONAGENS — GARANTIDO &amp; CAPRICHOSO</div>
  <div class="tabs">
    <div class="tab all">Todos</div>
    <div class="tab gar">❤️ Garantido</div>
    <div class="tab cap">⭐ Caprichoso</div>
  </div>
</header>
<div class="grid">
  <div class="card gar leg"><div class="holo"></div><span class="card-emoji">🐂</span><div class="card-name">Boi Garantido</div><div class="card-real">Tripa: Denison Piçanã</div><div class="stars">★★★</div></div>
  <div class="card cap leg"><div class="holo"></div><span class="card-emoji">🐃</span><div class="card-name">Boi Caprichoso</div><div class="card-real">A Estrela da Amazônia</div><div class="stars">★★★</div></div>
  <div class="card gar leg"><div class="holo"></div><span class="card-emoji">💃</span><div class="card-name">Cunhã-Poranga</div><div class="card-real">Isabelle Nogueira</div><div class="stars">★★★</div></div>
  <div class="card cap leg"><div class="holo"></div><span class="card-emoji">🦜</span><div class="card-name">Cunhã-Poranga</div><div class="card-real">Marcyele Albuquerque</div><div class="stars">★★★</div></div>
  <div class="card gar"><span class="card-emoji">🎤</span><div class="card-name">Levantador</div><div class="card-real">David Assayag</div><div class="stars">★★★</div></div>
  <div class="card cap"><span class="card-emoji">🎤</span><div class="card-name">Levantador</div><div class="card-real">Patrick Araujo</div><div class="stars">★★★</div></div>
</div>
<!-- card aberto em destaque -->
<div class="card-detail">
  <span class="cd-emoji">🌙</span>
  <div class="cd-role">PAJÉ · BOI CAPRICHOSO</div>
  <div class="cd-name">Pajé do Caprichoso</div>
  <div class="cd-real">Erick Beltrão</div>
  <div class="cd-stars">★★★</div>
  <div class="cd-stats">
    <div class="cd-stat"><span>🐂 FORÇA</span>80</div>
    <div class="cd-stat"><span>🪶 ENCANTO</span>95</div>
    <div class="cd-stat"><span>🥁 TOADA</span>75</div>
    <div class="cd-stat"><span>⭐ PAIXÃO</span>70</div>
  </div>
  <div class="cd-lore">Erick Beltrão — o Pajé dos Pajés. Pintura de onça, penas roxas e azuis, cajado sagrado. O ritual misterioso do Caprichoso começa com ele.</div>
</div>
</body></html>`
  },

  /* ── 5. VOTAÇÃO AO VIVO — Noite 3 ── */
  {
    name: '05-votacao-vivo',
    label: 'Votação ao Vivo',
    html: `<!DOCTYPE html><html><head><meta charset="utf-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box;}
  body{width:${W}px;height:${H}px;overflow:hidden;background:#0a0d18;font-family:'Georgia',serif;color:#f0e8d0;}
  .bg{position:absolute;inset:0;
    background:radial-gradient(ellipse at 30% 40%,rgba(226,35,26,.3) 0%,transparent 40%),
               radial-gradient(ellipse at 70% 60%,rgba(21,101,192,.3) 0%,transparent 40%),#0a0d18;}
  .header{position:relative;z-index:2;padding:60px 40px 30px;text-align:center;}
  .live-badge{display:inline-flex;align-items:center;gap:12px;background:rgba(226,35,26,.2);
    border:2px solid rgba(226,35,26,.6);border-radius:30px;padding:12px 28px;margin-bottom:20px;
    font-size:24px;font-weight:900;letter-spacing:2px;color:#ff6b6b;}
  .live-dot{width:16px;height:16px;background:#ff3030;border-radius:50%;
    box-shadow:0 0 12px #ff3030;}
  .title{font-size:58px;font-weight:900;color:#ffe15a;line-height:1;}
  .sub{font-size:22px;color:#c8a96e;margin-top:10px;}
  .timer{font-size:100px;font-weight:900;color:#fff;
    text-shadow:0 0 40px rgba(255,255,255,.4);margin:20px 0 0;}
  .timer-lbl{font-size:18px;letter-spacing:3px;opacity:.5;margin-top:4px;}
  /* barra de disputa */
  .tug-wrap{position:relative;z-index:2;margin:40px 40px 0;}
  .tug-labels{display:flex;justify-content:space-between;font-size:32px;font-weight:900;margin-bottom:12px;}
  .tug-bar{display:flex;height:52px;border-radius:26px;overflow:hidden;
    box-shadow:0 8px 30px rgba(0,0,0,.5);}
  .tug-gar{background:linear-gradient(90deg,#8b0000,#e2231a,#ff6b6b);flex:54;
    display:flex;align-items:center;justify-content:flex-end;padding-right:20px;
    font-size:24px;font-weight:900;color:#fff;}
  .tug-cap{background:linear-gradient(90deg,#5aa0ff,#1565c0,#0a2a6b);flex:46;
    display:flex;align-items:center;padding-left:20px;
    font-size:24px;font-weight:900;color:#fff;}
  /* botões de voto */
  .vote-area{position:relative;z-index:2;margin:50px 40px 0;display:flex;gap:24px;}
  .vote-btn{flex:1;border-radius:24px;padding:0;overflow:hidden;cursor:pointer;
    box-shadow:0 12px 40px rgba(0,0,0,.5);}
  .vote-btn-inner{padding:40px 20px;text-align:center;}
  .vote-btn.gar .vote-btn-inner{background:linear-gradient(180deg,#e2231a,#6b0000);}
  .vote-btn.cap .vote-btn-inner{background:linear-gradient(180deg,#1565c0,#0a1a5c);}
  .vote-emoji{font-size:90px;display:block;margin-bottom:10px;}
  .vote-label{font-size:26px;font-weight:900;}
  .vote-count{font-size:18px;opacity:.7;margin-top:6px;}
  /* placar total */
  .scores{position:relative;z-index:2;margin:30px 40px 0;display:flex;gap:20px;}
  .score-box{flex:1;background:rgba(255,255,255,.04);border:1px solid rgba(200,169,110,.2);
    border-radius:16px;padding:20px;text-align:center;}
  .score-val{font-size:44px;font-weight:900;line-height:1;}
  .score-lbl{font-size:14px;letter-spacing:2px;opacity:.5;margin-top:4px;}
  /* call to action */
  .cta{position:relative;z-index:2;margin:32px 40px 0;text-align:center;}
  .cta p{font-size:20px;opacity:.6;line-height:1.5;}
  .share-btn{display:inline-block;margin-top:20px;background:linear-gradient(180deg,#c8a96e,#7a5c20);
    border-radius:16px;padding:20px 50px;font-size:22px;font-weight:900;color:#1a1000;}
</style></head><body>
<div class="bg"></div>
<div class="header">
  <div class="live-badge"><div class="live-dot"></div> AO VIVO</div>
  <div class="title">🏆 GRANDE FINAL</div>
  <div class="sub">Bumbódromo · Noite 3 · Parintins 2026</div>
  <div class="timer">12</div>
  <div class="timer-lbl">SEGUNDOS RESTANTES</div>
</div>
<div class="tug-wrap">
  <div class="tug-labels">
    <span style="color:#ff6b6b">❤️ 54%</span>
    <span style="color:#7ab8ff">46% ⭐</span>
  </div>
  <div class="tug-bar">
    <div class="tug-gar">GARANTIDO</div>
    <div class="tug-cap">CAPRICHOSO</div>
  </div>
</div>
<div class="vote-area">
  <div class="vote-btn gar"><div class="vote-btn-inner">
    <span class="vote-emoji">❤️</span>
    <div class="vote-label">GARANTIDO</div>
    <div class="vote-count">127.430 votos</div>
  </div></div>
  <div class="vote-btn cap"><div class="vote-btn-inner">
    <span class="vote-emoji">⭐</span>
    <div class="vote-label">CAPRICHOSO</div>
    <div class="vote-count">109.810 votos</div>
  </div></div>
</div>
<div class="scores">
  <div class="score-box">
    <div class="score-val" style="color:#ff6b6b">1.248</div>
    <div class="score-lbl">SEUS PONTOS</div>
  </div>
  <div class="score-box">
    <div class="score-val" style="color:#ffe15a">🏅 3º</div>
    <div class="score-lbl">RANKING NACIONAL</div>
  </div>
</div>
<div class="cta">
  <p>Defenda seu boi e ajude a decidir o campeão do Festival de Parintins 2026!</p>
  <div class="share-btn">📲 Desafiar Amigos</div>
</div>
</body></html>`
  },
];

// ── Render com Playwright ────────────────────────────────────────────────────
const browser = await chromium.launch({
  executablePath: '/opt/pw-browsers/chromium-1194/chrome-linux/chrome',
  args: ['--no-sandbox','--disable-setuid-sandbox']
});

const files = [];
for (const s of screens) {
  const page = await browser.newPage();
  await page.setViewportSize({ width: W, height: H });
  await page.setContent(s.html, { waitUntil: 'networkidle' });
  await page.waitForTimeout(300);
  const path = `screenshots/${s.name}.png`;
  await page.screenshot({ path, fullPage: false });
  await page.close();
  files.push(path);
  console.log(`✓  ${s.label} → ${path}`);
}

await browser.close();
console.log(`\n✅ ${files.length} screenshots gerados em /screenshots/`);
