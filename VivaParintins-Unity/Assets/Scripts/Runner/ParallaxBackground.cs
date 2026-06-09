using UnityEngine;
using VivaParintins.Core;

namespace VivaParintins.Runner
{
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [Range(0f, 1f)]
        [Tooltip("0.1 = céu (lento), 0.5 = meio, 0.9 = frente (rápido)")]
        public float parallaxFactor = 0.5f;
        public bool infiniteLoop = true;

        [Header("Phase Sprites")]
        [Tooltip("6 sprites — Porto, Mercado, Tribo, Curral, Praça, Bumbódromo")]
        public Sprite[] phaseSprites;

        private Transform cameraTransform;
        private SpriteRenderer spriteRenderer;
        private float tileWidth;
        private float lastCameraX;

        void Start()
        {
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
            spriteRenderer = GetComponent<SpriteRenderer>();

            ApplyPhaseSprite();
            CalculateTileWidth();

            lastCameraX = cameraTransform != null ? cameraTransform.position.x : 0f;
        }

        void ApplyPhaseSprite()
        {
            if (spriteRenderer == null || phaseSprites == null) return;
            int phase = GameManager.Instance != null ? GameManager.Instance.currentPhaseIndex : 0;
            phase = Mathf.Clamp(phase, 0, phaseSprites.Length - 1);
            if (phaseSprites[phase] != null)
                spriteRenderer.sprite = phaseSprites[phase];
        }

        void CalculateTileWidth()
        {
            if (spriteRenderer != null && spriteRenderer.sprite != null)
                tileWidth = spriteRenderer.sprite.bounds.size.x * transform.localScale.x;
            else
                tileWidth = 20f;
        }

        void LateUpdate()
        {
            if (cameraTransform == null) return;

            float camDeltaX = cameraTransform.position.x - lastCameraX;
            lastCameraX = cameraTransform.position.x;

            transform.position += Vector3.right * (camDeltaX * parallaxFactor);

            if (!infiniteLoop || tileWidth <= 0f) return;

            float relativeCamX = cameraTransform.position.x * (1f - parallaxFactor);
            float distFromOrigin = transform.position.x - relativeCamX;

            if (distFromOrigin > tileWidth * 0.5f)
                transform.position -= Vector3.right * tileWidth;
            else if (distFromOrigin < -tileWidth * 0.5f)
                transform.position += Vector3.right * tileWidth;
        }
    }
}
