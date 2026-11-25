using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer и Snapshots")]
    public AudioMixer mainMixer;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot pausedSnapshot;

    [Header("Источники музыки (Crossfade)")]
    public AudioSource menuMusicSource;
    public AudioSource levelMusicSource;
    public float crossfadeDuration = 1.0f;

    [Header("Источник звуковых эффектов (SFX)")]
    public AudioSource sfxSource;

    [Header("8+ Звуковых клипов")]
    public AudioClip moveClip;      // 1. Рывок/движение
    public AudioClip hitClip;       // 2. Попадание/урон
    public AudioClip pickupClip;    // 3. Подбор/очки
    public AudioClip windUpClip;    // 4. Замах врага ДО атаки
    public AudioClip deathClip;     // 5. Смерть/взрыв
    public AudioClip uiClickClip;   // 6. Клик по кнопке
    public AudioClip winClip;       // 7. Победа
    public AudioClip loseClip;      // 8. Поражение

    private bool isPlayingLevelMusic = false;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (SaveManager.Instance != null)
        {
            SetMusicVolume(SaveManager.Instance.data.musicVolume);
            SetSFXVolume(SaveManager.Instance.data.sfxVolume);
        }

        if (menuMusicSource != null && !menuMusicSource.isPlaying)
        {
            menuMusicSource.volume = 1f;
            menuMusicSource.Play();
        }
        if (levelMusicSource != null)
        {
            levelMusicSource.volume = 0f;
            levelMusicSource.Play();
        }
    }

    public void SwitchToLevelMusic()
    {
        if (isPlayingLevelMusic) return;
        isPlayingLevelMusic = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(CrossfadeMusic(menuMusicSource, levelMusicSource));
    }

    public void SwitchToMenuMusic()
    {
        if (!isPlayingLevelMusic) return;
        isPlayingLevelMusic = false;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(CrossfadeMusic(levelMusicSource, menuMusicSource));
    }

    IEnumerator CrossfadeMusic(AudioSource fadeOut, AudioSource fadeIn)
    {
        if (fadeIn != null && !fadeIn.isPlaying) fadeIn.Play();
        float timer = 0f;
        float startOutVol = fadeOut != null ? fadeOut.volume : 0f;

        while (timer < crossfadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / crossfadeDuration;
            if (fadeOut != null) fadeOut.volume = Mathf.Lerp(startOutVol, 0f, t);
            if (fadeIn != null) fadeIn.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        if (fadeOut != null) fadeOut.volume = 0f;
        if (fadeIn != null) fadeIn.volume = 1f;
    }

    public void SetPausedAudio(bool isPaused)
    {
        if (isPaused && pausedSnapshot != null)
            pausedSnapshot.TransitionTo(0.25f);
        else if (!isPaused && normalSnapshot != null)
            normalSnapshot.TransitionTo(0.25f);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float clamped = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        if (mainMixer != null)
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(clamped) * 20f);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.data.musicVolume = sliderValue;
            SaveManager.Instance.SaveGame();
        }
    }

    public void SetSFXVolume(float sliderValue)
    {
        float clamped = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        if (mainMixer != null)
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(clamped) * 20f);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.data.sfxVolume = sliderValue;
            SaveManager.Instance.SaveGame();
        }
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1.0f, bool randomPitch = true)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = randomPitch ? Random.Range(0.92f, 1.08f) : 1.0f;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayMove() => PlaySFX(moveClip, 0.25f);
    public void PlayHit() => PlaySFX(hitClip, 0.85f);
    public void PlayPickup() => PlaySFX(pickupClip, 0.9f);
    public void PlayWindUp() => PlaySFX(windUpClip, 0.5f);
    public void PlayDeath() => PlaySFX(deathClip, 1.0f);
    public void PlayUIClick() => PlaySFX(uiClickClip, 0.8f, false);
    public void PlayWin() => PlaySFX(winClip, 1.0f, false);
    public void PlayLose() => PlaySFX(loseClip, 1.0f, false);
}