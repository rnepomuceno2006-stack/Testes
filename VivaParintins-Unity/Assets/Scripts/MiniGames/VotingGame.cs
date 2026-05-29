using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;

namespace VivaParintins.MiniGames
{
    /// <summary>
    /// Tela de Votação ao Vivo — Noite 3 Grande Final.
    /// Mecânica de cabo de guerra: jogador toca no botão do seu boi
    /// contra votos de CPU simulados. Visual épico com timer regressivo.
    /// </summary>
    public class VotingGame : MonoBehaviour
    {
        [Header("UI")]
        public Slider tugSlider;
        public TextMeshProUGUI garScoreText;
        public TextMeshProUGUI capScoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI liveBadgeText;
        public Button garButton;
        public Button capButton;
        public TextMeshProUGUI garVotesText;
        public TextMeshProUGUI capVotesText;
        public GameObject resultPanel;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI rankText;

        [Header("VFX")]
        public ParticleSystem garParticles;
        public ParticleSystem capParticles;
        public Animator cameraAnim;

        [Header("Config")]
        public float gameDuration = 20f;
        public float cpuVoteInterval = 0.35f;

        // ── Estado ───────────────────────────────────────────────────────
        float _garVotes, _capVotes;
        float _timer;
        bool _running;
        float _cpuTimer;

        void Start()
        {
            _garVotes = GameManager.Instance.collectiveGar;
            _capVotes = GameManager.Instance.collectiveCap;
            _timer = gameDuration;
            _running = true;

            garButton.onClick.AddListener(OnGarVote);
            capButton.onClick.AddListener(OnCapVote);

            // música épica de final
            AudioManager.Instance?.PlayTeamTheme(GameManager.Instance.CurrentTeamData);

            StartCoroutine(LiveBadgePulse());
        }

        void Update()
        {
            if (!_running) return;

            // CPU vota a cada intervalo
            _cpuTimer += Time.deltaTime;
            if (_cpuTimer >= cpuVoteInterval)
            {
                _cpuTimer = 0;
                float bias = GameManager.Instance.selectedTeam == Team.Garantido ? 0.52f : 0.48f;
                if (Random.value < bias) _garVotes += Random.Range(100, 800);
                else                     _capVotes += Random.Range(100, 800);
            }

            // timer
            _timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(Mathf.Max(0, _timer)).ToString();

            RefreshUI();

            if (_timer <= 0) EndVoting();
        }

        // ── Votos do jogador ─────────────────────────────────────────────
        void OnGarVote()
        {
            if (!_running) return;
            _garVotes += Random.Range(500, 1500);
            garParticles?.Play();
            AudioManager.Instance?.PlayTap();
        }

        void OnCapVote()
        {
            if (!_running) return;
            _capVotes += Random.Range(500, 1500);
            capParticles?.Play();
            AudioManager.Instance?.PlayTap();
        }

        // ── UI ───────────────────────────────────────────────────────────
        void RefreshUI()
        {
            float tot = _garVotes + _capVotes;
            float garPct = tot > 0 ? _garVotes / tot : 0.5f;
            tugSlider.value = garPct;
            garScoreText.text = FormatVotes(_garVotes);
            capScoreText.text = FormatVotes(_capVotes);
            garVotesText.text = $"{garPct * 100:F1}%";
            capVotesText.text = $"{(1 - garPct) * 100:F1}%";
        }

        string FormatVotes(float v) => v >= 1000 ? $"{v / 1000:F0}k" : ((int)v).ToString();

        // ── Fim ──────────────────────────────────────────────────────────
        void EndVoting()
        {
            _running = false;
            garButton.interactable = false;
            capButton.interactable = false;

            float tot = _garVotes + _capVotes;
            bool garWins = _garVotes > _capVotes;
            bool playerWins = (GameManager.Instance.selectedTeam == Team.Garantido) == garWins;

            GameManager.Instance.collectiveGar = (int)_garVotes;
            GameManager.Instance.collectiveCap = (int)_capVotes;

            resultPanel.SetActive(true);
            resultText.text = playerWins
                ? $"🏆 {GameManager.Instance.CurrentTeamData.teamName} VENCEU!\nParabéns, a galera do seu boi dominou o Bumbódromo!"
                : $"😤 O contrário venceu dessa vez...\nMas seu boi vai voltar mais forte no ano que vem!";
            rankText.text = playerWins
                ? "⭐ Você estava do lado certo!"
                : "💪 A disputa foi acirrada!";

            VFX.VFXManager.Instance?.VictoryRain(GameManager.Instance.selectedTeam);
            AudioManager.Instance?.PlayWin();
        }

        IEnumerator LiveBadgePulse()
        {
            while (_running)
            {
                liveBadgeText.alpha = 1f;
                yield return new WaitForSeconds(0.6f);
                liveBadgeText.alpha = 0.3f;
                yield return new WaitForSeconds(0.4f);
            }
        }
    }
}
