using UnityEngine;
using UnityEngine.SceneManagement;
using VivaParintins.Data;
using VivaParintins.Phases;

namespace VivaParintins.Core
{
    /// <summary>
    /// Singleton central do jogo. Persiste entre cenas.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Configuração")]
        public TeamData garantidoData;
        public TeamData caprichosoData;

        [Header("Estado atual")]
        public Team selectedTeam;
        public int lumaCoins;
        public int[] phaseStars = new int[13]; // 0–3 estrelas por fase
        public int collectiveGar;
        public int collectiveCap;

        // ── Nomes das cenas ────────────────────────────────────────────────
        public const string SCENE_INTRO  = "IntroScene";
        public const string SCENE_MAP    = "MapScene";
        public const string SCENE_GAME   = "MiniGameScene";
        public const string SCENE_GALLERY = "GalleryScene";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }

        // ── Seleção de boi ─────────────────────────────────────────────────
        public void SelectTeam(Team team)
        {
            selectedTeam = team;
            SaveProgress();
            SceneManager.LoadScene(SCENE_MAP);
        }

        public TeamData CurrentTeamData =>
            selectedTeam == Team.Garantido ? garantidoData : caprichosoData;

        // ── Progresso das fases ───────────────────────────────────────────
        public void SavePhaseResult(int phaseIndex, int stars)
        {
            if (stars > phaseStars[phaseIndex])
                phaseStars[phaseIndex] = stars;

            int pts = stars * 100 + Random.Range(0, 50);
            if (selectedTeam == Team.Garantido) collectiveGar += pts;
            else collectiveCap += pts;
            collectiveGar += Random.Range(0, Mathf.FloorToInt(pts * 0.6f));
            collectiveCap += Random.Range(0, Mathf.FloorToInt(pts * 0.6f));

            SaveProgress();
        }

        public bool IsPhaseUnlocked(int index)
        {
            if (index == 0) return true;
            return phaseStars[index - 1] > 0;
        }

        // ── Runner phase navigation ───────────────────────────────────────
        // Total de 8 fases runner + mini-games originais
        public int currentPhaseIndex;
        const int TotalRunnerPhases = 8;

        public void LoadPhaseRunner(int index)
        {
            currentPhaseIndex = Mathf.Clamp(index, 0, TotalRunnerPhases - 1);
            SaveProgress();
            // Carrega a cena da fase pelo índice — nomes definidos em PhaseDefinitions
            string[] sceneNames = {
                "Phase_Porto", "Phase_Feira", "Phase_Mercado", "Phase_Orla",
                "Phase_Comunas", "Phase_Curral", "Phase_Praca", "Phase_Bumbodromo"
            };
            SceneManager.LoadScene(sceneNames[currentPhaseIndex]);
        }

        public void LoadNextPhase()
        {
            int next = currentPhaseIndex + 1;
            if (next >= TotalRunnerPhases)
                SceneManager.LoadScene(SCENE_MAP);
            else
                LoadPhaseRunner(next);
        }

        public void OnPlayerDied()
        {
            // PhaseRunner trata o auto-revive via GameEconomyManager
            var runner = UnityEngine.Object.FindObjectOfType<Phases.PhaseRunner>();
            runner?.OnPlayerDied();
        }

        // ── Luma coins ───────────────────────────────────────────────────
        public void AddLuma(int amount)
        {
            lumaCoins += amount;
            SaveProgress();
        }

        // ── Persistência ─────────────────────────────────────────────────
        void SaveProgress()
        {
            PlayerPrefs.SetInt("selectedTeam", (int)selectedTeam);
            PlayerPrefs.SetInt("lumaCoins", lumaCoins);
            PlayerPrefs.SetInt("collectiveGar", collectiveGar);
            PlayerPrefs.SetInt("collectiveCap", collectiveCap);
            for (int i = 0; i < phaseStars.Length; i++)
                PlayerPrefs.SetInt($"phase_{i}", phaseStars[i]);
            PlayerPrefs.SetInt("currentPhaseIndex", currentPhaseIndex);
            PlayerPrefs.Save();
        }

        void LoadProgress()
        {
            selectedTeam = (Team)PlayerPrefs.GetInt("selectedTeam", 0);
            lumaCoins = PlayerPrefs.GetInt("lumaCoins", 0);
            collectiveGar = PlayerPrefs.GetInt("collectiveGar", 0);
            collectiveCap = PlayerPrefs.GetInt("collectiveCap", 0);
            for (int i = 0; i < phaseStars.Length; i++)
                phaseStars[i] = PlayerPrefs.GetInt($"phase_{i}", 0);
            currentPhaseIndex = PlayerPrefs.GetInt("currentPhaseIndex", 0);
        }

        // ── Placar nacional ──────────────────────────────────────────────
        public (float garPct, float capPct) GetTugPercent()
        {
            int tot = collectiveGar + collectiveCap;
            if (tot == 0) return (0.5f, 0.5f);
            return ((float)collectiveGar / tot, (float)collectiveCap / tot);
        }
    }
}
