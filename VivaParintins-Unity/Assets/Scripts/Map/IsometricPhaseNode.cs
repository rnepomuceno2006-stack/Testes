// Coloque em cada "Building" do mapa isométrico.
// Cada building = prefab 3D cartoon do local (Porto, Feira, Bumbódromo etc).
// Requer um Collider para detectar toque/clique.

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VivaParintins.Map
{
    public class IsometricPhaseNode : MonoBehaviour
    {
        [Header("Visual")]
        public Sprite buildingIcon;          // ícone 2D para o popup
        public GameObject lockedOverlay;     // objeto semi-transparente quando bloqueado
        public GameObject glowEffect;        // brilho quando desbloqueado e não completado
        public ParticleSystem bossParticles; // partículas épicas só no Bumbódromo
        public TextMeshPro    starsLabel;    // label 3D flutuando sobre o building

        [Header("Bandeiras de time (ativam conforme fase)")]
        public GameObject garFlag;
        public GameObject capFlag;

        // Configurado por IsometricMapController.RefreshNodes()
        public int  PhaseIndex  { get; private set; }
        public bool IsUnlocked  { get; private set; }

        Action<int> _onTap;

        // ── Setup ─────────────────────────────────────────────────────────
        public void Setup(int index, bool unlocked, int stars, bool isBoss, Action<int> onTap)
        {
            PhaseIndex = index;
            IsUnlocked = unlocked;
            _onTap     = onTap;

            // Locked overlay
            lockedOverlay?.SetActive(!unlocked);

            // Glow: desbloqueado mas ainda não tem 3 estrelas
            glowEffect?.SetActive(unlocked && stars < 3);

            // Boss particles
            if (isBoss && unlocked) bossParticles?.Play();
            else bossParticles?.Stop();

            // Estrelas flutuantes
            if (starsLabel)
            {
                starsLabel.gameObject.SetActive(stars > 0);
                starsLabel.text = stars > 0
                    ? new string('★', stars) + new string('☆', 3 - stars)
                    : "";
            }

            // Bandeiras: mostram cores do time do jogador
            bool garSelected = Core.GameManager.Instance?.selectedTeam == Core.Team.Garantido;
            garFlag?.SetActive(unlocked && garSelected);
            capFlag?.SetActive(unlocked && !garSelected);

            // Animação idle de float vertical
            if (unlocked) StartIdleFloat();
        }

        // ── Click / Touch ─────────────────────────────────────────────────
        void OnMouseDown()
        {
#if !UNITY_EDITOR
            // Em mobile o toque chega via OnMouseDown com Physics Raycaster na câmera
#endif
            _onTap?.Invoke(PhaseIndex);
        }

        // ── Animação idle ─────────────────────────────────────────────────
        void StartIdleFloat()
        {
            StartCoroutine(IdleFloatRoutine());
        }

        System.Collections.IEnumerator IdleFloatRoutine()
        {
            Vector3 origin = transform.localPosition;
            float   phase  = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float   amp    = 0.15f;
            float   speed  = 1.2f;

            while (true)
            {
                float y = origin.y + Mathf.Sin(Time.time * speed + phase) * amp;
                transform.localPosition = new Vector3(origin.x, y, origin.z);
                yield return null;
            }
        }
    }
}
