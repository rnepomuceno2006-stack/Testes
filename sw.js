/* Viva Parintins!!! — Service Worker (offline completo) */
const CACHE = 'viva-parintins-v1';
const ASSETS = [
  './',
  './index.html',
  './manifest.webmanifest',
  './icons/icon-192.png',
  './icons/icon-512.png',
  './lib/three/build/three.module.js',
  './lib/three/examples/jsm/postprocessing/EffectComposer.js',
  './lib/three/examples/jsm/postprocessing/RenderPass.js',
  './lib/three/examples/jsm/postprocessing/UnrealBloomPass.js',
  './lib/three/examples/jsm/postprocessing/OutlinePass.js',
  './lib/three/examples/jsm/postprocessing/OutputPass.js',
  './lib/three/examples/jsm/postprocessing/ShaderPass.js',
  './lib/three/examples/jsm/postprocessing/Pass.js',
  './lib/three/examples/jsm/utils/BufferGeometryUtils.js',
  './lib/three/examples/jsm/shaders/LuminosityHighPassShader.js',
  './lib/three/examples/jsm/shaders/CopyShader.js'
];

self.addEventListener('install', e => {
  self.skipWaiting();
  e.waitUntil(caches.open(CACHE).then(c => c.addAll(ASSETS)).catch(() => {}));
});

self.addEventListener('activate', e => {
  e.waitUntil(
    caches.keys().then(keys => Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k))))
      .then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', e => {
  const req = e.request;
  if (req.method !== 'GET') return;
  e.respondWith(
    caches.match(req).then(hit => hit || fetch(req).then(res => {
      const copy = res.clone();
      caches.open(CACHE).then(c => c.put(req, copy)).catch(() => {});
      return res;
    }).catch(() => caches.match('./index.html')))
  );
});
