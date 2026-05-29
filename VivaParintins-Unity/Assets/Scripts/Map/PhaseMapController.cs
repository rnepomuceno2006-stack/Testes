using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;
using VivaParintins.Data;
using VivaParintins.VFX;

namespace VivaParintins.Map
{
    /// <summary>
    /// Controla o mapa estilo Mario World / Candy Crush.
    /// Câmera flutua suavemente sobre ilhas 3D temáticas.
    /// Cada nó é uma ilha com o personagem do quesito em cima.
    /// </summary>
    public class PhaseMapController : MonoBehaviour
    {
        [Header("Referências")]
        public PhaseData[] phases;           // 13 fases em ordem
        public Transform[] nightSeparators; // 3 marcos "Noite 1/2/3" no mapa
        public PhaseNodeView[] nodeViews;    // views dos nós (1 por fase)
        public Camera mapCamera;
        public float cameraFollowSpeed = 3f;

        [Header("UI")]
        public TextMeshProUGUI teamNameText;
        public TextMeshProUGUI progressText;
        public Slider tugOfWarSlider;
        public TextMeshProUGUI garScoreText;
        public TextMeshProUGUI capScoreText;
        public Button challengeBtn;
        public Button shareBtn;

        [Header("Estrelas Luma flutuantes")]
        public Transform[] lumaSpawnPoints;
        int _focusedNode = -1;

        // ── Unity lifecycle ───────────────────────────────────────────────
        void Start()
        {
            RefreshUI();
            RefreshNodes();
            FocusOnLatestUnlocked();
            AudioManager.Instance?.PlayTeamTheme(GameManager.Instance.CurrentTeamData);

            challengeBtn.onClick.AddListener(OnChallenge);
            shareBtn.onClick.AddListener(OnShare);
        }

        // ── Atualiza visual de todos os nós ──────────────────────────────
        void RefreshNodes()
        {
            for (int i = 0; i < nodeViews.Length; i++)
            {
                bool unlocked = GameManager.Instance.IsPhaseUnlocked(i);
                int stars = GameManager.Instance.phaseStars[i];
                nodeViews[i].SetState(unlocked, stars, phases[i]);

                if (unlocked && stars == 0)  // próxima a jogar — pulsa
                    nodeViews[i].StartPulse();

                // unlock VFX se acabou de desbloquear
                if (unlocked && stars == 0 && i > 0 && GameManager.Instance.phaseStars[i-1] > 0)
                    VFXManager.Instance?.PhaseUnlocked(nodeViews[i].transform.position);
            }
        }

        // ── Câmera scroll suave para o nó mais avançado ─────────────────
        void FocusOnLatestUnlocked()
        {
            for (int i = phases.Length - 1; i >= 0; i--)
            {
                if (GameManager.Instance.IsPhaseUnlocked(i))
                {
                    _focusedNode = i;
                    break;
                }
            }
            if (_focusedNode < 0) _focusedNode = 0;
            StartCoroutine(SmoothCameraToNode(_focusedNode, instant: true));
        }

        IEnumerator SmoothCameraToNode(int index, bool instant = false)
        {
            var target = nodeViews[index].transform.position + new Vector3(0, 8f, -12f);
            if (instant) { mapCamera.transform.position = target; yield break; }

            float t = 0;
            var start = mapCamera.transform.position;
            while (t < 1f)
            {
                mapCamera.transform.position = Vector3.Lerp(start, target, t);
                t += Time.deltaTime * cameraFollowSpeed;
                yield return null;
            }
            mapCamera.transform.position = target;
        }

        // ── Toque em nó ──────────────────────────────────────────────────
        public void OnNodeTapped(int phaseIndex)
        {
            if (!GameManager.Instance.IsPhaseUnlocked(phaseIndex)) return;
            AudioManager.Instance?.PlayTap();
            StartCoroutine(SmoothCameraToNode(phaseIndex));
            _focusedNode = phaseIndex;
            // breve delay → carrega mini-game
            StartCoroutine(LoadPhaseAfterDelay(phaseIndex, 0.8f));
        }

        IEnumerator LoadPhaseAfterDelay(int index, float delay)
        {
            yield return new WaitForSeconds(delay);
            MiniGameLoader.Instance.LoadPhase(phases[index]);
        }

        // ── UI ───────────────────────────────────────────────────────────
        void RefreshUI()
        {
            var gm = GameManager.Instance;
            var team = gm.CurrentTeamData;
            teamNameText.text = $"Modo Festival — {team.teamName}";
            teamNameText.color = team.primaryColor;

            int done = 0;
            for (int i = 0; i < gm.phaseStars.Length; i++)
                if (gm.phaseStars[i] > 0) done++;
            progressText.text = $"{done}/{phases.Length} fases concluídas";

            var (gp, cp) = gm.GetTugPercent();
            tugOfWarSlider.value = gp;
            garScoreText.text = FormatScore(gm.collectiveGar);
            capScoreText.text = FormatScore(gm.collectiveCap);
        }

        string FormatScore(int n) => n >= 1000 ? $"{n / 1000}k" : n.ToString();

        void OnChallenge()
        {
            var gm = GameManager.Instance;
            int total = 0;
            foreach (var s in gm.phaseStars) total += s * 100;
            string team = gm.selectedTeam == Team.Garantido ? "Garantido" : "Caprichoso";
            string txt = $"⚔️ Te DESAFIO no VIVA PARINTINS!!!\n" +
                         $"Defendo o {team}! Supere meus {total} pts!\n" +
                         $"Qual é o SEU boi? 🐂🔥";
            GUIUtility.systemCopyBuffer = txt;
            ToastManager.Show("Desafio copiado! Cole no WhatsApp 📲");
        }

        void OnShare()
        {
            int total = 0;
            foreach (var s in GameManager.Instance.phaseStars) total += s;
            string team = GameManager.Instance.selectedTeam == Team.Garantido
                ? "Garantido ❤️" : "Caprichoso ⭐";
            string txt = $"🎪 VIVA PARINTINS!!!\n" +
                         $"Defendo o {team}!\n" +
                         $"⭐ {total} estrelas em {phases.Length} fases do Festival de Parintins 2026!\n" +
                         $"Qual é o SEU boi? 🐂";
            GUIUtility.systemCopyBuffer = txt;
            ToastManager.Show("Progresso copiado! 📲");
        }
    }
}
