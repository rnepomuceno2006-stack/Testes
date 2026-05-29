using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Core;
using VivaParintins.Data;
using VivaParintins.VFX;

namespace VivaParintins.UI
{
    /// <summary>
    /// Tela inicial épica.
    /// Background: planeta flutuante do festival com câmera orbitando (Mario Galaxy).
    /// Split visual: vermelho (Garantido) vs azul (Caprichoso).
    /// </summary>
    public class IntroController : MonoBehaviour
    {
        [Header("Countdown")]
        public TextMeshProUGUI daysText;
        public TextMeshProUGUI hoursText;
        public TextMeshProUGUI minutesText;
        public TextMeshProUGUI secondsText;
        public GameObject countdownPanel;
        public TextMeshProUGUI festivalStatusText;
        // Data do Festival: 26 Jun 2026 20:00 (Manaus UTC-4)
        static readonly DateTime FestivalStart = new DateTime(2026, 6, 26, 20, 0, 0);

        [Header("Elenco — painéis")]
        public Transform garCastContainer;   // parent dos itens de elenco GAR
        public Transform capCastContainer;
        public GameObject castItemPrefab;    // prefab: emoji + nome + papel

        [Header("Boi 3D")]
        public Transform garBoiPedestal;     // base giratória do Boi Garantido 3D
        public Transform capBoiPedestal;
        public ParticleSystem garAura;       // partículas vermelhas ao redor
        public ParticleSystem capAura;

        [Header("Botões")]
        public Button garButton;
        public Button capButton;
        public Button festivalModeButton;

        [Header("Animação intro")]
        public Animator introAnimator;       // animação de entrada épica
        public AudioClip introFanfare;       // fanfarra de abertura

        // ── Dados ────────────────────────────────────────────────────────
        [System.Serializable]
        public struct CastEntry { public string emoji, name, role; }
        public CastEntry[] garCast;
        public CastEntry[] capCast;

        // ── Unity lifecycle ───────────────────────────────────────────────
        void Start()
        {
            garButton.onClick.AddListener(() => SelectTeam(Team.Garantido));
            capButton.onClick.AddListener(() => SelectTeam(Team.Caprichoso));
            festivalModeButton.onClick.AddListener(OpenFestivalMode);

            PopulateCast(garCastContainer, garCast);
            PopulateCast(capCastContainer, capCast);

            introAnimator?.SetTrigger("Enter");
            if (introFanfare) AudioManager.Instance?.PlayTap(); // troque por fanfare

            garAura?.Play();
            capAura?.Play();

            StartCoroutine(CountdownLoop());
            StartCoroutine(PedestalRotation());
        }

        // ── Countdown ────────────────────────────────────────────────────
        IEnumerator CountdownLoop()
        {
            while (true)
            {
                UpdateCountdown();
                yield return new WaitForSeconds(1f);
            }
        }

        void UpdateCountdown()
        {
            var now = DateTime.UtcNow.AddHours(-4); // UTC-4 Manaus
            var festEnd = FestivalStart.AddDays(2).AddHours(23).AddMinutes(59);

            if (now > festEnd)
            {
                countdownPanel.SetActive(false);
                festivalStatusText.text = "🎊 Festival 2026 encerrado! Até 2027!";
                festivalStatusText.gameObject.SetActive(true);
                return;
            }
            if (now >= FestivalStart)
            {
                countdownPanel.SetActive(false);
                festivalStatusText.text = "🔥 O FESTIVAL ESTÁ ACONTECENDO AGORA!";
                festivalStatusText.gameObject.SetActive(true);
                return;
            }

            var diff = FestivalStart - now;
            daysText.text    = ((int)diff.TotalDays).ToString("D2");
            hoursText.text   = diff.Hours.ToString("D2");
            minutesText.text = diff.Minutes.ToString("D2");
            secondsText.text = diff.Seconds.ToString("D2");
        }

        // ── Elenco ────────────────────────────────────────────────────────
        void PopulateCast(Transform container, CastEntry[] cast)
        {
            foreach (Transform c in container) Destroy(c.gameObject);
            foreach (var entry in cast)
            {
                var go = Instantiate(castItemPrefab, container);
                var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length >= 3)
                {
                    texts[0].text = entry.emoji;
                    texts[1].text = entry.name;
                    texts[2].text = entry.role;
                }
            }
        }

        // ── Pedestais rotativos ──────────────────────────────────────────
        IEnumerator PedestalRotation()
        {
            while (true)
            {
                garBoiPedestal?.Rotate(Vector3.up, 30f * Time.deltaTime);
                capBoiPedestal?.Rotate(Vector3.up, 30f * Time.deltaTime);
                yield return null;
            }
        }

        // ── Seleção de boi ────────────────────────────────────────────────
        void SelectTeam(Team team)
        {
            AudioManager.Instance?.PlayTap();
            VFXManager.Instance?.PhaseBurst(
                team == Team.Garantido
                    ? garButton.transform.position
                    : capButton.transform.position,
                team);
            StartCoroutine(TransitionToGame(team));
        }

        IEnumerator TransitionToGame(Team team)
        {
            introAnimator?.SetTrigger("Exit");
            yield return new WaitForSeconds(0.6f);
            GameManager.Instance.SelectTeam(team);
        }

        void OpenFestivalMode()
        {
            AudioManager.Instance?.PlayTap();
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameManager.SCENE_MAP);
        }
    }
}
