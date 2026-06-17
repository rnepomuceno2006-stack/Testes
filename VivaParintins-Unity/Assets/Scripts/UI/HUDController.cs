// HUD de gameplay — spec §4.1
// Attach no Canvas da cena Runner.
// Referência: 1920×1080, Scale With Screen Size, Match 0.5

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;

namespace VivaParintins.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Vidas — topo-esquerda")]
        public Image[] heartIcons;          // 3 ícones de coração/estrela
        public Sprite heartSprite;
        public Sprite starSprite;

        [Header("Contador de colecionáveis — topo-centro")]
        public Image collectibleIcon;       // ícone 52px
        public TextMeshProUGUI collectibleText; // "12 / 24" Nunito 900 44px

        [Header("Barra de progresso — base-centro")]
        public Slider progressBar;
        public Image progressFill;          // gradiente #FFD23C → #F2901E
        public Image progressStart;         // medalhão estrela 60px
        public Image progressEnd;           // ícone landmark 60px

        [Header("Pausa — topo-direita")]
        public Button pauseButton;

        private Team _team;
        private int _maxLives;

        void Start()
        {
            _team = GameManager.Instance != null ? GameManager.Instance.selectedTeam : Team.Caprichoso;
            _maxLives = heartIcons.Length;
            ApplyTeamTheme();
        }

        void ApplyTeamTheme()
        {
            bool isGar = _team == Team.Garantido;
            Sprite icon = isGar ? heartSprite : starSprite;

            foreach (var h in heartIcons)
                if (h != null) h.sprite = icon;

            if (collectibleIcon != null)
                collectibleIcon.sprite = icon;
        }

        public void UpdateLives(int current)
        {
            for (int i = 0; i < heartIcons.Length; i++)
            {
                if (heartIcons[i] == null) continue;
                heartIcons[i].color = i < current ? Color.white : new Color(1, 1, 1, 0.25f);
            }
        }

        public void UpdateCollectibles(int gathered, int total)
        {
            if (collectibleText != null)
                collectibleText.text = $"{gathered} / {total}";
        }

        public void UpdateProgress(float ratio)
        {
            if (progressBar != null)
                progressBar.value = Mathf.Clamp01(ratio);
        }
    }
}
