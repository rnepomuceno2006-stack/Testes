using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;

namespace VivaParintins.MiniGames
{
    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea] public string question;
        public string[] options = new string[4];
        public int correctIndex;
        public string explanation;
    }

    public class QuizGame : MiniGameBase
    {
        [Header("Quiz")]
        public QuizQuestion[] questions;
        public TextMeshProUGUI questionText;
        public Button[] optionButtons;
        public TextMeshProUGUI[] optionTexts;
        public TextMeshProUGUI feedbackText;
        public Image questionBg;

        int _current, _correct;
        List<QuizQuestion> _pool;
        bool _waiting;

        protected override void OnGameStart()
        {
            _current = 0; _correct = 0;
            _pool = new List<QuizQuestion>(questions);
            // embaralha
            for (int i = _pool.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_pool[i], _pool[j]) = (_pool[j], _pool[i]);
            }
            if (_pool.Count > 5) _pool = _pool.GetRange(0, 5);
            ShowQuestion();
        }

        void ShowQuestion()
        {
            if (_current >= _pool.Count) { EndGame((float)_correct / _pool.Count); return; }

            var q = _pool[_current];
            questionText.text = q.question;
            feedbackText.text = "";

            for (int i = 0; i < optionButtons.Length; i++)
            {
                bool valid = i < q.options.Length;
                optionButtons[i].gameObject.SetActive(valid);
                if (!valid) continue;
                optionTexts[i].text = q.options[i];
                optionButtons[i].interactable = true;

                int idx = i; // closure
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnAnswer(idx));
            }
        }

        void OnAnswer(int idx)
        {
            if (_waiting) return;
            _waiting = true;
            var q = _pool[_current];
            bool correct = idx == q.correctIndex;

            // visual feedback
            optionButtons[idx].GetComponent<Image>().color =
                correct ? new Color(0.2f, 0.8f, 0.3f) : new Color(0.9f, 0.2f, 0.2f);
            optionButtons[q.correctIndex].GetComponent<Image>().color =
                new Color(0.2f, 0.8f, 0.3f);

            foreach (var b in optionButtons) b.interactable = false;

            if (correct)
            {
                _correct++;
                feedbackText.text = "✅ Correto!";
                feedbackText.color = new Color(0.2f, 0.9f, 0.3f);
                AddScore(200 + (_current == 0 ? 50 : 0));
                AudioManager.Instance?.PlayPerfect();
            }
            else
            {
                feedbackText.text = $"❌ Errado! {q.explanation}";
                feedbackText.color = new Color(1f, 0.4f, 0.4f);
                AudioManager.Instance?.PlayMiss();
            }

            StartCoroutine(NextAfterDelay(1.1f));
        }

        IEnumerator NextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _current++;
            _waiting = false;
            foreach (var b in optionButtons)
                b.GetComponent<Image>().color = Color.white;
            ShowQuestion();
        }
    }
}
