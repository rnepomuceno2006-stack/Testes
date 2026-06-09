// Mapa-múndi estilo Super Mario Run / Mario World.
// Mostra os 6 pontos turísticos de Parintins como nós num caminho.
// Visual: vista aérea isométrica da cidade de Parintins com o caminho do torcedor.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;
using VivaParintins.Phases;

namespace VivaParintins.Map
{
    public class RunnerWorldMapController : MonoBehaviour
    {
        [Header("Nós do mapa (6 pontos turísticos)")]
        public RunnerMapNode[] nodes; // tamanho 6

        [Header("Personagem no mapa")]
        public Transform playerMarker;     // ícone do torcedor no mapa
        public float     markerMoveSpeed = 4f;

        [Header("HUD")]
        public TextMeshProUGUI teamNameText;
        public Image            teamColorBar;  // barra vermelha ou azul no topo
        public TextMeshProUGUI  collectiblesTotal; // total de ❤️/⭐ acumulados
        public TextMeshProUGUI  worldText;         // "Parintins — Jornada do Torcedor"

        [Header("Popup de fase")]
        public GameObject      popup;
        public Image           popupBg;
        public TextMeshProUGUI popupTitle;
        public TextMeshProUGUI popupDesc;
        public TextMeshProUGUI popupStars;
        public Button          playButton;
        public Button          closeButton;

        [Header("Config")]
        public RunnerPhaseList phaseList;

        // ── Estado ───────────────────────────────────────────────────────
        int _selectedPhase = -1;

        // ── Unity lifecycle ──────────────────────────────────────────────
        void Start()
        {
            RefreshHUD();
            RefreshNodes();
            popup.SetActive(false);
            playButton.onClick.AddListener(OnPlay);
            closeButton.onClick.AddListener(() => popup.SetActive(false));

            // Posiciona marker no nó mais avançado
            int furthest = GetFurthestUnlocked();
            if (playerMarker && nodes[furthest] != null)
                playerMarker.position = nodes[furthest].transform.position + Vector3.up * 0.5f;
        }

        // ── Nós ──────────────────────────────────────────────────────────
        void RefreshNodes()
        {
            bool isGar = GameManager.Instance.selectedTeam == Team.Garantido;

            for (int i = 0; i < nodes.Length && i < phaseList.phases.Length; i++)
            {
                if (nodes[i] == null) continue;
                bool unlocked = GameManager.Instance.IsPhaseUnlocked(i);
                int  stars    = i < GameManager.Instance.phaseStars.Length
                                    ? GameManager.Instance.phaseStars[i] : 0;
                bool isBoss   = i == 5;
                string name   = isGar
                                    ? phaseList.phases[i].landmarkNameGar
                                    : phaseList.phases[i].landmarkNameCap;

                nodes[i].Setup(i, unlocked, stars, isBoss, name, OnNodeTapped);
            }
        }

        void OnNodeTapped(int index)
        {
            if (!GameManager.Instance.IsPhaseUnlocked(index))
            {
                UI.ToastManager.Show("Complete a fase anterior primeiro!");
                return;
            }

            _selectedPhase = index;
            var cfg = phaseList.phases[index];
            bool isGar = GameManager.Instance.selectedTeam == Team.Garantido;

            popupTitle.text = isGar ? cfg.landmarkNameGar : cfg.landmarkNameCap;
            popupDesc.text  = cfg.description;

            int stars = index < GameManager.Instance.phaseStars.Length
                            ? GameManager.Instance.phaseStars[index] : 0;
            popupStars.text = stars > 0
                ? new string('⭐', stars) + new string('☆', 3 - stars)
                : "☆☆☆ Não jogada";

            // Cor do popup conforme time
            if (popupBg)
                popupBg.color = isGar
                    ? new Color(0.8f, 0.1f, 0.1f, 0.95f)
                    : new Color(0.05f, 0.15f, 0.7f, 0.95f);

            popup.SetActive(true);

            // Move o marker até o nó selecionado
            StartCoroutine(MoveMarkerTo(nodes[index].transform.position + Vector3.up * 0.5f));
        }

        void OnPlay()
        {
            if (_selectedPhase < 0) return;
            popup.SetActive(false);
            AudioManager.Instance?.PlayTap();
            GameManager.Instance.LoadPhaseRunner(_selectedPhase);
        }

        // ── HUD ──────────────────────────────────────────────────────────
        void RefreshHUD()
        {
            bool isGar = GameManager.Instance.selectedTeam == Team.Garantido;
            teamNameText.text = isGar ? "BOI GARANTIDO ❤️" : "BOI CAPRICHOSO ⭐";

            if (teamColorBar)
                teamColorBar.color = isGar
                    ? new Color(0.85f, 0.1f, 0.1f)
                    : new Color(0.05f, 0.2f, 0.85f);

            int total = 0;
            foreach (var s in GameManager.Instance.phaseStars) total += s;
            string icon = isGar ? "❤️" : "⭐";
            collectiblesTotal.text = $"{icon} {total * 10}";
            worldText.text = "Parintins — Jornada do Torcedor";
        }

        // ── Marker ───────────────────────────────────────────────────────
        IEnumerator MoveMarkerTo(Vector3 target)
        {
            if (playerMarker == null) yield break;
            Vector3 start = playerMarker.position;
            float   t     = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * markerMoveSpeed;
                playerMarker.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
        }

        int GetFurthestUnlocked()
        {
            int last = 0;
            for (int i = 0; i < nodes.Length; i++)
                if (GameManager.Instance.IsPhaseUnlocked(i)) last = i;
            return last;
        }
    }
}
