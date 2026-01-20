using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ArrowGame.Settings
{
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Music")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Image musicFillImage;

        [Header("SFX")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Image sfxFillImage;

        [Header("Vibration")]
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Image vibrationCheckmark;

        [Header("Post Processing")]
        [SerializeField] private Toggle postProcessingToggle;
        [SerializeField] private Image postProcessingCheckmark;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button resetButton;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.25f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        private Tween fadeTween;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        private void Start()
        {
            SetupListeners();
            LoadCurrentValues();
        }

        private void SetupListeners()
        {
            if (musicSlider != null)
            {
                musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
            }

            if (vibrationToggle != null)
            {
                vibrationToggle.onValueChanged.AddListener(OnVibrationToggleChanged);
            }

            if (postProcessingToggle != null)
            {
                postProcessingToggle.onValueChanged.AddListener(OnPostProcessingToggleChanged);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetClicked);
            }
        }

        private void LoadCurrentValues()
        {
            if (SettingsManager.Instance == null) return;

            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(SettingsManager.Instance.MusicVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(SettingsManager.Instance.SfxVolume);
            }

            if (vibrationToggle != null)
            {
                vibrationToggle.SetIsOnWithoutNotify(SettingsManager.Instance.VibrationEnabled);
                UpdateToggleVisual(vibrationCheckmark, SettingsManager.Instance.VibrationEnabled);
            }

            if (postProcessingToggle != null)
            {
                postProcessingToggle.SetIsOnWithoutNotify(SettingsManager.Instance.PostProcessingEnabled);
                UpdateToggleVisual(postProcessingCheckmark, SettingsManager.Instance.PostProcessingEnabled);
            }
        }

        private void OnMusicSliderChanged(float value)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetMusicVolume(value);
            }
        }

        private void OnSfxSliderChanged(float value)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetSfxVolume(value);
            }
        }

        private void OnVibrationToggleChanged(bool isOn)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetVibration(isOn);
                UpdateToggleVisual(vibrationCheckmark, isOn);

                if (isOn)
                {
                    Feedback.HapticFeedback.LightImpact();
                }
            }
        }

        private void OnPostProcessingToggleChanged(bool isOn)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetPostProcessing(isOn);
                UpdateToggleVisual(postProcessingCheckmark, isOn);
            }
        }

        private void UpdateToggleVisual(Image checkmark, bool isOn)
        {
            if (checkmark != null)
            {
                checkmark.gameObject.SetActive(isOn);
            }
        }

        private void OnResetClicked()
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.ResetToDefaults();
                LoadCurrentValues();
            }
        }

        public void Show()
        {
            if (settingsPanel == null) return;

            LoadCurrentValues();

            fadeTween?.Kill();
            settingsPanel.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = true;

                fadeTween = canvasGroup.DOFade(1f, fadeInDuration)
                    .SetUpdate(true)
                    .OnComplete(() => canvasGroup.interactable = true);
            }
        }

        public void Hide()
        {
            if (settingsPanel == null) return;

            fadeTween?.Kill();

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;

                fadeTween = canvasGroup.DOFade(0f, fadeOutDuration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        settingsPanel.SetActive(false);
                        canvasGroup.blocksRaycasts = false;
                    });
            }
            else
            {
                settingsPanel.SetActive(false);
            }
        }

        public void Toggle()
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}
