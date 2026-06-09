// Câmera 2D side-scrolling estilo Super Mario Run.
// Segue o jogador com offset à frente, limites de mapa e shake no hit.

using System.Collections;
using UnityEngine;

namespace VivaParintins.Runner
{
    public class RunnerCameraController : MonoBehaviour
    {
        [Header("Alvo")]
        public Transform player;
        public float leadDistance   = 3.5f;   // quanto à frente da câmera fica
        public float heightOffset   = 1.5f;   // altura acima do jogador
        public float smoothSpeed    = 8f;

        [Header("Limites")]
        public float minX = 0f;
        public float maxX = 999f;
        public float minY = -2f;
        public float maxY = 10f;

        [Header("Zoom")]
        public float normalSize    = 5f;
        public float landmarkSize  = 8f;
        public float zoomSpeed     = 2f;

        Camera _cam;
        float  _targetSize;
        bool   _locked;      // true ao completar fase — câmera para de seguir

        void Awake()
        {
            _cam = GetComponent<Camera>();
            _targetSize = normalSize;
            if (_cam) _cam.orthographicSize = normalSize;
        }

        void LateUpdate()
        {
            if (_locked || player == null) return;

            Vector3 target = new Vector3(
                Mathf.Clamp(player.position.x + leadDistance, minX, maxX),
                Mathf.Clamp(player.position.y + heightOffset, minY, maxY),
                transform.position.z);

            transform.position = Vector3.Lerp(transform.position, target, smoothSpeed * Time.deltaTime);

            // Zoom suave
            if (_cam)
                _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetSize, zoomSpeed * Time.deltaTime);
        }

        // ── API pública ──────────────────────────────────────────────────

        // Chamado pelo RunnerLevelManager ao tomar hit
        public void Shake(float duration = 0.3f, float magnitude = 0.25f)
        {
            StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        // Chamado pelo RunnerLevelManager ao completar fase
        public void ZoomOutForLandmark()
        {
            _locked = true;
            _targetSize = landmarkSize;
            // Continua o zoom mesmo sem seguir o jogador
            StartCoroutine(FinishZoom());
        }

        IEnumerator FinishZoom()
        {
            while (_cam && Mathf.Abs(_cam.orthographicSize - landmarkSize) > 0.05f)
            {
                _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, landmarkSize, zoomSpeed * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            Vector3 originalPos = transform.position;
            float   elapsed     = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
                float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
                transform.position = new Vector3(x, y, originalPos.z);
                yield return null;
            }
            transform.position = originalPos;
        }
    }
}
