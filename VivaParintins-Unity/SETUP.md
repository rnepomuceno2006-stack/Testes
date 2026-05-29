# Viva Parintins!!! — Unity Setup

## 1. Requisitos
- Unity 2022.3 LTS (Long Term Support)
- Android Build Support instalado no Unity Hub
- JDK 17 (instale via Unity Hub → Installs → seu Unity → Add modules)

## 2. Abrir o projeto
1. Unity Hub → Add project → selecione a pasta `VivaParintins-Unity/`
2. Espere importar (primeira vez: 2–5 min)

## 3. Assets obrigatórios da Asset Store (GRATUITOS ou pagos)

### Visual Mario Galaxy
| Asset | Link | Preço |
|-------|------|-------|
| **Toony Colors Pro 2** (cel-shader) | Unity Asset Store | ~$60 |
| **Cartoon FX Remaster** (partículas) | Unity Asset Store | ~$45 |
| **Low Poly Tropical Pack** (ilhas) | Unity Asset Store | ~$25 |
| **Fantasy Skybox** (céu estrelado) | Unity Asset Store | Grátis |
| **Particle Ribbon** (rastros) | Unity Asset Store | Grátis |

### Personagens
| Asset | Uso |
|-------|-----|
| **Synty Studios — Polygon Fantasy** | Base dos personagens, customizados com cores dos bois |
| **ou: Cartoon Character Creator** | Personagens estilizados mais próximos do Mario Galaxy |
| **Mixamo** (mixamo.com) | Animações GRATUITAS para os personagens |

### Áudio
| Asset | Uso |
|-------|-----|
| **Toadas instrumentais 2026** | Contacte a Associação dos artistas do Festival de Parintins |
| **Casual Game Sounds** (Asset Store) | SFX de hit, miss, win |
| **Jungle/Amazon Ambience** | Som de fundo no mapa |

## 4. Configurar personagens com as imagens do Gemini

As imagens 2D geradas pelo Gemini podem ser usadas como:
- **Card artwork** nos cards da galeria (importe como Sprite, modo: Single)
- **Billboard sprites** no mundo 3D (personagem 2D com normal map = efeito 2.5D)
- **UI portrait** na intro e no HUD

### Como importar:
1. Arraste as imagens para `Assets/Sprites/Characters/`
2. Texture Type: **Sprite (2D and UI)**
3. Para uso 3D: crie um Quad, aplique um material com a sprite → Billboard Shader

### Para o look Mario Galaxy em 2D:
1. Import o shader **"Sprite Glow"** (Asset Store, grátis)
2. Aplique nas sprites dos personagens
3. Configure a cor de glow: vermelho para Garantido, azul para Caprichoso

## 5. Configurar Android
1. Edit → Project Settings → Player
2. **Company Name**: VivaParintins
3. **Product Name**: Viva Parintins!!!
4. **Package Name**: br.com.vivaparintins.game
5. **Minimum API Level**: Android 7.0 (API 24)
6. **Target API Level**: Android 14 (API 34)
7. Aba **Publishing Settings** → crie sua Keystore aqui

## 6. Build do APK
1. File → Build Settings → Android → Switch Platform
2. Marque **Development Build** para testar
3. Build → escolha onde salvar o .apk
4. Para Play Store: Build App Bundle (.aab) em vez de APK

## 7. Cenas necessárias (crie no Unity)
| Cena | Script principal |
|------|-----------------|
| `IntroScene` | `IntroController.cs` |
| `MapScene` | `PhaseMapController.cs` |
| `MiniGameScene` | `MiniGameLoader.cs` → detecta tipo e ativa o game certo |
| `GalleryScene` | (crie usando `CharacterData` ScriptableObjects) |

## 8. ScriptableObjects — dados dos 26 personagens
Cada personagem = 1 arquivo `.asset` em `Assets/Resources/Characters/`
Crie via: clique direito → Create → VivaParintins → Character Data

### Garantido (13):
- Boi Garantido, Pajé (Adriano Paketá), Cunhã-Poranga (Isabelle Nogueira)
- Rainha (Lívia Cristina), Sinhazinha (Raíra Lins), Porta-Estandarte (Jeveny Mendonça)
- Levantador (David Assayag), Apresentador (Israel Paulain), Amo (João Paulo Faria)
- Tuxaua, Mãe Catirina, Pai Francisco, Batucada

### Caprichoso (13):
- Boi Caprichoso, Pajé (Erick Beltrão), Cunhã-Poranga (Marcyele Albuquerque)
- Rainha (Cleise Simas), Sinhazinha (Valentina Cid), Porta-Estandarte (Marcela Marialva)
- Levantador (Patrick Araujo), Apresentador (Edmundo Oran), Amo do Caprichoso
- Tuxaua, Mãe Catirina, Pai Francisco, Marujada de Guerra
