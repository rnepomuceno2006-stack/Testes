// NÃO COLOQUE NA CENA — referência visual e de posicionamento para o estilo
// ToonScapes Spring Isles + Zelda Breath of the Wild cel-shaded.
//
// ══════════════════════════════════════════════════════════════════════
// DIREÇÃO DE ARTE: VIVA PARINTINS!!! — ESTILO VISUAL
// ══════════════════════════════════════════════════════════════════════
//
// ASSET PRINCIPAL:
//   ToonScapes: Spring Isles (Unity Asset Store)
//   https://assetstore.unity.com/packages/3d/environments/fantasy/toonscapes-spring-isles-361638
//   Use os seguintes prefabs do pack:
//     - FloatingIsland_Large   → base de cada localização de Parintins
//     - FloatingIsland_Medium  → ilhas decorativas entre fases
//     - Waterfall_Animated     → quedas d'água nas ilhas
//     - ToonTree_Tropical      → palmeiras e árvores amazônicas
//     - RockFormation_*        → rochas nas bordas das ilhas
//     - GrassPlane_*           → gramado vibrante
//
// ══════════════════════════════════════════════════════════════════════
// SHADER — CEL SHADING ZELDA STYLE (URP)
// ══════════════════════════════════════════════════════════════════════
//
// 1. Pipeline: Universal Render Pipeline (URP) 14.x
// 2. Shader dos personagens billboard: "Universal Render Pipeline/2D/Sprite-Lit-Default"
//    + Outline pass (Custom URP Feature) espessura 2px, cor preta
// 3. Shader dos ambientes 3D: Shader Graph cel-shading
//    - Ramp texture: 2 tons (shadow + highlight), sem gradiente suave
//    - Rim light: cor do time do jogador (vermelho GAR / azul CAP)
//    - Outline: Post-process Edge Detection (Sobel filter no depth buffer)
// 4. Post-processing (Volume Global):
//    - Bloom: Threshold 0.9, Intensity 0.4 (brilho Mario Galaxy nas estrelas/Lumas)
//    - Color Adjustments: Saturation +25 (cores vibrantes do festival)
//    - Tonemapping: ACES
//
// ══════════════════════════════════════════════════════════════════════
// LAYOUT DAS 8 ILHAS FLUTUANTES — MapScene
// ══════════════════════════════════════════════════════════════════════
//
// Câmera:
//   Mode: Perspective (não ortográfica — dá profundidade 3D igual Zelda)
//   FOV: 45
//   Position: (0, 22, -18)
//   Rotation: (28, 0, 0)
//   Background: Skybox estrelado azul-escuro (asset Fantasy Skybox FREE)
//
// As ilhas ficam em alturas VARIADAS para dar profundidade (estilo BotW):
//
//   [0] PORTO DE PARINTINS        pos: (-10,  0, -8)  altura Y=0  ← nível do rio
//       ToonScapes: FloatingIsland_Large
//       Elementos: guindastes, barcos no rio abaixo, caixas de carga
//       Bandeira: "PORTO DE PARINTINS" em placa 3D
//       Conexão: ponte de corda/madeira para ilha [1]
//
//   [1] FEIRA DE ARTESANATOS      pos: ( -5,  2, -5)  altura Y=2
//       ToonScapes: FloatingIsland_Medium
//       Elementos: barracas coloridas com toldos, cerâmicas, penas decorativas
//       Detalhe: varal com tecidos entre as barracas
//
//   [2] MERCADO MUNICIPAL         pos: (  2,  1, -6)  altura Y=1
//       ToonScapes: FloatingIsland_Large
//       Elementos: caixotes de frutas regionais, açaí, guaraná, jambú
//       Detalhe: vendedores 2D billboard (sprites simples)
//
//   [3] ORLA DE PARINTINS         pos: ( -8,  3,  0)  altura Y=3
//       ToonScapes: FloatingIsland_Large (com WaterFall saindo da borda)
//       Elementos: calçadão, banco de praça, Rio Amazonas embaixo
//       Detalhe: barcos passando no rio (prefabs animados)
//
//   [4] COMUNAS BAR               pos: (  4,  4,  0)  altura Y=4
//       ToonScapes: FloatingIsland_Medium
//       Elementos: bar com luzes neon, mesas, torcedores 2D billboard
//       Detalhe: iluminação noturna (Point Lights coloridos)
//
//   [5] CURRAL DO BOI             pos: ( -3,  5,  5)  altura Y=5
//       ToonScapes: FloatingIsland_Large
//       Elementos: arena circular, bandeiras vermelhas/azuis, boi 3D
//       Detalhe: fumaça/névoa mística ao redor (Particle System)
//
//   [6] PRAÇA DOS DOIS BOIS       pos: (  3,  6,  5)  altura Y=6
//       ToonScapes: FloatingIsland_Large
//       Elementos: 2 estátuas dos bois (Garantido vermelho | Caprichoso azul)
//         frente a frente no centro da ilha, flores entre elas
//       Detalhe: partículas de corações e estrelas flutuando
//
//   [7] BUMBÓDROMO (BOSS)         pos: (  0,  9, 10)  altura Y=9  ← topo
//       ToonScapes: FloatingIsland_Large × 2 (fundidas)
//       Elementos: mini-estádio oval, arquibancadas vermelhas e azuis
//         luzes de holofote, confete permanente (ParticleSystem)
//       Detalhe: aura épica pulsante, raio de luz do céu apontando para ele
//       Portal de acesso: arco dourado com estrelas orbitando
//
// ══════════════════════════════════════════════════════════════════════
// CONEXÕES ENTRE ILHAS (Caminhos visuais)
// ══════════════════════════════════════════════════════════════════════
//
//   Fase concluída → ponte de pedra sólida (asset ToonScapes StonePathBridge)
//   Fase bloqueada → ponte de corda quebrada + cadeado 3D flutuante
//   Rota principal: [0]→[1]→[2]→[3]→[4]→[5]→[6]→[7]
//   As ilhas têm alturas diferentes — as pontes sobem levemente (curvas)
//
// ══════════════════════════════════════════════════════════════════════
// ILUMINAÇÃO GERAL
// ══════════════════════════════════════════════════════════════════════
//
//   Sun (Directional):
//     Color: #FFF0C8 (luz quente amazônica, fim de tarde)
//     Intensity: 1.1
//     Rotation: (45, -30, 0)
//
//   Ambient (Lighting Settings):
//     Mode: Gradient
//     Sky Color:    #1A3A6B (azul noite festival)
//     Horizon:      #FF6B2B (laranja pôr do sol Parintins)
//     Ground Color: #2D4A1E (verde amazônico)
//
//   Fog:
//     Mode: Linear
//     Color: #8EC5FC (névoa azul suave)
//     Start: 30   End: 80
//     (dá profundidade e esconde os limites do mapa)
//
// ══════════════════════════════════════════════════════════════════════
// ANIMAÇÕES AMBIENTES
// ══════════════════════════════════════════════════════════════════════
//
//   1. Todas as ilhas têm IsometricPhaseNode.IdleFloatRoutine() — sobem/descem levemente
//   2. Lumas (Mario Galaxy sprites 2D) orbitam as ilhas completadas
//   3. Pássaros 2D billboard cruzam o céu aleatoriamente (simples sprite animado)
//   4. Nuvens baixas passam sob as ilhas superiores (plane com shader de cloud scrolling)
//   5. Cachoeiras nas bordas das ilhas: ToonScapes WaterFall_Animated

namespace VivaParintins.Map
{
    // Apenas documentação — nenhum código executável aqui
}
