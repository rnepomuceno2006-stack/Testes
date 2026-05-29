using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;
using VivaParintins.Data;
using VivaParintins.VFX;

namespace VivaParintins.MiniGames
{
    /// <summary>
    /// Mini-game de Ritmo de Toada.
    /// Notas (corações/estrelas 3D) caem do topo e o jogador toca no momento certo.
    /// Visual: lane 3D com perspectiva, efeitos de partícula no hit perfeito.
    /// </summary>
    public class RhythmGame : MiniGameBase
    {
        [Header("Ritmo")]
        public int bpm = 130;
        public int totalNotes = 16;
        public float noteSpeed = 400f;      // pixels/s
        public float hitWindow_Perfect = 50f;
        public float hitWindow_Good = 120f;

        [Header("UI")]
        public RectTransform laneContainer;
        public RectTransform hitLine;
        public GameObject notePrefab;       // nota 3D estilizada (coração/estrela)
        public Slider progressSlider;
        public TextMeshProUGUI judgeText;   // PERFEITO! / BOM! / ERROU
        public TextMeshProUGUI comboText;   // 🔥 Combo x7

        [Header("VFX")]
        public ParticleSystem hitParticlesGar;
        public ParticleSystem hitParticlesCap;
        public Animator cameraShakeAnim;

        // ── Estado ───────────────────────────────────────────────────────
        List<NoteInstance> _notes = new();
        int _spawned, _hits, _combo, _maxCombo;
        float _spawnTimer, _beatInterval;
        bool _running;

        struct NoteInstance
        {
            public RectTransform rect;
            public bool judged;
        }

        // ── Inicialização ─────────────────────────────────────────────────
        protected override void OnGameStart()
        {
            _running = true;
            _spawned = _hits = _combo = _maxCombo = 0;
            _spawnTimer = 0;

            var team = GameManager.Instance.CurrentTeamData;
            bpm = team.bpm;
            _beatInterval = 60f / bpm;

            AudioManager.Instance?.PlayTeamTheme(team);
        }

        void Update()
        {
            if (!_running) return;

            // spawn de notas
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _beatInterval && _spawned < totalNotes)
            {
                SpawnNote();
                _spawned++;
                _spawnTimer = 0;
            }

            // mover notas
            float hitY = hitLine.anchoredPosition.y;
            for (int i = _notes.Count - 1; i >= 0; i--)
            {
                var n = _notes[i];
                if (n.judged) continue;
                n.rect.anchoredPosition += Vector2.down * noteSpeed * Time.deltaTime;

                // passou da hit line → miss
                if (n.rect.anchoredPosition.y < hitY - hitWindow_Good)
                {
                    n.judged = true;
                    _notes[i] = n;
                    Judge("miss", n.rect.position);
                }
            }

            progressSlider.value = (float)_spawned / totalNotes;

            // acabou todas as notas?
            if (_spawned >= totalNotes && _notes.TrueForAll(n => n.judged))
                StartCoroutine(FinishAfterDelay(0.5f));
        }

        void SpawnNote()
        {
            var go = Instantiate(notePrefab, laneContainer);
            var rt = go.GetComponent<RectTransform>();

            // posição X ligeiramente aleatória dentro da lane
            rt.anchoredPosition = new Vector2(
                Random.Range(-60f, 60f),
                laneContainer.rect.height * 0.5f + 60f);

            // cor do boi
            var img = go.GetComponent<Image>();
            if (img != null)
                img.color = GameManager.Instance.CurrentTeamData.primaryColor;

            _notes.Add(new NoteInstance { rect = rt, judged = false });
        }

        // ── Input ────────────────────────────────────────────────────────
        public void OnTapButton()
        {
            if (!_running) return;
            AudioManager.Instance?.PlayTap();

            float hitY = hitLine.anchoredPosition.y;
            NoteInstance best = default;
            float bestDist = float.MaxValue;
            int bestIdx = -1;

            for (int i = 0; i < _notes.Count; i++)
            {
                if (_notes[i].judged) continue;
                float dist = Mathf.Abs(_notes[i].rect.anchoredPosition.y - hitY);
                if (dist < bestDist) { bestDist = dist; best = _notes[i]; bestIdx = i; }
            }

            if (bestIdx < 0) return;
            if (bestDist > hitWindow_Good) return;

            var n = _notes[bestIdx];
            n.judged = true;
            _notes[bestIdx] = n;

            string kind = bestDist < hitWindow_Perfect ? "perfect" : "good";
            Judge(kind, n.rect.position);
        }

        // ── Julgamento ───────────────────────────────────────────────────
        void Judge(string kind, Vector3 worldPos)
        {
            int pts = 0;
            switch (kind)
            {
                case "perfect":
                    pts = 100; _combo++; _hits++;
                    judgeText.text = "PERFEITO!"; judgeText.color = Color.yellow;
                    AudioManager.Instance?.PlayPerfect();
                    VFXManager.Instance?.JudgeEffect("perfect", worldPos);
                    break;
                case "good":
                    pts = 50; _combo++; _hits++;
                    judgeText.text = "BOM!"; judgeText.color = new Color(0.47f, 0.71f, 1f);
                    AudioManager.Instance?.PlayGood();
                    VFXManager.Instance?.JudgeEffect("good", worldPos);
                    break;
                case "miss":
                    pts = 0; _combo = 0;
                    judgeText.text = "ERROU"; judgeText.color = new Color(1f, 0.4f, 0.4f);
                    AudioManager.Instance?.PlayMiss();
                    VFXManager.Instance?.JudgeEffect("miss", worldPos);
                    break;
            }

            _maxCombo = Mathf.Max(_maxCombo, _combo);
            int bonus = _combo > 1 ? _combo * 5 : 0;
            AddScore(pts + bonus);
            comboText.text = _combo > 1 ? $"🔥 Combo x{_combo}" : "";

            // auto-limpar texto após delay
            StopCoroutine(nameof(ClearJudgeText));
            StartCoroutine(nameof(ClearJudgeText));
        }

        IEnumerator ClearJudgeText()
        {
            yield return new WaitForSeconds(0.5f);
            judgeText.text = "";
        }

        IEnumerator FinishAfterDelay(float delay)
        {
            _running = false;
            yield return new WaitForSeconds(delay);
            float accuracy = totalNotes > 0 ? (float)_hits / totalNotes : 0f;
            EndGame(accuracy);
        }
    }
}
