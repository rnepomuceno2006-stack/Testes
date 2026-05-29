using UnityEngine;

namespace VivaParintins.Data
{
    public enum Team { Garantido, Caprichoso }

    [CreateAssetMenu(fileName = "TeamData", menuName = "VivaParintins/Team Data")]
    public class TeamData : ScriptableObject
    {
        public Team team;
        public string teamName;
        public string theme2026;
        public Color primaryColor;
        public Color glowColor;
        public Sprite emblem;          // ❤️ ou ⭐ — importe da Asset Store
        public string tokenSymbol;     // "♥" ou "★"
        public AudioClip toadaTheme;   // música tema (instrumental)
        public int bpm;                // 130 GAR / 124 CAP
    }
}
