// Crie via: clique direito → Create → VivaParintins → Relic Data
// Um arquivo .asset por relíquia em Assets/Resources/Relics/

using UnityEngine;

namespace VivaParintins.Data
{
    public enum RelicBuff
    {
        Magnet,           // Microfone de Ouro — ímã de Miçangas
        Shield,           // Maracá Místico — absorve 1 colisão
        Dash,             // Berrante Poderoso — dash de invencibilidade 5s
        ScoreMultiplier,  // Estandarte Glorioso — score x2/x3
        Glide,            // Sombrinha Estrelada — pulo duplo / planar
        ArrowShot         // Arco e Flecha — destrói obstáculos à frente
    }

    [CreateAssetMenu(menuName = "VivaParintins/Relic Data")]
    public class RelicData : ScriptableObject
    {
        [Header("Identidade")]
        public string relicName;
        public string ownerCharacter;    // Ex: "David Assayag"
        public Team   faction;
        public Sprite icon;

        [Header("Buff")]
        public RelicBuff buffType;
        [Tooltip("Multiplicador do buff. Magnet: raio em unidades. Shield: nº de bloqueios. Dash: duração em s. Score: multiplicador.")]
        public float buffValue = 1f;

        [Header("Loja")]
        [Tooltip("Custo em Penas de Ouro (moeda premium).")]
        public int   costPenas = 50;
        [Tooltip("Custo de upgrade por nível (multiplicado pelo nível atual).")]
        public int   upgradeCostBase = 30;
        public int   maxLevel = 3;

        [Header("Lore")]
        [TextArea]
        public string description;
    }
}
