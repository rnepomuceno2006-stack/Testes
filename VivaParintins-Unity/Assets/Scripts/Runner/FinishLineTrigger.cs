using UnityEngine;
using System.Collections;

namespace VivaParintins.Runner
{
    [RequireComponent(typeof(Collider2D))]
    public class FinishLineTrigger : MonoBehaviour
    {
        [Header("VFX")]
        public GameObject confettiPrefab;

        [Header("References")]
        public RunnerCameraController cameraController;

        private bool triggered;

        void Start()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered) return;
            if (!other.CompareTag("Player")) return;

            triggered = true;
            SpawnConfetti();
            RunnerLevelManager.Instance?.LevelComplete();

            if (cameraController != null)
                StartCoroutine(ZoomAndDetach(cameraController));
        }

        void SpawnConfetti()
        {
            if (confettiPrefab == null) return;
            var vfx = Instantiate(confettiPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 4f);
        }

        IEnumerator ZoomAndDetach(RunnerCameraController cam)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            cam.DetachFromPlayer();
            cam.ZoomOutForLandmark();
        }
    }
}
