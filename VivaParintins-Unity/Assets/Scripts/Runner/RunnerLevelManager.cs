using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using VivaParintins.Core;
using VivaParintins.Data;

namespace VivaParintins.Runner
{
    public class RunnerLevelManager : MonoBehaviour
    {
        public static RunnerLevelManager Instance { get; private set; }

        [Header("Level State")]
        public int collectiblesGathered;
        public int totalCollectibles;
        public float timeElapsed;
        public bool levelComplete;
        public int currentPhaseIndex;

        [Header("Lives")]
        public int maxLives = 3;
        private int currentLives;

        [Header("HUD References")]
        public TextMeshProUGUI collectiblesText;
        public TextMeshProUGUI livesText;
        public Slider progressBar;
        public Image cameraFlashImage;

        [Header("Panels")]
        public GameObject gameOverPanel;
        public GameObject levelCompletePanel;

        [Header("VFX / SFX")]
        public GameObject collectVFXPrefab;
        public AudioClip collectSFX;
        public AudioClip hitSFX;
        public AudioClip gameOverSFX;

        [Header("References")]
        public LandmarkReveal landmarkReveal;
        public RunnerCameraController cameraController;

        private AudioSource audioSource;
        private bool isGarantido;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            currentLives = maxLives;
            levelComplete = false;
            collectiblesGathered = 0;
            timeElapsed = 0f;

            if (GameManager.Instance != null)
            {
                currentPhaseIndex = GameManager.Instance.currentPhaseIndex;
                isGarantido = GameManager.Instance.selectedTeam == Team.Garantido;
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

            UpdateHUD();
        }

        void Update()
        {
            if (!levelComplete)
            {
                timeElapsed += Time.deltaTime;
                UpdateProgressBar();
            }
        }

        public void OnCollect(Vector3 position)
        {
            collectiblesGathered++;
            PlaySFX(collectSFX);
            SpawnCollectVFX(position);
            UpdateHUD();
        }

        public void OnPlayerHit()
        {
            if (levelComplete) return;
            currentLives--;
            PlaySFX(hitSFX);
            UpdateHUD();
            StartCoroutine(CameraFlashRed());
            StartCoroutine(CameraShakeOnHit());

            if (currentLives <= 0)
                GameOver();
        }

        public void GameOver()
        {
            Time.timeScale = 0f;
            PlaySFX(gameOverSFX);
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }

        public void LevelComplete()
        {
            if (levelComplete) return;
            levelComplete = true;

            int stars = CalculateStars();
            GameManager.Instance?.SavePhaseResult(currentPhaseIndex, stars);

            if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
            if (landmarkReveal != null) landmarkReveal.Reveal(currentPhaseIndex);
            if (cameraController != null) cameraController.ZoomOutForLandmark();
        }

        public void RetryLevel()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.LoadPhaseRunner(currentPhaseIndex);
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        int CalculateStars()
        {
            if (totalCollectibles == 0) return 3;
            float ratio = (float)collectiblesGathered / totalCollectibles;
            if (ratio >= 0.8f) return 3;
            if (ratio >= 0.4f) return 2;
            return 1;
        }

        void UpdateHUD()
        {
            if (collectiblesText != null)
            {
                string icon = isGarantido ? "❤️" : "⭐";
                collectiblesText.text = $"{icon} {collectiblesGathered}/{totalCollectibles}";
            }
            if (livesText != null)
                livesText.text = $"♥ {currentLives}";
        }

        void UpdateProgressBar()
        {
            if (progressBar == null) return;
            var generator = ProceduralLevelGenerator.Instance;
            if (generator == null) return;
            float playerX = cameraController != null && cameraController.Player != null
                ? cameraController.Player.position.x
                : transform.position.x;
            progressBar.value = Mathf.Clamp01(playerX / generator.LevelLength);
        }

        void SpawnCollectVFX(Vector3 pos)
        {
            if (collectVFXPrefab == null) return;
            var vfx = Instantiate(collectVFXPrefab, pos, Quaternion.identity);
            Destroy(vfx, 1.5f);
        }

        void PlaySFX(AudioClip clip)
        {
            if (clip == null || audioSource == null) return;
            audioSource.PlayOneShot(clip);
        }

        IEnumerator CameraFlashRed()
        {
            if (cameraFlashImage == null) yield break;
            cameraFlashImage.color = new Color(1f, 0f, 0f, 0.5f);
            float t = 0f;
            while (t < 0.3f)
            {
                t += Time.unscaledDeltaTime;
                cameraFlashImage.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.5f, 0f, t / 0.3f));
                yield return null;
            }
            cameraFlashImage.color = Color.clear;
        }

        IEnumerator CameraShakeOnHit()
        {
            if (cameraController == null) yield break;
            cameraController.Shake(0.3f, 0.15f);
            yield return null;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
