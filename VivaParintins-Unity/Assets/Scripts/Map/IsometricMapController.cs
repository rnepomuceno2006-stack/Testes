// Coloque na Hierarquia: GameObject "IsometricMap" na MapScene.
// Câmera: ortográfica, rotation X=30, Y=45, Z=0 — visão isométrica clássica.
// Cada fase = um "Building" no mapa (Island, Porto, Feira, Mercado, Orla, Comunas, Curral, Bumbódromo).
//
// LAYOUT DOS 8 LOCAIS DE PARINTINS (isométrico, scrollável):
//   0: Porto de Parintins         — canto inferior esquerdo, barcos e guindastes
//   1: Feira de Artesanatos       — mercado colorido, barracas
//   2: Mercado Municipal          — prédio central com telhado verde
//   3: Orla de Parintins          — beira do rio, píeres
//   4: Comunas Bar                — bar animado, luzes noturnas
//   5: Curral do Boi              — arena/curral com bandeiras
//   6: Praça dos Dois Bois        — praça central com as duas estátuas
//   7: Bumbódromo (BOSS)          — grande estádio, centro do mapa

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;
using VivaParintins.Phases;

namespace VivaParintins.Map
{
    public class IsometricMapController : MonoBehaviour
    {
        [Header("Câmera isométrica")]
        public Camera mapCamera;
        public float  scrollSpeed    = 8f;
        public float  zoomMin        = 4f;
        public float  zoomMax        = 12f;
        public Vector2 mapBoundsMin;
        public Vector2 mapBoundsMax;

        [Header("Nós de fase (arraste os 8 GameObjects de building)")]
        public IsometricPhaseNode[] phaseNodes; // tamanho 8

        [Header("HUD")]
        public TextMeshProUGUI miçangasText;
        public TextMeshProUGUI penasText;
        public Slider          tugSlider;
        public TextMeshProUGUI garPctText;
        public TextMeshProUGUI capPctText;
        public TextMeshProUGUI teamNameText;

        [Header("Popup de fase")]
        public GameObject          popupPanel;
        public Image               popupBuildingIcon;
        public TextMeshProUGUI     popupTitle;
        public TextMeshProUGUI     popupDescription;
        public TextMeshProUGUI     popupStars;
        public TextMeshProUGUI     popupBestScore;
        public Button              popupPlayButton;
        public Button              popupCloseButton;

        [Header("Config")]
        public PhaseList phaseList;

        // ── Estado ───────────────────────────────────────────────────────
        IsometricPhaseNode _selected;
        Vector2 _dragOrigin;
        bool    _dragging;
        float   _pinchStartDist;
        float   _pinchStartZoom;

        // ── Unity lifecycle ──────────────────────────────────────────────
        void Start()
        {
            RefreshHUD();
            RefreshNodes();
            popupPanel.SetActive(false);
            popupCloseButton.onClick.AddListener(() => popupPanel.SetActive(false));
            popupPlayButton.onClick.AddListener(OnPlayClicked);

            // Centraliza câmera no nó mais avançado desbloqueado
            CenterOnFurthestUnlocked();
        }

        void Update()
        {
            HandleCameraInput();
        }

        // ── Nós ──────────────────────────────────────────────────────────
        void RefreshNodes()
        {
            for (int i = 0; i < phaseNodes.Length; i++)
            {
                if (phaseNodes[i] == null) continue;
                bool  unlocked = GameManager.Instance.IsPhaseUnlocked(i);
                int   stars    = GameManager.Instance.phaseStars[Mathf.Min(i, GameManager.Instance.phaseStars.Length - 1)];
                bool  isBoss   = i == 7;
                phaseNodes[i].Setup(i, unlocked, stars, isBoss, OnNodeTapped);
            }
        }

        void OnNodeTapped(int phaseIndex)
        {
            if (!GameManager.Instance.IsPhaseUnlocked(phaseIndex))
            {
                UI.ToastManager.Show("Complete a fase anterior primeiro!");
                return;
            }

            _selected = phaseNodes[phaseIndex];
            var cfg = phaseList.phases[phaseIndex];

            popupTitle.text       = cfg.displayName;
            popupDescription.text = cfg.description;

            int stars = GameManager.Instance.phaseStars[Mathf.Min(phaseIndex, GameManager.Instance.phaseStars.Length - 1)];
            popupStars.text = stars > 0
                ? new string('⭐', stars) + new string('☆', 3 - stars)
                : "☆☆☆ — não jogada";

            popupBestScore.text = stars > 0 ? $"Meta: {cfg.coinGoal} miçangas" : "";
            if (popupBuildingIcon && _selected.buildingIcon)
                popupBuildingIcon.sprite = _selected.buildingIcon;

            popupPlayButton.interactable = true;
            popupPanel.SetActive(true);

            // Câmera faz pan suave até o nó selecionado
            StartCoroutine(PanTo(_selected.transform.position));
        }

        void OnPlayClicked()
        {
            if (_selected == null) return;
            popupPanel.SetActive(false);
            AudioManager.Instance?.PlayTap();
            GameManager.Instance.LoadPhaseRunner(_selected.PhaseIndex);
        }

        // ── HUD ──────────────────────────────────────────────────────────
        void RefreshHUD()
        {
            miçangasText.text = $"🍄 {GameEconomyManager.Instance.Micangas}";
            penasText.text    = $"🪶 {GameEconomyManager.Instance.PenasOuro}";

            var (garPct, capPct) = GameManager.Instance.GetTugPercent();
            tugSlider.value  = garPct;
            garPctText.text  = $"❤️ {garPct * 100:F0}%";
            capPctText.text  = $"⭐ {capPct * 100:F0}%";
            teamNameText.text = GameManager.Instance.CurrentTeamData?.teamName ?? "";
        }

        // ── Câmera: drag + pinch zoom ────────────────────────────────────
        void HandleCameraInput()
        {
#if UNITY_EDITOR
            // Mouse scroll zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
                mapCamera.orthographicSize = Mathf.Clamp(
                    mapCamera.orthographicSize - scroll * 3f, zoomMin, zoomMax);

            // Drag com botão do meio ou alt+clique
            if (Input.GetMouseButtonDown(2) || (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt)))
                _dragOrigin = mapCamera.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButton(2) || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt)))
                PanCamera(_dragOrigin - (Vector2)mapCamera.ScreenToWorldPoint(Input.mousePosition));
#endif
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    _dragOrigin = mapCamera.ScreenToWorldPoint(t.position);
                    _dragging   = true;
                }
                if (t.phase == TouchPhase.Moved && _dragging)
                    PanCamera(_dragOrigin - (Vector2)mapCamera.ScreenToWorldPoint(t.position));
                if (t.phase == TouchPhase.Ended) _dragging = false;
            }
            else if (Input.touchCount == 2)
            {
                _dragging = false;
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);
                if (t1.phase == TouchPhase.Began)
                {
                    _pinchStartDist = Vector2.Distance(t0.position, t1.position);
                    _pinchStartZoom = mapCamera.orthographicSize;
                }
                float dist = Vector2.Distance(t0.position, t1.position);
                if (_pinchStartDist > 0)
                    mapCamera.orthographicSize = Mathf.Clamp(
                        _pinchStartZoom * (_pinchStartDist / dist), zoomMin, zoomMax);
            }
        }

        void PanCamera(Vector2 delta)
        {
            Vector3 pos = mapCamera.transform.position;
            pos.x = Mathf.Clamp(pos.x + delta.x, mapBoundsMin.x, mapBoundsMax.x);
            pos.z = Mathf.Clamp(pos.z + delta.y, mapBoundsMin.y, mapBoundsMax.y);
            mapCamera.transform.position = pos;
        }

        IEnumerator PanTo(Vector3 target)
        {
            Vector3 start = mapCamera.transform.position;
            Vector3 end   = new Vector3(target.x, mapCamera.transform.position.y, target.z - 4f);
            end.x = Mathf.Clamp(end.x, mapBoundsMin.x, mapBoundsMax.x);
            end.z = Mathf.Clamp(end.z, mapBoundsMin.y, mapBoundsMax.y);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * scrollSpeed * 0.5f;
                mapCamera.transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
        }

        void CenterOnFurthestUnlocked()
        {
            int last = 0;
            for (int i = 0; i < phaseNodes.Length; i++)
                if (GameManager.Instance.IsPhaseUnlocked(i)) last = i;

            if (phaseNodes[last] != null)
                StartCoroutine(PanTo(phaseNodes[last].transform.position));
        }
    }
}
