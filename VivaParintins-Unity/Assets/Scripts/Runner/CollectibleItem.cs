// Coloque em cada ❤️ (Garantido) ou ⭐ (Caprichoso) no cenário.
// Requer: SpriteRenderer, Collider2D (Is Trigger = true), tag "Collectible"

using System.Collections;
using UnityEngine;
using VivaParintins.Core;

namespace VivaParintins.Runner
{
    [RequireComponent(typeof(Collider2D))]
    public class CollectibleItem : MonoBehaviour
    {
        [Header("Sprites")]
        public Sprite heartSprite;   // ❤️ Garantido
        public Sprite starSprite;    // ⭐ Caprichoso

        [Header("Animação idle")]
        public float bobAmplitude = 0.18f;
        public float bobSpeed     = 2.2f;
        public float rotateSpeed  = 45f;   // graus/s

        SpriteRenderer _sr;
        Vector3        _originPos;
        float          _phase;
        bool           _collected;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _originPos = transform.position;
            _phase = Random.Range(0f, Mathf.PI * 2f);

            // Aplica sprite conforme time
            if (_sr)
                _sr.sprite = GameManager.Instance?.selectedTeam == Team.Garantido
                    ? heartSprite : starSprite;
        }

        void Update()
        {
            if (_collected) return;

            // Bob vertical
            float y = _originPos.y + Mathf.Sin(Time.time * bobSpeed + _phase) * bobAmplitude;
            transform.position = new Vector3(_originPos.x, y, _originPos.z);

            // Rotação suave
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected || !other.CompareTag("Player")) return;
            _collected = true;
            RunnerLevelManager.Instance?.OnCollect(transform.position);
            StartCoroutine(CollectAnimation());
        }

        IEnumerator CollectAnimation()
        {
            float t = 0f;
            Vector3 startScale = transform.localScale;

            while (t < 0.2f)
            {
                t += Time.deltaTime;
                float p = t / 0.2f;
                transform.localScale = Vector3.Lerp(startScale, startScale * 1.6f, p);
                if (_sr) _sr.color = new Color(1f, 1f, 1f, 1f - p);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
