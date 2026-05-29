using UnityEngine;

namespace VivaParintins.Data
{
    public enum MiniGameType { Ritmo, Quiz, Contrario, Versos }
    public enum FestivalNight { Noite1 = 1, Noite2 = 2, Noite3 = 3 }

    [CreateAssetMenu(fileName = "PhaseData", menuName = "VivaParintins/Phase Data")]
    public class PhaseData : ScriptableObject
    {
        [Header("Identificação")]
        public string phaseId;
        public string phaseName;           // "Porto dos Caboclos"
        public string quesito;             // "Pai Francisco"
        public FestivalNight night;
        public MiniGameType gameType;

        [Header("Posição no Mapa")]
        public Vector3 mapPosition;        // posição da bolha no mapa 3D
        public float nodeScale = 1f;

        [Header("Visual")]
        public GameObject nodeEnvironment; // ambiente 3D da fase (ilha temática)
        public Sprite nodeIcon;            // ícone sobre a bolha
        public Color nightColor;           // cor tema da noite

        [Header("Descrição")]
        [TextArea(2, 4)]
        public string description;

        [Header("Recompensas")]
        public int starsRequired = 1;      // estrelas mínimas para desbloquear próxima
        public int lumaReward = 10;
        public CharacterData unlockCharacter; // personagem desbloqueado ao completar

        [Header("Dificuldade")]
        [Range(1, 5)] public int difficulty = 2;
    }
}
