// Modal de Pausa — spec §4.4
// CONTINUAR / REINICIAR FASE / MENU + toggles de som/música

using UnityEngine;
using UnityEngine.UI;
using VivaParintins.Runner;

namespace VivaParintins.UI
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Botões")]
        public Button continueButton;
        public Button restartButton;
        public Button menuButton;

        [Header("Toggles")]
        public Toggle soundToggle;
        public Toggle musicToggle;

        [Header("Overlay")]
        public Image overlayImage;

        void Awake()
        {
            gameObject.SetActive(false);
        }

        void Start()
        {
            if (continueButton != null) continueButton.onClick.AddListener(OnContinue);
            if (restartButton  != null) restartButton.onClick.AddListener(OnRestart);
            if (menuButton     != null) menuButton.onClick.AddListener(OnMenu);

            if (soundToggle != null)
            {
                soundToggle.isOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
                soundToggle.onValueChanged.AddListener(v =>
                {
                    PlayerPrefs.SetInt("SoundOn", v ? 1 : 0);
                    AudioListener.volume = v ? 1f : 0f;
                });
            }

            if (musicToggle != null)
            {
                musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
                musicToggle.onValueChanged.AddListener(v =>
                    PlayerPrefs.SetInt("MusicOn", v ? 1 : 0));
            }
        }

        public void Show()
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);
        }

        void OnContinue()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }

        void OnRestart()
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
