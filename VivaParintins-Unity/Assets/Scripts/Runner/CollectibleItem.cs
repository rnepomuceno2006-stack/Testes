using System.Collections;
using UnityEngine;
using VivaParintins.Core;
using VivaParintins.Data;

namespace VivaParintins.Runner
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CollectibleItem : MonoBehaviour
    {
        [Header("Sprites")]
        public Sprite heartSprite;
        public Sprite starSprite;

        [Header("Idle Animation")]
        public float bobAmplitude = 0.18f;
        public float bobSpeed = 2.2f;
        public float rotateSpeed = 45f;

        private SpriteRenderer sr;
        private Collider2D col;
        private Vector3 originPos;
        private float phase;
        private bool collected;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            col.isTrigger = true;
            originPos = transform.position;
            phase = Random.Range(0f, Mathf.PI * 2f);

            bool isGarantido = GameManager.Instance != null && GameManager.Instance.selectedTeam == Team.Garantido;
            if (sr != null)
                sr.sprite = isGarantido ? heartSprite : starSprite;
        }

        void Update()
        {
            if (collected) return;
            float y = originPos.y + Mathf.Sin(Time.time * bobSpeed + phase) * bobAmplitude;
            transform.position = new Vector3(originPos.x, y, originPos.z);
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || !other.CompareTag("Player")) return;
            collected = true;
            col.enabled = false;
            RunnerLevelManager.Instance?.OnCollect(transform.position);
            StartCoroutine(CollectAnimation());
        }

        IEnumerator CollectAnimation()
        {
            float elapsed = 0f;
            float duration = 0.2f;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = startScale * 1.6f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
                if (sr != null) sr.color = new Color(1f, 1f, 1f, 1f - t);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
