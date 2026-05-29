using System.Collections;
using UnityEngine;
using TMPro;
using VivaParintins.Data;

namespace VivaParintins.Map
{
    /// <summary>
    /// View de cada nó de fase no mapa 3D.
    /// Ilha flutuante com personagem, estrelas e efeitos de glow.
    /// </summary>
    public class PhaseNodeView : MonoBehaviour
    {
        [Header("Visual")]
        public GameObject islandMesh;        // ilha 3D (prefab da fase)
        public GameObject characterDisplay;  // personagem 3D em cima da ilha
        public GameObject lockOverlay;       // cadeado quando bloqueado
        public Renderer glowRing;            // anel de glow abaixo da ilha
        public GameObject[] starObjects;     // 3 estrelas 3D (★)
        public GameObject pulseRing;         // anel pulsante quando disponível

        [Header("UI World Space")]
        public TextMeshPro phaseNameLabel;
        public TextMeshPro quesiteLabel;

        [Header("Animação")]
        public float floatAmplitude = 0.3f;
        public float floatSpeed = 1.2f;
        float _floatOffset;
        Vector3 _basePosition;
        bool _isPulsing;

        // ── Dados ────────────────────────────────────────────────────────
        PhaseData _data;
        bool _unlocked;
        int _stars;

        void Start()
        {
            _basePosition = transform.position;
            _floatOffset = Random.Range(0f, Mathf.PI * 2);
        }

        void Update()
        {
            // ilha flutua suavemente como em Mario Galaxy
            float y = Mathf.Sin(Time.time * floatSpeed + _floatOffset) * floatAmplitude;
            transform.position = _basePosition + Vector3.up * y;
        }

        // ── API ──────────────────────────────────────────────────────────
        public void SetState(bool unlocked, int stars, PhaseData data)
        {
            _unlocked = unlocked;
            _stars = stars;
            _data = data;

            lockOverlay.SetActive(!unlocked);
            characterDisplay.SetActive(unlocked);
            phaseNameLabel.text = data.phaseName;
            quesiteLabel.text = data.quesito;

            // estrelas
            for (int i = 0; i < starObjects.Length; i++)
                starObjects[i].SetActive(i < stars);

            // glow ativo apenas se desbloqueado
            if (glowRing != null)
            {
                glowRing.gameObject.SetActive(unlocked);
                if (unlocked)
                    glowRing.material.SetColor("_EmissionColor",
                        stars > 0 ? data.nightColor * 1.5f : Color.white * 0.6f);
            }
        }

        public void StartPulse()
        {
            if (_isPulsing || !_unlocked) return;
            _isPulsing = true;
            pulseRing.SetActive(true);
            StartCoroutine(PulseRoutine());
        }

        IEnumerator PulseRoutine()
        {
            var s = pulseRing.transform;
            while (_isPulsing)
            {
                float t = 0;
                while (t < 1f) { s.localScale = Vector3.one * Mathf.Lerp(0.9f, 1.25f, t); t += Time.deltaTime * 1.5f; yield return null; }
                t = 0;
                while (t < 1f) { s.localScale = Vector3.one * Mathf.Lerp(1.25f, 0.9f, t); t += Time.deltaTime * 1.5f; yield return null; }
            }
        }

        // ── Toque ────────────────────────────────────────────────────────
        void OnMouseDown()
        {
            if (!_unlocked) return;
            int idx = transform.GetSiblingIndex();
            FindObjectOfType<PhaseMapController>()?.OnNodeTapped(idx);
        }
    }
}
