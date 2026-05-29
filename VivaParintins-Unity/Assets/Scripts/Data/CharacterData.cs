using UnityEngine;

namespace VivaParintins.Data
{
    public enum StarRarity { One = 1, Two = 2, Three = 3 }

    [CreateAssetMenu(fileName = "CharacterData", menuName = "VivaParintins/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identidade")]
        public string characterId;
        public Team team;
        public string role;            // "Cunhã-Poranga", "Pajé", etc.
        public string cardName;        // nome do personagem no card
        public string realName;        // nome real do integrante 2026
        public StarRarity rarity;

        [Header("Visual")]
        public GameObject characterPrefab;   // modelo 3D (Asset Store / custom)
        public Sprite cardArtwork;           // arte 2D do card
        public Color glowColor;
        public Material cardHoloMaterial;    // material holográfico p/ lendários

        [Header("Stats")]
        [Range(0, 100)] public int forca;    // 🐂
        [Range(0, 100)] public int encanto;  // 🪶
        [Range(0, 100)] public int toada;    // 🥁
        [Range(0, 100)] public int paixao;   // ❤️/⭐

        [Header("Lore")]
        [TextArea(3, 6)]
        public string lore;

        [Header("Áudio")]
        public AudioClip voiceLine;    // frase icônica do personagem
    }
}
