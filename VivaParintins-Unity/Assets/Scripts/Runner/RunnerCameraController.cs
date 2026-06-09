using System.Collections;
using UnityEngine;

namespace VivaParintins.Runner
{
    [RequireComponent(typeof(Camera))]
    public class RunnerCameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform player;
        public float leadDistance = 3.5f;
        public float heightOffset = 1.5f;
        public float smoothSpeed = 8f;

        [Header("Bounds")]
        public float minX = 0f;
        public float maxX = 9999f;
        public float minY = -2f;
        public float maxY = 10f;

        [Header("Zoom")]
        public float normalSize = 5f;
        public float landmarkSize = 8f;
        public float zoomSpeed = 2f;

        public Transform Player => player;

        private Camera cam;
        private float targetSize;
        private bool locked;

        void Awake()
        {
            cam = GetComponent<Camera>();
            targetSize = normalSize;
            if (cam != null) cam.orthographicSize = normalSize;
        }

        void LateUpdate()
        {
            if (cam != null)
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

            if (locked || player == null) return;

            Vector3 target = new Vector3(
                Mathf.Clamp(player.position.x + leadDistance, minX, maxX),
                Mathf.Clamp(player.position.y + heightOffset, minY, maxY),
                transform.position.z
            );

            transform.position = Vector3.Lerp(transform.position, target, smoothSpeed * Time.deltaTime);
        }

        public void Shake(float duration = 0.3f, float magnitude = 0.15f)
        {
            StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        public void ZoomOutForLandmark()
        {
            targetSize = landmarkSize;
        }

        public void DetachFromPlayer()
        {
            locked = true;
        }

        IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            Vector3 originalPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float decay = 1f - (elapsed / duration);
                float x = originalPos.x + Random.Range(-1f, 1f) * magnitude * decay;
                float y = originalPos.y + Random.Range(-1f, 1f) * magnitude * decay;
                transform.position = new Vector3(x, y, originalPos.z);
                yield return null;
            }
            transform.position = originalPos;
        }
    }
}
