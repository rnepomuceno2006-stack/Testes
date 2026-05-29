// Coloque na Hierarquia: GameObject "PhaseRunner" em cada cena de fase.
// Controla a lógica de HUD, timer, spawn de obstáculos/itens e condição de vitória.
// Referencia o PhaseList ScriptableObject para ler a configuração da fase ativa.

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VivaParintins.Core;
using VivaParintins.Player;
using VivaParintins.Share;

namespace VivaParintins.Phases
{
    public class PhaseRunner : MonoBehaviour
    {
        [Header("Configuração")]
        public PhaseList phaseList;

        [Header("HUD")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI miçangasText;
        public Slider          progressBar;
        public GameObject      buffBanner;   // "BUFF DA TOADA ATIVO!"
        public GameObject      windIndicator;

        [Header("Resultado")]
        public GameObject resultPanel;
        public TextMeshProUGUI resultTitle;
        public TextMeshProUGUI starsText;
        public Button shareButton;
        public Button nextButton;
        public Button retryButton;

        [Header("Spawners")]
        public ObstacleSpawner obstacleSpawner;
        public CoinSpawner     coinSpawner;
        public ItemSpawner     itemSpawner;

        // ── Estado ───────────────────────────────────────────────────────
        PhaseConfig _cfg;
        float _timeLeft;
        int   _score;
        bool  _running;
        int   _phaseIndex;

        void Start()
        {
            _phaseIndex = GameManager.Instance?.currentPhaseIndex ?? 0;
            _cfg = phaseList.phases[Mathf.Clamp(_phaseIndex, 0, phaseList.phases.Length - 1)];

            _timeLeft = _cfg.timeLimitSeconds;
            _running  = true;

            // Velocidade base do runner
            var player = FindObjectOfType<PlayerMobileController>();
            if (player) player.forwardSpeed = _cfg.runnerSpeed;

            // Referência para o ímã
            GameEconomyManager.Instance.activePlayer = player;

            // Modificadores de fase
            if (_cfg.hasWindPhysics) StartCoroutine(WindEffect(player));
            if (_cfg.hasToadaBuff)   ActivateToadaBuff(player);
            if (_cfg.hasBeatSync)    obstacleSpawner?.EnableBeatSync();

            obstacleSpawner?.StartSpawning(_cfg);
            coinSpawner?.StartSpawning(_cfg);
            itemSpawner?.StartSpawning(_cfg);

            windIndicator?.SetActive(_cfg.hasWindPhysics);

            shareButton?.onClick.AddListener(OnShare);
            nextButton?.onClick.AddListener(OnNext);
            retryButton?.onClick.AddListener(OnRetry);
        }

        void Update()
        {
            if (!_running) return;

            _timeLeft -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(Mathf.Max(0, _timeLeft)).ToString("D2");
            scoreText.text = _score.ToString("N0");
            miçangasText.text = GameEconomyManager.Instance.Micangas.ToString();

            float pct = 1f - (_timeLeft / _cfg.timeLimitSeconds);
            progressBar.value = pct;

            if (_timeLeft <= 0) EndPhase();
        }

        // ── Pontuação (chamada pelos spawners/colisões) ───────────────────
        public void AddScore(int points)
        {
            _score += points;
        }

        // ── Modificadores especiais ──────────────────────────────────────
        IEnumerator WindEffect(PlayerMobileController player)
        {
            while (_running)
            {
                yield return new WaitForSeconds(Random.Range(3f, 6f));
                if (!_running) break;
                float windForce = Random.Range(-1, 2) * 1; // -1, 0 ou +1 pista
                if (player != null)
                    player.SendMessage("ChangeLane", (int)windForce, SendMessageOptions.DontRequireReceiver);
                ToastManager.Show("💨 Vento forte!");
            }
        }

        void ActivateToadaBuff(PlayerMobileController player)
        {
            if (player) player.forwardSpeed *= 1.25f;
            buffBanner?.SetActive(true);
            AudioManager.Instance?.PlayTeamTheme(GameManager.Instance?.CurrentTeamData);
            ToastManager.Show("🎵 BUFF DA TOADA! +25% velocidade!");
        }

        // ── Fim de fase ──────────────────────────────────────────────────
        void EndPhase()
        {
            _running = false;
            obstacleSpawner?.StopSpawning();
            coinSpawner?.StopSpawning();

            int stars = CalculateStars();
            GameManager.Instance?.SavePhaseResult(_phaseIndex, stars);

            resultPanel.SetActive(true);
            bool cleared = stars > 0;
            resultTitle.text = cleared
                ? $"🎉 {_cfg.displayName} COMPLETO!"
                : $"😤 Tente novamente!";
            starsText.text = new string('⭐', stars) + new string('☆', 3 - stars);

            bool isFinal = _cfg.isBossPhase;
            nextButton.gameObject.SetActive(cleared && !isFinal);
            shareButton.gameObject.SetActive(isFinal && cleared);
            retryButton.gameObject.SetActive(!cleared);

            VFX.VFXManager.Instance?.VictoryRain(GameManager.Instance.selectedTeam);
            if (cleared) AudioManager.Instance?.PlayWin();
        }

        int CalculateStars()
        {
            float ratio = (float)GameEconomyManager.Instance.Micangas / _cfg.coinGoal;
            if (ratio >= 1.0f) return 3;
            if (ratio >= 0.6f) return 2;
            if (ratio >= 0.3f) return 1;
            return 0;
        }

        // ── Botões ───────────────────────────────────────────────────────
        void OnShare()
        {
            ViralShareManager.Instance?.Share(_score, CalculateStars(), CalculateStars() > 0);
        }

        void OnNext()
        {
            GameManager.Instance?.LoadNextPhase();
        }

        void OnRetry()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        // Chamado pelo GameManager quando o jogador morre sem escudo
        public void OnPlayerDied()
        {
            if (!GameEconomyManager.Instance.TryUseGuarana())
                EndPhase();
        }
    }
}
