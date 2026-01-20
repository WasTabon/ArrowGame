using UnityEngine;
using UnityEngine.UI;

namespace ArrowGame.Audio
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private Button musicToggleButton;
        [SerializeField] private Image musicIconOn;
        [SerializeField] private Image musicIconOff;

        [Header("SFX")]
        [SerializeField] private Button sfxToggleButton;
        [SerializeField] private Image sfxIconOn;
        [SerializeField] private Image sfxIconOff;

        private void Start()
        {
            if (musicToggleButton != null)
            {
                musicToggleButton.onClick.AddListener(OnMusicToggle);
            }

            if (sfxToggleButton != null)
            {
                sfxToggleButton.onClick.AddListener(OnSfxToggle);
            }

            UpdateUI();
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void OnMusicToggle()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleMusic();
                UpdateUI();
            }
        }

        private void OnSfxToggle()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleSfx();
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (AudioManager.Instance == null) return;

            bool musicMuted = AudioManager.Instance.IsMusicMuted;
            bool sfxMuted = AudioManager.Instance.IsSfxMuted;

            if (musicIconOn != null) musicIconOn.gameObject.SetActive(!musicMuted);
            if (musicIconOff != null) musicIconOff.gameObject.SetActive(musicMuted);

            if (sfxIconOn != null) sfxIconOn.gameObject.SetActive(!sfxMuted);
            if (sfxIconOff != null) sfxIconOff.gameObject.SetActive(sfxMuted);
        }
    }
}
