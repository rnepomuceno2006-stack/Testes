// Modal de Vitória — spec §4.4
// Título dourado + 3 estrelas animadas + PRÓXIMA FASE / REPETIR / MENU

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using VivaParintins.Core;
using VivaParintins.Runner;

namespace VivaParintins.UI
{
    public class VictoryPanel : MonoBehaviour
    {
        [Header("Título")]
        public TextMeshProUGUI titleText;       // "FASE CONCLUÍDA!" — Cinzel 900 dourado

        [Header("Estrelas de rating (3)")]
        public Image[] starImages;              // estrela do meio fica maior
        public Sprite starFilledSprite;
        public Sprite starEmptySprite;

        [Header("Resumo")]
        public TextMeshProUGUI collectiblesText;
        public TextMeshProUGUI timeText;

        [Header("Botões")]
        public Button nextPhaseButton;          // PRÓXIMA FASE — pílula dourada
        public Button retryButton;              // REPETIR
        public Button menuButton;               // MENU

        [Header("Overlay")]
        public Image overlayImage;

        void Awake()
        {
            gameObject.SetActive(false);
        }

        void Start()
        {
            if (nextPhaseButton != null) nextPhaseButton.onClick.AddListener(OnNextPhase);
            if (retryButton     != null) retryButton.onClick.AddListener(OnRetry);
            if (menuButton      != null) menuButton.onClick.AddListener(OnMenu);
        }

        public void Show(int stars, int gathered, int total, float timeElapsed)
        {
            gameObject.SetActive(true);

            if (titleText != null) titleText.text = "FASE CONCLUÍDA!";

            if (collectiblesText != null)
                collectiblesText.text = $"{gathered} / {total} coletados";

            if (timeText != null)
            {
                int m = Mathf.FloorToInt(timeElapsed / 60f);
                int s = Mathf.FloorToInt(timeElapsed % 60f);
                timeText.text = $"{m:00}:{s:00}";
            }

            StartCoroutine(AnimateStars(stars));
            StartCoroutine(AnimateIn());
        }

        IEnumerator AnimateStars(int stars)
        {
            yield return new WaitForSecondsRealtime(0.4f);

            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;

                starImages[i].sprite = i < stars ? starFilledSprite : starEmptySprite;

                // Estrela do meio (índice 1) é maior — spec: "a do meio maior"
                float scale = (i == 1) ? 1.3f : 1f;

                if (i < stars)
                {
                    // Anima entrada com glow
                    starImages[i].transform.localScale = Vector3.zero;
                    float t = 0f;
                    while (t < 0.25f)
                    {
                        t += Time.unscaledDeltaTime;
                        starImages[i].transform.localScale = Vector3.one * scale *
                            Mathf.SmoothStep(0f, 1f, t / 0.25f);
                        yield return null;
                    }
                    starImages[i].transform.localScale = Vector3.one * scale;
                    yield return new WaitForSecondsRealtime(0.1f);
                }
                else
                {
                    starImages[i].transform.localScale = Vector3.one * scale;
                }
            }
        }

        IEnumerator AnimateIn()
        {
            if (overlayImage != null)
            {
                overlayImage.color = new Color(0, 0, 0, 0);
                float t = 0f;
                while (t < 0.4f)
                {
                    t += Time.unscaledDeltaTime;
                    overlayImage.color = new Color(0, 0, 0, Mathf.Lerp(0f, 0.65f, t / 0.4f));
                    yield return null;
                }
            }
        }

        void OnNextPhase()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.LoadNextPhase();
        }

        void OnRetry()
        {
            Time.timeScale = 1f;
            RunnerLevelManager.Instance?.RetryLevel();
        }

        void OnMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
