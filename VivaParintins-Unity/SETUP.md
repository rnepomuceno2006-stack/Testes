# Viva Parintins!!! — Unity Setup

## 1. Requisitos
- Unity **2022.3 LTS** (Long Term Support)
- **Universal Render Pipeline (URP)** — crie o projeto como "3D (URP)"
- Android Build Support instalado no Unity Hub
- JDK 17 (instale via Unity Hub → Installs → seu Unity → Add modules)

---

## 2. Direção de Arte: Zelda Breath of the Wild + Festival de Parintins

O jogo usa o estilo **ToonScapes Spring Isles** como base visual:
- Ilhas flutuantes em alturas variadas (profundidade 3D real)
- Cel-shading URP com outline preto estilo cartoon
- Cores supersaturadas: vermelho/ouro (Garantido) e azul/roxo (Caprichoso)
- Personagens: sprites 2D painterly (imagens do Gemini) como billboards no mundo 3D
- Pós-processamento: Bloom + ACES Tonemapping

---

## 3. Assets Obrigatórios

### 🌿 Asset Principal — Cenário
| Asset | Link | Preço |
|-------|------|-------|
| **ToonScapes: Spring Isles** | [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/fantasy/toonscapes-spring-isles-361638) | ~$30 |

Use os prefabs: `FloatingIsland_Large/Medium`, `WaterFall_Animated`, `ToonTree_Tropical`, `RockFormation`, `StonePathBridge`

### 🎨 Shader e Pós-Processamento
| Asset | Link | Preço |
|-------|------|-------|
| **Fantasy Skybox FREE** | Asset Store | Grátis |
| **Particle Ribbon** | Asset Store | Grátis |
| **Toony Colors Pro 2** | Asset Store | ~$60 (opcional — URP built-in já resolve) |

### 🎭 Personagens (sprites 2D → billboards 3D)
Imagens geradas no Gemini já disponíveis — importe como Sprite e aplique em Quads com `BillboardController.cs`

### 🔊 Áudio
| Asset | Uso |
|-------|-----|
| Toadas do Festival (contactar associação) | Música principal |
| **Casual Game Sounds** (Asset Store, grátis) | SFX: hit, miss, win |
| **Jungle/Amazon Ambience** | Som de fundo nas ilhas |

---

## 4. Configuração URP + Cel-Shading (Zelda Style)

### Pipeline
1. Edit → Project Settings → Graphics → selecione o **URP Asset**
2. No URP Asset: Rendering Path = **Forward+**, HDR = On

### Outline (contorno preto nos objetos)
1. No URP Renderer (Universal Renderer Data):
   - Add Renderer Feature → **Full Screen Pass Renderer Feature**
   - Shader: crie um Shader Graph "Sobel Edge Outline" no depth buffer
   - Thickness: 1.5, Color: #1A1A1A

### Post-processing (Volume Global na cena)
```
Bloom:
  Threshold: 0.85
  Intensity: 0.5
  Scatter: 0.7

Color Adjustments:
  Saturation: +30   ← cores vibrantes do festival
  Contrast: +10

Tonemapping: ACES

Vignette:
  Color: #000000
  Intensity: 0.2
```

### Iluminação
```
Directional Light (Sun):
  Color: #FFF0C8 (luz amazônica quente)
  Intensity: 1.1
  Rotation: (45, -30, 0)

Environment (Lighting Settings):
  Sky Color:    #1A3A6B
  Horizon:      #FF6B2B  ← laranja pôr do sol Parintins
  Ground:       #2D4A1E

Fog: Linear, Start 30, End 80, Color #8EC5FC
```

---

## 5. Montar o Mapa (MapScene) — Ilhas Flutuantes

Siga as coordenadas em `MapLayoutGuide.cs`. Resumo:

| Fase | Local | Posição (X,Y,Z) | Altura |
|------|-------|-----------------|--------|
| 0 | Porto de Parintins | (-10, 0, -8) | Nível do rio |
| 1 | Feira de Artesanatos | (-5, 2, -5) | +2 |
| 2 | Mercado Municipal | (2, 1, -6) | +1 |
| 3 | Orla de Parintins | (-8, 3, 0) | +3 |
| 4 | Comunas Bar | (4, 4, 0) | +4 |
| 5 | Curral do Boi | (-3, 5, 5) | +5 |
| 6 | Praça dos Dois Bois | (3, 6, 5) | +6 |
| 7 | **Bumbódromo (BOSS)** | (0, 9, 10) | **+9 (topo)** |

**Câmera do mapa:**
- Perspective, FOV 45
- Position: (0, 22, -18), Rotation: (28, 0, 0)

**Conexões entre ilhas:**
- Fase concluída → ponte de pedra sólida (prefab ToonScapes StonePathBridge)
- Fase bloqueada → ponte de corda quebrada + cadeado flutuante

---

## 6. Importar Personagens (Sprites 2D → Mundo 3D)

1. Arraste as imagens para `Assets/Sprites/Characters/`
2. Texture Type: **Sprite (2D and UI)**, Alpha: Transparency
3. Para cada personagem:
   - Crie um **Quad** 3D na cena
   - Material com shader: `Universal Render Pipeline/Lit`
   - Adicione `BillboardController.cs` (modo: YAxisLocked)
   - Escala sugerida: (2, 3, 1) para personagens em pé

### Glow estilo Mario Galaxy nos sprites:
- No material: habilite **Emission**
- Garantido: `#FF2200` intensity 0.3
- Caprichoso: `#0044FF` intensity 0.3

---

## 7. Configurar Android

1. Edit → Project Settings → Player
2. **Company Name**: VivaParintins
3. **Product Name**: Viva Parintins!!!
4. **Package Name**: br.com.vivaparintins.game
5. **Minimum API Level**: Android 7.0 (API 24)
6. **Target API Level**: Android 14 (API 34)
7. **Graphics APIs**: remova Vulkan, deixe apenas OpenGLES3
8. Aba **Publishing Settings** → crie sua Keystore

---

## 8. Build do APK

1. File → Build Settings → Android → **Switch Platform**
2. Marque **Development Build** para testar
3. **Build** → escolha onde salvar o `.apk`
4. Para Play Store: **Build App Bundle (.aab)**

---

## 9. Cenas necessárias

| Cena | Script principal |
|------|-----------------|
| `IntroScene` | `IntroController.cs` |
| `MapScene` | `IsometricMapController.cs` |
| `Phase_Porto` ... `Phase_Bumbodromo` | `PhaseRunner.cs` |
| `MiniGameScene` | `MiniGameLoader.cs` |
| `GalleryScene` | `CharacterData` ScriptableObjects |

---

## 10. Personagens disponíveis (sprites prontos)

### Garantido ❤️ (9 imagens geradas)
Israel Paulain · David Assayag · João Paulo Faria · Daniela Tapajós ·
Raíra Lins · Lívia Cristina · Adriano Paketá · Isabelle Nogueira · Boi Garantido

### Caprichoso ⭐ (9 imagens geradas)
Edmundo Oran · Patrick Araújo · Caetano Medeiros · Marcela Marialva ·
Valentina Cid · Cleise Simas · Erick Beltrão · Marcyele Albuquerque · Boi Caprichoso

> Versão 2.0: Tuxaua, Mãe Catirina, Pai Francisco, Batucada/Marujada de cada time.
