// Coloque em cada nó (ponto turístico) no mapa-múndi.
// Requer um Collider2D para detectar toque.

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VivaParintins.Map
{
    public class RunnerMapNode : MonoBehaviour
    {
        [Header("Visual")]
        public SpriteRenderer nodeSprite;       // ícone do local
        public SpriteRenderer lockIcon;         // cadeado quando bloqueado
        public SpriteRenderer completedBadge;   // medalha quando completo
        public ParticleSystem idleParticles;    // brilho idle quando disponível
        public TextMesh       starsLabel;       // estrelas flutuantes 3D
        public TextMesh       nameLabel;        // nome do local

        // ── Config ───────────────────────────────────────────────────────
        public int  PhaseIndex { get; private set; }
        bool        _unlocked;
        Action<int> _onTap;

        static readonly Color LockedColor    = new Color(0.4f, 0.4f, 0.4f);
        static readonly Color UnlockedColor  = Color.white;
        static readonly Color CompletedColor = new Color(1f, 0.95f, 0.6f);

        // ── Setup ─────────────────────────────────────────────────────────
        public void Setup(int index, bool unlocked, int stars, bool isBoss, string displayName, Action<int> onTap)
        {
            PhaseIndex = index;
            _unlocked  = unlocked;
            _onTap     = onTap;

            // Cor do nó
            if (nodeSprite)
                nodeSprite.color = !unlocked ? LockedColor
                                 : stars > 0 ? CompletedColor
                                 : UnlockedColor;

            // Cadeado
            lockIcon?.gameObject.SetActive(!unlocked);

            // Medalha de completo
            completedBadge?.gameObject.SetActive(stars == 3);

            // Partículas: só em fases disponíveis não completadas
            if (idleParticles)
            {
                if (unlocked && stars == 0) idleParticles.Play();
                else                         idleParticles.Stop();
            }

            // Boss: escala maior
            if (isBoss && unlocked)
                transform.localScale = Vector3.one * 1.3f;

            // Estrelas
            if (starsLabel)
            {
                starsLabel.gameObject.SetActive(stars > 0);
                starsLabel.text = new string('★', stars);
                starsLabel.color = stars == 3 ? Color.yellow : Color.white;
            }

            // Nome
            if (nameLabel) nameLabel.text = displayName;

            // Float idle
            if (unlocked) StartCoroutine(FloatRoutine());
        }

        // ── Input ─────────────────────────────────────────────────────────
        void OnMouseDown() => _onTap?.Invoke(PhaseIndex);

        // ── Float ─────────────────────────────────────────────────────────
        System.Collections.IEnumerator FloatRoutine()
        {
            Vector3 origin = transform.localPosition;
            float   offset = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            while (true)
            {
                transform.localPosition = origin + Vector3.up *
                    (Mathf.Sin(Time.time * 1.5f + offset) * 0.12f);
                yield return null;
            }
        }
    }
}
