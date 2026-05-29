using UnityEngine;
using UnityEngine.SceneManagement;
using VivaParintins.Data;

namespace VivaParintins.MiniGames
{
    public class MiniGameLoader : MonoBehaviour
    {
        public static MiniGameLoader Instance { get; private set; }
        static PhaseData _pendingPhase;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadPhase(PhaseData phase)
        {
            _pendingPhase = phase;
            SceneManager.LoadScene(GameManager.SCENE_GAME);
        }

        public void ReturnToMap() => SceneManager.LoadScene(GameManager.SCENE_MAP);

        // Chame isso no Start da MiniGameScene para pegar a fase
        public static PhaseData GetPendingPhase() => _pendingPhase;
    }
}
