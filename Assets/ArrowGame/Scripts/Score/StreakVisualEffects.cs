using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ArrowGame.Score
{
    public class StreakVisualEffects : MonoBehaviour
    {
        public static StreakVisualEffects Instance { get; private set; }

        [Header("Screen Pulse")]
        [SerializeField] private Image screenPulseImage;
        [SerializeField] private float pulseAlpha = 0.15f;
        [SerializeField] private float pulseDuration = 0.3f;

        [Header("Vignette Intensity")]
        [SerializeField] private Image streakVignetteImage;
        [SerializeField] private float vignetteMaxAlpha = 0.3f;

        [Header("Colors by Multiplier")]
        [SerializeField] private Color color2x = Color.cyan;
        [SerializeField] private Color color3x = Color.green;
        [SerializeField] private Color color4x = Color.yellow;
        [SerializeField] private Color color5x = new Color(1f, 0.5f, 0f);

        private Tween pulseTween;
        private Tween vignetteTween;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (screenPulseImage != null)
            {
                screenPulseImage.color = new Color(1f, 1f, 1f, 0f);
            }

            if (streakVignetteImage != null)
            {
                streakVignetteImage.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        private void Start()
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.OnMultiplierChanged += OnMultiplierChanged;
                StreakManager.Instance.OnStreakBroken += OnStreakBroken;
            }
        }

        private void OnEnable()
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.OnMultiplierChanged += OnMultiplierChanged;
                StreakManager.Instance.OnStreakBroken += OnStreakBroken;
            }
        }

        private void OnDisable()
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.OnMultiplierChanged -= OnMultiplierChanged;
                StreakManager.Instance.OnStreakBroken -= OnStreakBroken;
            }
        }

        private void OnMultiplierChanged(int multiplier)
        {
            if (multiplier > 1)
            {
                PlayScreenPulse(multiplier);
                UpdateVignetteIntensity(multiplier);
            }
            else
            {
                FadeOutVignette();
            }
        }

        private void PlayScreenPulse(int multiplier)
        {
            if (screenPulseImage == null) return;

            pulseTween?.Kill();

            Color pulseColor = GetColorForMultiplier(multiplier);
            pulseColor.a = pulseAlpha;
            screenPulseImage.color = pulseColor;

            pulseTween = screenPulseImage.DOFade(0f, pulseDuration).SetEase(Ease.OutQuad);
        }

        private void UpdateVignetteIntensity(int multiplier)
        {
            if (streakVignetteImage == null) return;

            vignetteTween?.Kill();

            Color vignetteColor = GetColorForMultiplier(multiplier);
            float targetAlpha = vignetteMaxAlpha * (multiplier - 1) / 4f;
            vignetteColor.a = streakVignetteImage.color.a;

            streakVignetteImage.color = vignetteColor;
            vignetteTween = streakVignetteImage.DOFade(targetAlpha, 0.3f).SetEase(Ease.OutQuad);
        }

        private void FadeOutVignette()
        {
            if (streakVignetteImage == null) return;

            vignetteTween?.Kill();
            vignetteTween = streakVignetteImage.DOFade(0f, 0.3f).SetEase(Ease.OutQuad);
        }

        private Color GetColorForMultiplier(int multiplier)
        {
            switch (multiplier)
            {
                case 2: return color2x;
                case 3: return color3x;
                case 4: return color4x;
                case 5: return color5x;
                default: return color5x;
            }
        }

        private void OnStreakBroken()
        {
            if (screenPulseImage != null)
            {
                pulseTween?.Kill();
                screenPulseImage.color = new Color(1f, 0f, 0f, 0.3f);
                pulseTween = screenPulseImage.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);
            }

            FadeOutVignette();
        }

        public void TriggerPulse(Color color)
        {
            if (screenPulseImage == null) return;

            pulseTween?.Kill();
            color.a = pulseAlpha;
            screenPulseImage.color = color;
            pulseTween = screenPulseImage.DOFade(0f, pulseDuration).SetEase(Ease.OutQuad);
        }
    }
}
