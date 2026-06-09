// Coloque na cena Runner_Bumbodromo.
// Mostra os dois bois (Garantido vermelho e Caprichoso azul) dançando na arena
// ao fundo durante a corrida final, com holofotes e reação ao desempenho do jogador.

using UnityEngine;
using VivaParintins.Core;

namespace VivaParintins.Runner
{
    public class BumbodromoBois : MonoBehaviour
    {
        [Header("Bois na arena (sprites/quads ao fundo)")]
        public Transform boiGarantido;     // boi vermelho à esquerda
        public Transform boiCaprichoso;    // boi azul à direita

        [Header("Animação de dança")]
        public float swaySpeed     = 2.2f;   // velocidade do balanço
        public float swayAngle     = 8f;     // ângulo do balanço (graus)
        public float bobAmplitude  = 0.25f;  // sobe/desce
        public float bobSpeed      = 3f;

        [Header("Holofotes")]
        public Light  spotGarantido;
        public Light  spotCaprichoso;
        public Color  garColor = new Color(1f, 0.15f, 0.15f);
        public Color  capColor = new Color(0.15f, 0.3f, 1f);

        [Header("Destaque do boi do jogador")]
        [Tooltip("O boi do time do jogador fica maior e mais iluminado.")]
        public float  playerBoiScale = 1.25f;
        public float  rivalBoiScale  = 1f;

        Vector3 _garOrigin, _capOrigin;
        float   _garPhase, _capPhase;

        void Start()
        {
            if (boiGarantido) _garOrigin = boiGarantido.localPosition;
            if (boiCaprichoso) _capOrigin = boiCaprichoso.localPosition;
            _garPhase = 0f;
            _capPhase = Mathf.PI; // fora de fase para parecerem dançando em alternância

            if (spotGarantido) spotGarantido.color = garColor;
            if (spotCaprichoso) spotCaprichoso.color = capColor;

            HighlightPlayerBoi();
        }

        void Update()
        {
            AnimateBoi(boiGarantido, _garOrigin, _garPhase);
            AnimateBoi(boiCaprichoso, _capOrigin, _capPhase);
        }

        void AnimateBoi(Transform boi, Vector3 origin, float phase)
        {
            if (boi == null) return;

            // Bob vertical
            float y = origin.y + Mathf.Sin(Time.time * bobSpeed + phase) * bobAmplitude;
            boi.localPosition = new Vector3(origin.x, y, origin.z);

            // Sway (balanço lateral estilo dança do boi)
            float angle = Mathf.Sin(Time.time * swaySpeed + phase) * swayAngle;
            boi.localRotation = Quaternion.Euler(0, 0, angle);
        }

        void HighlightPlayerBoi()
        {
            bool isGar = GameManager.Instance != null && GameManager.Instance.selectedTeam == Team.Garantido;

            if (boiGarantido)
                boiGarantido.localScale = Vector3.one * (isGar ? playerBoiScale : rivalBoiScale);
            if (boiCaprichoso)
                boiCaprichoso.localScale = Vector3.one * (isGar ? rivalBoiScale : playerBoiScale);

            // Holofote do boi do jogador mais intenso
            if (spotGarantido) spotGarantido.intensity = isGar ? 2.5f : 1.2f;
            if (spotCaprichoso) spotCaprichoso.intensity = isGar ? 1.2f : 2.5f;
        }

        // Chamado pelo RunnerLevelManager quando o jogador coleta um item:
        // o boi do time pula de alegria
        public void OnPlayerScored()
        {
            bool isGar = GameManager.Instance != null && GameManager.Instance.selectedTeam == Team.Garantido;
            Transform boi = isGar ? boiGarantido : boiCaprichoso;
            if (boi != null) StartCoroutine(CelebrationHop(boi));
        }

        System.Collections.IEnumerator CelebrationHop(Transform boi)
        {
            Vector3 origin = boi.localPosition;
            float t = 0f;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                float hop = Mathf.Sin((t / 0.3f) * Mathf.PI) * 0.8f;
                boi.localPosition = origin + Vector3.up * hop;
                yield return null;
            }
            boi.localPosition = origin;
        }
    }
}
