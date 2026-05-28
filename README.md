# 🐂 Boi Impact — O Jogo do Festival de Parintins

Jogo de celular/tablet (PWA) inspirado no **Festival Folclórico de Parintins**, com visual
3D **cel-shading no estilo Genshin Impact** e jogabilidade de exploração/aventura no estilo
**Zelda: Ocarina of Time**. Escolha seu boi — **Garantido** (♥ vermelho) ou **Caprichoso**
(★ azul) — e ajude a recuperar os objetos sagrados dos quesitos ao longo de **3 noites**.

## ✨ Recursos

- **Aventura em mundo 3D**: você é o **torcedor** do boi escolhido e explora a cidade de
  Parintins — Bumbódromo, Cidade do Boi, Porto Amazonas, Praça da Catedral e a floresta.
- **Missões estilo Zelda**: os itens/quesitos do boi (Pajé, Cunhã-Poranga, Levantador de
  Toadas, Porta-Estandarte, Amo do Boi…) perderam objetos sagrados. Converse com o povo da
  cidade, **siga as pistas**, **encontre o objeto perdido** e **devolva ao seu dono**.
- **3 noites de festival**: recupere os quesitos de cada noite; vence o boi com mais pontos
  (contra o boi rival, controlado pela CPU).
- **Visual cel-shading**: `MeshToonMaterial`, contornos escuros (OutlinePass) e brilho
  mágico (UnrealBloomPass), com partículas elementais — estética Genshin Impact em Three.js.
- **Toadas 2026**: trilha sintetizada no clima de cada boi + refrões/temas exibidos em tela
  nos momentos-chave. *(Veja "Toadas 2026" abaixo.)*
- **Recompensas temáticas**: o Garantido ganha **♥ Corações** (vermelho) e o Caprichoso
  ganha **★ Estrelas** (prateada). Cada boi tem sua própria carteira (localStorage).
- **Controles**: WASD/setas + mouse no PC; direcional virtual + botões no celular/tablet.

## 📱 Como jogar / instalar no Android (PWA)

O jogo funciona em qualquer navegador. Para instalar como **app** na tela inicial,
ele precisa ser servido por **HTTPS** (ex.: GitHub Pages):

1. No GitHub, vá em **Settings → Pages**.
2. Em **Source**, escolha esta branch e a pasta **/ (root)**. Salve.
3. Abra a URL gerada (algo como `https://<usuario>.github.io/<repo>/`) no **Chrome do Android**.
4. Toque no menu **⋮ → Instalar app** (ou "Adicionar à tela inicial").
5. Pronto: abre em tela cheia, com ícone, e funciona **offline**.

> Também dá para só abrir o `index.html` no navegador para jogar — mas a instalação
> como app e o modo offline exigem HTTPS.

## 🎵 Toadas 2026

O jogo referencia os **temas oficiais de 2026** de cada boi:

- **Garantido** — *"Parintins: Portal do Encantamento"* (toadas de galera, lenda/ritual e de amo)
- **Caprichoso** — *"Brinquedo que Canta seu Chão"* (tema homônimo, *É Hoje!*, *Povo do Norte*, *Trilha do Curupira*, itens femininos)

Refrões/bordões curtos aparecem em momentos-chave (abertura, recuperação de item, virada de
noite, vitória) e a trilha ambiente sintetizada usa a escala/clima de cada boi. As **letras
completas não estão embutidas** (são protegidas por direitos autorais). Para colá-las, edite o
objeto `TOADAS` em `index.html`:

```js
const TOADAS = {
  gar: { tema:'Parintins: Portal do Encantamento', abertura:'...', galera:'...', lenda:'...', feminino:'...', amo:'...', vitoria:'...' },
  cap: { tema:'Brinquedo que Canta seu Chão',       abertura:'...', galera:'...', lenda:'...', feminino:'...', amo:'...', vitoria:'...' }
};
```

## 🎨 Ícone do app

O ícone padrão (`icons/icon-192.png` e `icons/icon-512.png`) é gerado por código.
Para usar **sua própria arte**, coloque um PNG (de preferência quadrado) no repositório e rode:

```bash
node tools/make-icons.js caminho/da/sua-imagem.png
```

Isso recorta no centro e gera os dois tamanhos automaticamente.

## 🗂️ Arquivos

| Arquivo | Descrição |
|---|---|
| `index.html` | O jogo completo (HTML + CSS + JS em um arquivo) |
| `manifest.webmanifest` | Manifesto do PWA (nome, ícones, tela cheia) |
| `sw.js` | Service worker (funciona offline) |
| `icons/` | Ícones do app (192 e 512) |
| `tools/make-icons.js` | Gera os ícones a partir de uma imagem PNG |
