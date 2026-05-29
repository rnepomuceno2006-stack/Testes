using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;
using VivaParintins.Data;

namespace VivaParintins.MiniGames
{
    /// <summary>
    /// Classe base para todos os mini-games.
    /// Gerencia pontuação, transição de resultado e callbacks comuns.
    /// </summary>
    public abstract class MiniGameBase : MonoBehaviour
    {
        [Header("Base UI")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI scoreText;
        public GameObject gamePanel;
        public GameObject resultPanel;

        [Header("Painel de Resultado")]
        public TextMeshProUGUI resultTitleText;
        public TextMeshProUGUI resultStarsText;
        public TextMeshProUGUI resultScoreText;
        public TextMeshProUGUI resultPointsText;
        public Button nextButton;
        public Button shareButton;
        public Button retryButton;

        // ── Dados da fase ────────────────────────────────────────────────
        protected PhaseData CurrentPhase;
        protected int Score;
        protected int MaxCombo;

        // ── Inicialização ─────────────────────────────────────────────────
        public void StartPhase(PhaseData phase)
        {
            CurrentPhase = phase;
            Score = 0;
            if (titleText) titleText.text = $"{phase.phaseName}";
            gamePanel.SetActive(true);
            resultPanel.SetActive(false);
            OnGameStart();
        }

        protected abstract void OnGameStart();

        // ── Pontuação ────────────────────────────────────────────────────
        protected void AddScore(int pts)
        {
            Score += pts;
            if (scoreText) scoreText.text = Score.ToString("N0");
        }

        // ── Fim de jogo ──────────────────────────────────────────────────
        protected void EndGame(float accuracy)
        {
            int stars = accuracy >= 0.85f ? 3 : accuracy >= 0.55f ? 2 : accuracy > 0 ? 1 : 0;
            GameManager.Instance.SavePhaseResult(
                System.Array.IndexOf(FindObjectOfType<Map.PhaseMapController>().phases, CurrentPhase),
                stars);

            ShowResult(stars, accuracy);
            if (stars >= 2) AudioManager.Instance?.PlayWin();
        }

        void ShowResult(int stars, float accuracy)
        {
            gamePanel.SetActive(false);
            resultPanel.SetActive(true);

            resultTitleText.text = stars >= 2 ? "🎉 Mandou bem!" : "Fase Concluída";
            resultStarsText.text = new string('★', stars) + new string('☆', 3 - stars);
            resultScoreText.text = $"{CurrentPhase.quesito} — {Mathf.RoundToInt(accuracy * 100)}% de acerto";
            if (MaxCombo > 3)
                resultScoreText.text += $"  |  Combo máx: {MaxCombo}!";
            resultPointsText.text = $"+{Score} pts para o {GameManager.Instance.CurrentTeamData.teamName}!";

            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
                MiniGameLoader.Instance.ReturnToMap());

            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() => StartPhase(CurrentPhase));

            shareButton.onClick.RemoveAllListeners();
            shareButton.onClick.AddListener(ShareResult);

            // VFX chuva de estrelas/corações
            VFX.VFXManager.Instance?.VictoryRain(GameManager.Instance.selectedTeam);
        }

        void ShareResult()
        {
            int stars = 0;
            foreach (var s in GameManager.Instance.phaseStars) stars += s;
            var team = GameManager.Instance.CurrentTeamData;
            string txt = $"🎪 VIVA PARINTINS!!!\n" +
                         $"Completei \"{CurrentPhase.phaseName}\" pelo {team.teamName}!\n" +
                         $"⭐ {stars} estrelas no Festival de Parintins 2026!\n" +
                         $"Qual é o SEU boi? 🐂";
            GUIUtility.systemCopyBuffer = txt;
            ToastManager.Show("Copiado! Cole no WhatsApp 📲");
        }
    }
}
