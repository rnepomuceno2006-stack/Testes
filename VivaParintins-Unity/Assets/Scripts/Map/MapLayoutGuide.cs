// NÃO COLOQUE NA CENA — apenas referência de posicionamento isométrico.
// Use como guia ao posicionar os 8 buildings no Unity Editor.
//
// LAYOUT ISOMÉTRICO — visão top-down 45°, escala: 1 unidade = ~1 quarteirão
//
//   PARINTINS — ROTA DO TORCEDOR (sul→norte, rio à esquerda)
//
//   [0] Porto de Parintins        pos: (-8, 0, -6)   — canto SW, guindastes, barcos
//   [1] Feira de Artesanatos      pos: (-4, 0, -4)   — barracas coloridas
//   [2] Mercado Municipal         pos: ( 0, 0, -4)   — prédio central verde
//   [3] Orla de Parintins         pos: (-6, 0,  0)   — beira do rio, píer
//   [4] Comunas Bar               pos: ( 2, 0,  0)   — bar noturno, luzes
//   [5] Curral do Boi             pos: (-2, 0,  4)   — arena com bandeiras
//   [6] Praça dos Dois Bois       pos: ( 2, 0,  4)   — praça central, 2 estátuas
//   [7] Bumbódromo (BOSS)         pos: ( 0, 0,  8)   — grande estádio, centro-norte
//
// CÂMERA ISOMÉTRICA:
//   Transform: position (0, 14, -8), rotation (30, 45, 0)
//   Projection: Orthographic, Size: 8
//   Background: skybox estrelado (Mario Galaxy)
//
// ESTRADAS/CONEXÕES:
//   Crie planos com texture de calçada/asfalto entre os nós.
//   Adicione árvores tropicais, flores, cristais Mario Galaxy nas calçadas.
//
// EFEITOS VISUAIS:
//   - Bumbódromo: ParticleSystem com confete vermelho/azul permanente
//   - Rio Amazonas: plane animado com shader de água à esquerda do mapa
//   - Barcos: animação de balançar sobre a água no Porto
//   - Nós bloqueados: shader de dessaturação + cadeado 2D billboard
//   - Nós completados: halo de estrelas orbitando (Mario Galaxy Lumas)
//
// ILUMINAÇÃO:
//   Directional Light: color warm (#FFD580), intensity 1.2, angle 45°
//   Ambient: gradient sky, horizon cor do pôr do sol amazônico

namespace VivaParintins.Map
{
    // Apenas documentação — nenhum código executável aqui
}
