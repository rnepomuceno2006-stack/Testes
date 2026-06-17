// Marcadores de nó no mapa — spec §3.11
// Estados: Concluído | Atual | Desbloqueado | Travado | Boss

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VivaParintins.UI
{
    public enum NodeState { Locked, Unlocked, Current, Completed, Boss }

    public class MapNodeUI : MonoBehaviour
    {
        [Header("Referências")]
        public Image nodeImage;
        public TextMeshProUGUI numberText;      // Cinzel 900, gold-text
        public Image[] starImages;              // até 3 estrelas abaixo do nó
        public Sprite starFilled;
        public Sprite starEmpty;
        public GameObject lockIcon;
        public GameObject playIcon;             // ▶
        public TextMeshProUGUI playLabel;       // "JOGAR ▸"

        [Header("Sprites por estado")]
        public Sprite completedSprite;          // medalhão dourado 112px
        public Sprite currentSprite;            // cristal 148px
        public Sprite unlockedSprite;           // círculo prata 108px
        public Sprite lockedSprite;             // círculo escuro
        public Sprite bossSprite;               // vermelho escuro

        [Header("Cores")]
        public Color completedColor = UITheme.GoldBase;
        public Color currentColor   = Color.white;
        public Color unlockedColor  = UITheme.Hex("#9FB0C8");
        public Color lockedColor    = new Color(0.08f, 0.11f, 0.17f, 0.8f);

        public void SetState(NodeState state, int phaseNumber, int stars = 0)
        {
            if (lockIcon  != null) lockIcon.SetActive(false);
            if (playIcon  != null) playIcon.SetActive(false);
            if (playLabel != null) playLabel.gameObject.SetActive(false);

            switch (state)
            {
                case NodeState.Completed:
                    Apply(completedSprite, completedColor, phaseNumber.ToString());
                    SetStars(stars);
                    break;

                case NodeState.Current:
                    Apply(currentSprite, currentColor, "");
                    if (playIcon  != null) playIcon.SetActive(true);
                    if (playLabel != null) { playLabel.gameObject.SetActive(true); playLabel.text = "JOGAR ▸"; }
                    var bob = GetComponent<UIAnimations>();
                    if (bob == null) bob = gameObject.AddComponent<UIAnimations>();
                    bob.animType = UIAnimations.AnimType.Bob;
                    bob.bobAmplitude = 6f;
                    break;

                case NodeState.Unlocked:
                    Apply(unlockedSprite, unlockedColor, phaseNumber.ToString());
                    break;

                case NodeState.Locked:
                    Apply(lockedSprite, lockedColor, "");
                    if (lockIcon != null) lockIcon.SetActive(true);
                    break;

                case NodeState.Boss:
                    Apply(bossSprite, UITheme.Gar100, "");
                    if (lockIcon != null) lockIcon.SetActive(true);
                    break;
            }
        }

        void Apply(Sprite sprite, Color color, string number)
        {
            if (nodeImage   != null) { nodeImage.sprite = sprite; nodeImage.color = color; }
            if (numberText  != null) numberText.text = number;
        }

        void SetStars(int count)
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;
                starImages[i].gameObject.SetActive(true);
                starImages[i].sprite = i < count ? starFilled : starEmpty;
            }
        }
    }
}
