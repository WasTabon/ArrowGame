using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace ArrowGame.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioConfig config;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        private List<AudioSource> layerSources = new List<AudioSource>();
        private int currentActiveLayer = -1;
        private Tween musicFadeTween;

        private bool isMusicMuted;
        private bool isSfxMuted;

        private const string MUSIC_MUTE_KEY = "ArrowGame_MusicMuted";
        private const string SFX_MUTE_KEY = "ArrowGame_SfxMuted";

        public bool IsMusicMuted => isMusicMuted;
        public bool IsSfxMuted => isSfxMuted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSources();
            LoadMuteSettings();
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("AudioConfig not assigned to AudioManager!");
                return;
            }

            SetupStreakLayers();
            SubscribeToEvents();
            ApplySettingsFromManager();

            PlayMenuMusic();
        }

        private void ApplySettingsFromManager()
        {
            if (Settings.SettingsManager.Instance != null)
            {
                SetMusicVolume(Settings.SettingsManager.Instance.MusicVolume);
                SetSfxVolume(Settings.SettingsManager.Instance.SfxVolume);
            }
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SfxSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (uiSource == null)
            {
                GameObject uiObj = new GameObject("UISource");
                uiObj.transform.SetParent(transform);
                uiSource = uiObj.AddComponent<AudioSource>();
                uiSource.playOnAwake = false;
                uiSource.ignoreListenerPause = true;
            }
        }

        private void SetupStreakLayers()
        {
            if (config.streakLayers == null || config.streakLayers.Length == 0) return;

            GameObject layersParent = new GameObject("StreakLayers");
            layersParent.transform.SetParent(transform);

            for (int i = 0; i < config.streakLayers.Length; i++)
            {
                GameObject layerObj = new GameObject($"Layer_{i}");
                layerObj.transform.SetParent(layersParent.transform);
                
                AudioSource layerSource = layerObj.AddComponent<AudioSource>();
                layerSource.clip = config.streakLayers[i];
                layerSource.loop = true;
                layerSource.playOnAwake = false;
                layerSource.volume = 0f;
                
                layerSources.Add(layerSource);
            }
        }

        private void SubscribeToEvents()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += OnHit;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnMultiplierChanged += OnMultiplierChanged;
                Score.StreakManager.Instance.OnStreakBroken += OnStreakBroken;
            }

            if (Score.ScoreManager.Instance != null)
            {
                Score.ScoreManager.Instance.OnHighScoreBeaten += OnHighScoreBeaten;
            }
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit -= OnHit;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnMultiplierChanged -= OnMultiplierChanged;
                Score.StreakManager.Instance.OnStreakBroken -= OnStreakBroken;
            }

            if (Score.ScoreManager.Instance != null)
            {
                Score.ScoreManager.Instance.OnHighScoreBeaten -= OnHighScoreBeaten;
            }
        }

        private void OnGameStateChanged(Core.GameState state)
        {
            switch (state)
            {
                case Core.GameState.MainMenu:
                    PlayMenuMusic();
                    StopAllLayers();
                    break;
                case Core.GameState.Run:
                    PlayGameMusic();
                    StartStreakLayers();
                    break;
                case Core.GameState.GameOver:
                    PlaySound(config.gameOverSound, config.uiSoundVolume);
                    FadeOutLayers();
                    break;
            }
        }

        private void OnHit(Hit.HitResult result)
        {
            AudioClip clip = config.GetHitSound(result.Zone);
            if (clip != null)
            {
                float pitch = 1f;
                if (result.Zone == Hit.HitZone.Core)
                {
                    pitch = Random.Range(1.1f, 1.2f);
                }
                else if (result.Zone == Hit.HitZone.Miss)
                {
                    pitch = Random.Range(0.8f, 0.9f);
                }
                
                PlaySoundWithPitch(clip, config.hitSoundVolume, pitch);
            }
        }

        private void OnMultiplierChanged(int multiplier)
        {
            if (multiplier > 1)
            {
                PlaySound(config.multiplierUpSound, config.streakSoundVolume);
                SetActiveLayer(multiplier - 2);
            }
            else
            {
                SetActiveLayer(-1);
            }
        }

        private void OnStreakBroken()
        {
            PlaySound(config.streakBreakSound, config.streakSoundVolume);
            FadeOutLayers();
        }

        private void OnHighScoreBeaten(int score)
        {
            PlaySound(config.newHighScoreSound, config.uiSoundVolume);
        }

        public void PlayMenuMusic()
        {
            if (config.menuMusic != null)
            {
                CrossfadeMusic(config.menuMusic);
            }
        }

        public void PlayGameMusic()
        {
            if (config.gameMusic != null)
            {
                CrossfadeMusic(config.gameMusic);
            }
        }

        private void CrossfadeMusic(AudioClip newClip)
        {
            if (isMusicMuted) return;

            musicFadeTween?.Kill();

            if (musicSource.isPlaying && musicSource.clip == newClip) return;

            float targetVolume = config.musicVolume;

            if (musicSource.isPlaying)
            {
                musicFadeTween = musicSource.DOFade(0f, config.musicFadeDuration / 2f)
                    .OnComplete(() =>
                    {
                        musicSource.clip = newClip;
                        musicSource.Play();
                        musicSource.DOFade(targetVolume, config.musicFadeDuration / 2f);
                    });
            }
            else
            {
                musicSource.clip = newClip;
                musicSource.volume = 0f;
                musicSource.Play();
                musicFadeTween = musicSource.DOFade(targetVolume, config.musicFadeDuration);
            }
        }

        public void PlaySound(AudioClip clip, float volume = 1f)
        {
            if (clip == null || isSfxMuted) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void PlaySoundWithPitch(AudioClip clip, float volume, float pitch)
        {
            if (clip == null || isSfxMuted) return;
            
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
            sfxSource.pitch = 1f;
        }

        public void PlayUISound(AudioClip clip)
        {
            if (clip == null || isSfxMuted) return;
            uiSource.PlayOneShot(clip, config.uiSoundVolume);
        }

        public void PlayButtonClick()
        {
            PlayUISound(config.buttonClickSound);
        }

        public void PlayCountdownTick()
        {
            PlayUISound(config.countdownTickSound);
        }

        public void PlayCountdownGo()
        {
            PlayUISound(config.countdownGoSound);
        }

        private void StartStreakLayers()
        {
            if (isMusicMuted) return;

            foreach (var source in layerSources)
            {
                if (source.clip != null)
                {
                    source.volume = 0f;
                    source.time = musicSource.time % source.clip.length;
                    source.Play();
                }
            }
        }

        private void SetActiveLayer(int layerIndex)
        {
            if (isMusicMuted) return;

            for (int i = 0; i < layerSources.Count; i++)
            {
                float targetVolume = (i <= layerIndex) ? config.layerVolume : 0f;
                layerSources[i].DOFade(targetVolume, config.layerFadeDuration);
            }

            currentActiveLayer = layerIndex;
        }

        private void FadeOutLayers()
        {
            foreach (var source in layerSources)
            {
                source.DOFade(0f, config.layerFadeDuration);
            }
            currentActiveLayer = -1;
        }

        private void StopAllLayers()
        {
            foreach (var source in layerSources)
            {
                source.Stop();
                source.volume = 0f;
            }
            currentActiveLayer = -1;
        }

        public void ToggleMusic()
        {
            isMusicMuted = !isMusicMuted;
            
            if (isMusicMuted)
            {
                musicSource.Pause();
                foreach (var source in layerSources)
                {
                    source.Pause();
                }
            }
            else
            {
                musicSource.UnPause();
                foreach (var source in layerSources)
                {
                    source.UnPause();
                }
            }

            SaveMuteSettings();
        }

        public void ToggleSfx()
        {
            isSfxMuted = !isSfxMuted;
            SaveMuteSettings();
        }

        public void SetMusicVolume(float volume)
        {
            config.musicVolume = volume;
            if (!isMusicMuted)
            {
                musicSource.volume = volume;
            }
        }

        public void SetSfxVolume(float volume)
        {
            config.hitSoundVolume = volume;
            config.uiSoundVolume = volume;
            config.streakSoundVolume = volume;
        }

        private void LoadMuteSettings()
        {
            isMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1;
            isSfxMuted = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;
        }

        private void SaveMuteSettings()
        {
            PlayerPrefs.SetInt(MUSIC_MUTE_KEY, isMusicMuted ? 1 : 0);
            PlayerPrefs.SetInt(SFX_MUTE_KEY, isSfxMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
