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
        }

        public void Reveal(int phaseIndex)
        {
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
