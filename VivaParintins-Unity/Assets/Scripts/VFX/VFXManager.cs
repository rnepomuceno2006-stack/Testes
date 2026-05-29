using System.Collections;
using UnityEngine;
using VivaParintins.Data;

namespace VivaParintins.VFX
{
    /// <summary>
    /// Controla todos os efeitos de partícula do jogo.
    /// Requer Particle System prefabs configurados com os visuais Mario Galaxy.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Partículas — Garantido (corações vermelhos)")]
        public GameObject heartBurstPrefab;      // explosão ao completar fase
        public GameObject heartTrailPrefab;      // rastro do personagem
        public GameObject heartRainPrefab;       // chuva na tela de vitória

        [Header("Partículas — Caprichoso (estrelas azuis/amarelas)")]
        public GameObject starBurstPrefab;
        public GameObject starTrailPrefab;
        public GameObject starRainPrefab;

        [Header("Partículas Compartilhadas")]
        public GameObject lumaCollectPrefab;     // coletar Luma espírito
        public GameObject phaseUnlockPrefab;     // fase desbloqueada
        public GameObject perfectHitPrefab;      // acerto perfeito no ritmo
        public GameObject goodHitPrefab;         // bom acerto
        public GameObject missHitPrefab;         // erro (fumaça cinza)
        public GameObject cardOpenPrefab;        // abertura de card

        [Header("Efeitos de Tela")]
        public Animator screenShakeAnimator;     // shake na câmera no miss
        public GameObject screenFlashPrefab;     // flash branco no PERFEITO!

        [Header("Luma Spirits flutuantes")]
        public GameObject lumaGarPrefab;         // espírito coração vermelho
        public GameObject lumaCapPrefab;         // espírito estrela azul

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── API pública ───────────────────────────────────────────────────
        public void PhaseBurst(Vector3 worldPos, Team team)
        {
            var prefab = team == Team.Garantido ? heartBurstPrefab : starBurstPrefab;
            Spawn(prefab, worldPos);
        }

        public void JudgeEffect(string kind, Vector3 worldPos)
        {
            switch (kind)
            {
                case "perfect": Spawn(perfectHitPrefab, worldPos); ScreenFlash(); break;
                case "good":    Spawn(goodHitPrefab,    worldPos); break;
                case "miss":    Spawn(missHitPrefab,    worldPos); ScreenShake(); break;
            }
        }

        public void LumaCollect(Vector3 worldPos, Team team)
        {
            Spawn(lumaCollectPrefab, worldPos);
            // rastro sobe em direção à câmera
            var trail = team == Team.Garantido ? heartTrailPrefab : starTrailPrefab;
            Spawn(trail, worldPos);
        }

        public void PhaseUnlocked(Vector3 nodePos) => Spawn(phaseUnlockPrefab, nodePos);

        public void CardOpen(Vector3 screenCenter) => Spawn(cardOpenPrefab, screenCenter);

        public void VictoryRain(Team team)
        {
            var prefab = team == Team.Garantido ? heartRainPrefab : starRainPrefab;
            Spawn(prefab, Camera.main.transform.position + Vector3.forward * 3f);
        }

        // ── Helpers ───────────────────────────────────────────────────────
        GameObject Spawn(GameObject prefab, Vector3 pos)
        {
            if (prefab == null) return null;
            var go = Instantiate(prefab, pos, Quaternion.identity);
            // auto-destroy quando sistema de partícula terminar
            var ps = go.GetComponent<ParticleSystem>();
            if (ps != null) Destroy(go, ps.main.duration + ps.main.startLifetime.constantMax);
            return go;
        }

        void ScreenFlash()
        {
            if (screenFlashPrefab != null)
                StartCoroutine(FlashRoutine());
        }

        IEnumerator FlashRoutine()
        {
            var go = Instantiate(screenFlashPrefab);
            yield return new WaitForSeconds(0.15f);
            Destroy(go);
        }

        void ScreenShake()
        {
            if (screenShakeAnimator != null)
                screenShakeAnimator.SetTrigger("Shake");
        }
    }
}
