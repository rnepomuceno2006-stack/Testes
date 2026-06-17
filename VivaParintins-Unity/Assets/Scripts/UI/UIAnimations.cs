// Animações de loop reutilizáveis — spec §5
// Bob, Glow, Twinkle, Fly (vaga-lume)

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VivaParintins.UI
{
    public class UIAnimations : MonoBehaviour
    {
        public enum AnimType { Bob, Glow, Twinkle, Fly }

        [Header("Tipo")]
        public AnimType animType = AnimType.Bob;

        [Header("Bob — translateY 0→-8→0, 2.2s")]
        public float bobAmplitude = 8f;
        public float bobDuration  = 2.2f;

        [Header("Glow — drop-shadow pulsa, 2.4s")]
        public Graphic glowTarget;
        public Color glowColorA = new Color(1f, 0.82f, 0.24f, 0.6f);
        public Color glowColorB = new Color(1f, 0.82f, 0.24f, 1f);
        public float glowDuration = 2.4f;

        [Header("Twinkle — opacity .5→1 + scale .8→1.1, 2s")]
        public float twinkleDuration = 2f;

        [Header("Fly — órbita irregular 5s (vaga-lume)")]
        public float flyRadius   = 12f;
        public float flySpeed    = 1f;
        public float flyOffsetY  = 6f;

        RectTransform _rect;
        Vector2 _originAnchor;
        Vector3 _originPos;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            if (_rect != null) _originAnchor = _rect.anchoredPosition;
            _originPos = transform.localPosition;
        }

        void OnEnable()
        {
            switch (animType)
            {
                case AnimType.Bob:     StartCoroutine(BobLoop());     break;
                case AnimType.Glow:    StartCoroutine(GlowLoop());    break;
                case AnimType.Twinkle: StartCoroutine(TwinkleLoop()); break;
                case AnimType.Fly:     StartCoroutine(FlyLoop());     break;
            }
        }

        void OnDisable() => StopAllCoroutines();

        IEnumerator BobLoop()
        {
            while (true)
            {
                float t = 0f;
                while (t < bobDuration)
                {
                    t += Time.unscaledDeltaTime;
                    float y = -Mathf.Sin(t / bobDuration * Mathf.PI * 2f) * bobAmplitude;
                    if (_rect != null)
                        _rect.anchoredPosition = _originAnchor + new Vector2(0, y);
                    else
                        transform.localPosition = _originPos + new Vector3(0, y / 100f, 0);
                    yield return null;
                }
            }
        }

        IEnumerator GlowLoop()
        {
            if (glowTarget == null) yield break;
            while (true)
            {
                float t = 0f;
                while (t < glowDuration / 2f)
                {
                    t += Time.unscaledDeltaTime;
                    glowTarget.color = Color.Lerp(glowColorA, glowColorB, t / (glowDuration / 2f));
                    yield return null;
                }
                t = 0f;
                while (t < glowDuration / 2f)
                {
                    t += Time.unscaledDeltaTime;
                    glowTarget.color = Color.Lerp(glowColorB, glowColorA, t / (glowDuration / 2f));
                    yield return null;
                }
            }
        }

        IEnumerator TwinkleLoop()
        {
            var group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();

            while (true)
            {
                float t = 0f;
                while (t < twinkleDuration / 2f)
                {
                    t += Time.unscaledDeltaTime;
                    float ratio = t / (twinkleDuration / 2f);
                    group.alpha = Mathf.Lerp(0.5f, 1f, ratio);
                    transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1.1f, ratio);
                    yield return null;
                }
                t = 0f;
                while (t < twinkleDuration / 2f)
                {
                    t += Time.unscaledDeltaTime;
                    float ratio = t / (twinkleDuration / 2f);
                    group.alpha = Mathf.Lerp(1f, 0.5f, ratio);
                    transform.localScale = Vector3.one * Mathf.Lerp(1.1f, 0.8f, ratio);
                    yield return null;
                }
            }
        }

        IEnumerator FlyLoop()
        {
            // Órbita irregular 5s — vaga-lume
            float seed = Random.value * Mathf.PI * 2f;
            while (true)
            {
                float time = Time.unscaledTime * flySpeed;
                float x = Mathf.Sin(time + seed) * flyRadius;
                float y = Mathf.Cos(time * 1.3f + seed) * flyOffsetY;

                if (_rect != null)
                    _rect.anchoredPosition = _originAnchor + new Vector2(x, y);
                else
                    transform.localPosition = _originPos + new Vector3(x / 100f, y / 100f, 0);

                yield return null;
            }
        }
    }
}
