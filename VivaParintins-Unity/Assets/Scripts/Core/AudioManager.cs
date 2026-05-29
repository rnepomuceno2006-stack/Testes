using System.Collections;
using UnityEngine;
using VivaParintins.Data;

namespace VivaParintins.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource ambientSource;

        [Header("SFX")]
        public AudioClip sfxPerfect;
        public AudioClip sfxGood;
        public AudioClip sfxMiss;
        public AudioClip sfxWin;
        public AudioClip sfxLumaCollect;
        public AudioClip sfxPhaseUnlock;
        public AudioClip sfxCardFlip;
        public AudioClip sfxButtonTap;

        float _musicVol = 1f;
        Coroutine _fadeCoroutine;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── Música tema ──────────────────────────────────────────────────
        public void PlayTeamTheme(TeamData team, float fadeDuration = 1.5f)
        {
            if (team.toadaTheme == null) return;
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(CrossFadeMusic(team.toadaTheme, fadeDuration));
        }

        IEnumerator CrossFadeMusic(AudioClip newClip, float duration)
        {
            // fade out
            float t = 0;
            float startVol = musicSource.volume;
            while (t < duration * 0.5f)
            {
                musicSource.volume = Mathf.Lerp(startVol, 0, t / (duration * 0.5f));
                t += Time.deltaTime;
                yield return null;
            }
            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.loop = true;
            musicSource.Play();
            // fade in
            t = 0;
            while (t < duration * 0.5f)
            {
                musicSource.volume = Mathf.Lerp(0, _musicVol, t / (duration * 0.5f));
                t += Time.deltaTime;
                yield return null;
            }
            musicSource.volume = _musicVol;
        }

        // ── SFX helpers ──────────────────────────────────────────────────
        public void PlayPerfect() => sfxSource.PlayOneShot(sfxPerfect);
        public void PlayGood()    => sfxSource.PlayOneShot(sfxGood);
        public void PlayMiss()    => sfxSource.PlayOneShot(sfxMiss);
        public void PlayWin()     => sfxSource.PlayOneShot(sfxWin);
        public void PlayLuma()    => sfxSource.PlayOneShot(sfxLumaCollect);
        public void PlayUnlock()  => sfxSource.PlayOneShot(sfxPhaseUnlock);
        public void PlayCard()    => sfxSource.PlayOneShot(sfxCardFlip);
        public void PlayTap()     => sfxSource.PlayOneShot(sfxButtonTap);

        public void SetMusicVolume(float v)
        {
            _musicVol = v;
            musicSource.volume = v;
        }
    }
}
