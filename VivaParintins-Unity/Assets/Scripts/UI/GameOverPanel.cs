// Modal de Game Over — spec §4.4
// "Acabou a folia!" com 3 corações vazios + % da fase

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using VivaParintins.Core;
using VivaParintins.Runner;

namespace VivaParintins.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [Header("Textos")]
        public TextMeshProUGUI titleText;       // "Acabou a folia!"
        public TextMeshProUGUI percentText;     // "42% da fase concluída"

        [Header("Corações vazios (rating)")]
        public Image[] emptyHearts;            // 3 corações apagados

        [Header("Botões")]
        public Button retryButton;             // TENTAR DE NOVO — laranja
        public Button continueButton;          // CONTINUAR ▸ (revive)
        public Button menuButton;              // MENU

        [Header("Overlay")]
        public Image overlayImage;             // escurecer + blur a fase

        void Awake()
        {
            gameObject.SetActive(false);
        }

        void Start()
        {
            if (retryButton   != null) retryButton.onClick.AddListener(OnRetry);
            if (continueButton!= null) continueButton.onClick.AddListener(OnContinue);
            if (menuButton    != null) menuButton.onClick.AddListener(OnMenu);
        }

        public void Show(float completionRatio)
        {
            gameObject.SetActive(true);

            if (titleText != null)
                titleText.text = "Acabou a folia!";

            if (percentText != null)
                percentText.text = $"{Mathf.RoundToInt(completionRatio * 100f)}% da fase concluída";

            // Corações vazios — todos apagados no game over
            foreach (var h in emptyHearts)
                if (h != null) h.color = new Color(1, 1, 1, 0.3f);

            StartCoroutine(AnimateIn());
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
                    overlayImage.color = new Color(0, 0, 0, Mathf.Lerp(0f, 0.7f, t / 0.4f));
                    yield return null;
                }
            }

            // Anima o painel de baixo pra cima
            var rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                Vector2 target = rect.anchoredPosition;
                rect.anchoredPosition = target + new Vector2(0, -200f);
                float t = 0f;
                while (t < 0.35f)
                {
                    t += Time.unscaledDeltaTime;
                    rect.anchoredPosition = Vector2.Lerp(
                        target + new Vector2(0, -200f),
                        target,
                        Mathf.SmoothStep(0f, 1f, t / 0.35f));
                    yield return null;
                }
                rect.anchoredPosition = target;
            }
        }

        void OnRetry()
        {
            Time.timeScale = 1f;
            RunnerLevelManager.Instance?.RetryLevel();
        }

        void OnContinue()
        {
            // Revive: restaura 1 vida e fecha o painel
            Time.timeScale = 1f;
            gameObject.SetActive(false);
            // RunnerLevelManager.Instance?.Revive();  // implementar se quiser revive pago
        }

        void OnMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
