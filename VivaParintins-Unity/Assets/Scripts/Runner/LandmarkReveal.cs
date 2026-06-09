using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using VivaParintins.Core;
using VivaParintins.Data;

namespace VivaParintins.Runner
{
    [System.Serializable]
    public struct LandmarkData
    {
        public string landmarkName;
        public string landmarkDescription;
        public Sprite landmarkSprite;
        public Sprite characterSprite;
        public string characterName;
        public string characterRole;
        public string characterQuote;
        public AudioClip fanfare;
    }

    public class LandmarkReveal : MonoBehaviour
    {
        [Header("Landmark Datasets")]
        public LandmarkData[] landmarksGarantido;
        public LandmarkData[] landmarksCaprichoso;

        [Header("UI References")]
        public GameObject revealPanel;
        public Image landmarkImage;
        public Image characterImage;
        public TextMeshProUGUI landmarkNameText;
        public TextMeshProUGUI landmarkDescText;
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI characterRoleText;
        public TextMeshProUGUI characterQuoteText;
        public RectTransform characterRect;
        public Button nextPhaseButton;
        public Button menuButton;

        [Header("Grande Final — Bumbódromo (os dois bois entram na arena)")]
        [Tooltip("Índice da fase do Bumbódromo (última). Dispara a apresentação especial dos dois bois.")]
        public int bumbodromoPhaseIndex = 5;
        public GameObject finalePanel;          // painel exclusivo do grande final
        public Image      boiGarantidoImage;    // sprite do Boi Garantido (vermelho)
        public Image      boiCaprichosoImage;   // sprite do Boi Caprichoso (azul)
        public RectTransform boiGarantidoRect;  // entra deslizando da esquerda
        public RectTransform boiCaprichosoRect; // entra deslizando da direita
        public Sprite     boiGarantidoSprite;
        public Sprite     boiCaprichosoSprite;
        public TextMeshProUGUI finaleTitleText; // "GRANDE FINAL — BUMBÓDROMO!"
        public TextMeshProUGUI finaleSubtitleText;
        public ParticleSystem confeteGarantido; // confete vermelho
        public ParticleSystem confeteCaprichoso;// confete azul
        public AudioClip      finaleFanfare;
        public Button         finaleNextButton;
        public Button         finaleMenuButton;

        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            if (revealPanel != null) revealPanel.SetActive(false);
        }

        void Start()
        {
            if (nextPhaseButton != null) nextPhaseButton.onClick.AddListener(OnNextPhase);
            if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
            if (finaleNextButton != null) finaleNextButton.onClick.AddListener(OnNextPhase);
            if (finaleMenuButton != null) finaleMenuButton.onClick.AddListener(OnMenu);
            if (finalePanel != null) finalePanel.SetActive(false);
        }

        public void Reveal(int phaseIndex)
        {
            // Grande final no Bumbódromo: os dois bois entram na arena
            if (phaseIndex == bumbodromoPhaseIndex && finalePanel != null)
            {
                RevealFinale();
                return;
            }

            if (revealPanel == null) return;

            bool isGarantido = GameManager.Instance != null && GameManager.Instance.selectedTeam == Team.Garantido;
            var dataset = isGarantido ? landmarksGarantido : landmarksCaprichoso;

            if (dataset == null || phaseIndex >= dataset.Length) return;

            LandmarkData data = dataset[phaseIndex];

            if (landmarkImage != null) landmarkImage.sprite = data.landmarkSprite;
            if (characterImage != null) characterImage.sprite = data.characterSprite;
            if (landmarkNameText != null) landmarkNameText.text = data.landmarkName;
            if (landmarkDescText != null) landmarkDescText.text = data.landmarkDescription;
            if (characterNameText != null) characterNameText.text = data.characterName;
            if (characterRoleText != null) characterRoleText.text = data.characterRole;
            if (characterQuoteText != null) characterQuoteText.text = $"\"{data.characterQuote}\"";

            if (data.fanfare != null) audioSource.PlayOneShot(data.fanfare);

            revealPanel.SetActive(true);
            StartCoroutine(AnimateReveal());
        }

        IEnumerator AnimateReveal()
        {
            if (landmarkImage != null)
            {
                landmarkImage.transform.localScale = Vector3.zero;
                float t = 0f;
                while (t < 0.4f)
                {
                    t += Time.unscaledDeltaTime;
                    float s = Mathf.SmoothStep(0f, 1f, t / 0.4f);
                    landmarkImage.transform.localScale = Vector3.one * s;
                    yield return null;
                }
                landmarkImage.transform.localScale = Vector3.one;
            }

            if (characterRect != null)
            {
                Vector2 target = characterRect.anchoredPosition;
                Vector2 offscreen = target + new Vector2(600f, 0f);
                characterRect.anchoredPosition = offscreen;

                float t = 0f;
                while (t < 0.35f)
                {
                    t += Time.unscaledDeltaTime;
                    characterRect.anchoredPosition = Vector2.Lerp(offscreen, target, Mathf.SmoothStep(0f, 1f, t / 0.35f));
                    yield return null;
                }
                characterRect.anchoredPosition = target;
            }
        }

        // ── GRANDE FINAL: BUMBÓDROMO COM OS DOIS BOIS ─────────────────────
        public void RevealFinale()
        {
            bool isGarantido = GameManager.Instance != null && GameManager.Instance.selectedTeam == Team.Garantido;

            if (boiGarantidoImage != null && boiGarantidoSprite != null)
                boiGarantidoImage.sprite = boiGarantidoSprite;
            if (boiCaprichosoImage != null && boiCaprichosoSprite != null)
                boiCaprichosoImage.sprite = boiCaprichosoSprite;

            if (finaleTitleText != null)
                finaleTitleText.text = "GRANDE FINAL — BUMBÓDROMO!";
            if (finaleSubtitleText != null)
                finaleSubtitleText.text = isGarantido
                    ? "O Garantido ❤️ e o Caprichoso ⭐ se enfrentam na arena! Viva Parintins!"
                    : "O Caprichoso ⭐ e o Garantido ❤️ se enfrentam na arena! Viva Parintins!";

            if (finaleFanfare != null) audioSource.PlayOneShot(finaleFanfare);

            finalePanel.SetActive(true);
            StartCoroutine(AnimateFinale());
        }

        IEnumerator AnimateFinale()
        {
            // Boi Garantido entra deslizando da esquerda
            if (boiGarantidoRect != null)
            {
                Vector2 target = boiGarantidoRect.anchoredPosition;
                Vector2 offscreen = target + new Vector2(-900f, 0f);
                boiGarantidoRect.anchoredPosition = offscreen;
                StartCoroutine(SlideRect(boiGarantidoRect, offscreen, target, 0.6f));
            }

            // Boi Caprichoso entra deslizando da direita (levemente atrasado)
            if (boiCaprichosoRect != null)
            {
                Vector2 target = boiCaprichosoRect.anchoredPosition;
                Vector2 offscreen = target + new Vector2(900f, 0f);
                boiCaprichosoRect.anchoredPosition = offscreen;
                yield return new WaitForSecondsRealtime(0.15f);
                StartCoroutine(SlideRect(boiCaprichosoRect, offscreen, target, 0.6f));
            }

            yield return new WaitForSecondsRealtime(0.6f);

            // Explosão de confete dos dois bois ao se encontrarem no centro
            confeteGarantido?.Play();
            confeteCaprichoso?.Play();
        }

        IEnumerator SlideRect(RectTransform rect, Vector2 from, Vector2 to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                rect.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / duration));
                yield return null;
            }
            rect.anchoredPosition = to;
        }

        void OnNextPhase()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.LoadNextPhase();
        }

        void OnMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
