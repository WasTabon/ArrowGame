using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace ArrowGame.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [Header("Post Processing")]
        [SerializeField] private Volume postProcessVolume;

        private float musicVolume = 1f;
        private float sfxVolume = 1f;
        private bool vibrationEnabled = true;
        private bool postProcessingEnabled = true;

        private const string MUSIC_VOLUME_KEY = "ArrowGame_MusicVolume";
        private const string SFX_VOLUME_KEY = "ArrowGame_SfxVolume";
        private const string VIBRATION_KEY = "ArrowGame_Vibration";
        private const string POST_PROCESSING_KEY = "ArrowGame_PostProcessing";

        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public bool VibrationEnabled => vibrationEnabled;
        public bool PostProcessingEnabled => postProcessingEnabled;

        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;
        public event Action<bool> OnVibrationChanged;
        public event Action<bool> OnPostProcessingChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
            ApplySettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            OnMusicVolumeChanged?.Invoke(musicVolume);
            SaveSettings();

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetMusicVolume(musicVolume);
            }
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            OnSfxVolumeChanged?.Invoke(sfxVolume);
            SaveSettings();

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetSfxVolume(sfxVolume);
            }
        }

        public void SetVibration(bool enabled)
        {
            vibrationEnabled = enabled;
            OnVibrationChanged?.Invoke(vibrationEnabled);
            SaveSettings();
        }

        public void SetPostProcessing(bool enabled)
        {
            postProcessingEnabled = enabled;
            OnPostProcessingChanged?.Invoke(postProcessingEnabled);
            ApplyPostProcessing();
            SaveSettings();
        }

        public void ToggleVibration()
        {
            SetVibration(!vibrationEnabled);
        }

        public void TogglePostProcessing()
        {
            SetPostProcessing(!postProcessingEnabled);
        }

        private void ApplySettings()
        {
            ApplyPostProcessing();

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetMusicVolume(musicVolume);
                Audio.AudioManager.Instance.SetSfxVolume(sfxVolume);
            }
        }

        private void ApplyPostProcessing()
        {
            if (postProcessVolume != null)
            {
                postProcessVolume.enabled = postProcessingEnabled;
            }
        }

        public void SetPostProcessVolume(Volume volume)
        {
            postProcessVolume = volume;
            ApplyPostProcessing();
        }

        private void LoadSettings()
        {
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            vibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
            postProcessingEnabled = PlayerPrefs.GetInt(POST_PROCESSING_KEY, 1) == 1;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.SetInt(VIBRATION_KEY, vibrationEnabled ? 1 : 0);
            PlayerPrefs.SetInt(POST_PROCESSING_KEY, postProcessingEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ResetToDefaults()
        {
            SetMusicVolume(1f);
            SetSfxVolume(1f);
            SetVibration(true);
            SetPostProcessing(true);
        }
    }
}
