# 🐂 Boi Impact — O Jogo do Festival de Parintins

Jogo de celular/tablet (PWA) inspirado no **Festival Folclórico de Parintins**, com visual
no estilo **Genshin Impact** e mecânica dinâmica no estilo **Candy Crush**.
Escolha seu boi — **Garantido** (♥ vermelho) ou **Caprichoso** (★ azul) — e dispute
os **21 quesitos oficiais** ao longo de **3 noites**.

## ✨ Recursos

- **21 quesitos oficiais** divididos nos 3 blocos reais do festival:
  - **Bloco A — Comuns e Musicais** (1ª noite)
  - **Bloco B — Cenografia e Coreografia** (2ª noite)
  - **Bloco C — Artísticos** (3ª noite)
- Cada quesito é um **mini-game** diferente: mira/timing, sequência (Simon), toque rápido,
  reação, perguntas (trivia), memória e **match-3 (Candy Crush)** nos quesitos musicais.
- **Notas de 0 a 10** por quesito; vence quem somar mais nas 3 noites (contra a CPU rival).
- **Toadas**: trilha sonora sintetizada (Web Audio) + refrões de cada boi durante os
  desafios musicais. *(Veja "Toadas 2026" abaixo.)*
- **Recompensas temáticas**: o Garantido ganha **♥ Corações** (vermelho) e o Caprichoso
  ganha **★ Estrelas** (prateada). Cada boi tem sua própria carteira.
- **Loja de Artefatos** (estilo Genshin): compre e equipe artefatos que dão habilidades
  (piso de nota, eliminar opção errada, +tempo, reação mais tolerante, mira mais lenta,
  +50% de recompensa). Progresso salvo no aparelho (localStorage).
- **Visual e animações**: estética de floresta amazônica, partículas, combos, confete,
  vibração de tela e efeitos — pensado para **celulares e tablets** (layout responsivo).

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

As **toadas oficiais de 2026** não estão embutidas (não foi possível confirmá-las).
O jogo usa, por ora, refrões/bordões representativos de cada boi e uma trilha sintetizada.
Para inserir as toadas reais, edite as listas em `index.html`:

```js
const TOADAS = {
  gar: [ "refrão 1...", "refrão 2..." ],
  cap: [ "refrão 1...", "refrão 2..." ]
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
