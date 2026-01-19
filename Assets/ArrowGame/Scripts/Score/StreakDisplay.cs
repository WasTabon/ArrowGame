using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ArrowGame.Score
{
    public class StreakDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private Image streakFillBar;
        [SerializeField] private Image glowImage;

        [Header("Streak Thresholds (for fill bar)")]
        [SerializeField] private int maxStreakForBar = 35;

        [Header("Animation")]
        [SerializeField] private float punchScale = 1.3f;
        [SerializeField] private float punchDuration = 0.2f;
        [SerializeField] private float shakeStrength = 5f;
        [SerializeField] private float shakeDuration = 0.3f;

        [Header("Colors")]
        [SerializeField] private Color streak1xColor = Color.white;
        [SerializeField] private Color streak2xColor = Color.cyan;
        [SerializeField] private Color streak3xColor = Color.green;
        [SerializeField] private Color streak4xColor = Color.yellow;
        [SerializeField] private Color streak5xColor = new Color(1f, 0.5f, 0f);

        [Header("Glow Settings")]
        [SerializeField] private float glowPulseSpeed = 2f;
        [SerializeField] private float glowMinAlpha = 0.2f;
        [SerializeField] private float glowMaxAlpha = 0.8f;

        private Tween punchTween;
        private Tween glowTween;
        private int lastMultiplier = 1;

        private void Start()
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.OnStreakChanged += UpdateStreakDisplay;
                StreakManager.Instance.OnMultiplierChanged += UpdateMultiplierDisplay;
                StreakManager.Instance.OnStreakBroken += OnStreakBroken;

                UpdateStreakDisplay(StreakManager.Instance.CurrentStreak);
                UpdateMultiplierDisplay(StreakManager.Instance.CurrentMultiplier);
            }

            if (glowImage != null)
            {
                SetGlowAlpha(0f);
            }
        }

        private void OnEnable()
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.OnStreakChanged += UpdateStreakDisplay;
                StreakManager.Instance.OnMultiplierChanged += UpdateMultiplierDisplay;
                StreakManager.Instance.OnStreakBroken += OnStreakBroken;
            }
        }

        private void OnDisable()
        {
            if (StreakManager.Instance != null)
            {
                StreakManager.Instance.OnStreakChanged -= UpdateStreakDisplay;
                StreakManager.Instance.OnMultiplierChanged -= UpdateMultiplierDisplay;
                StreakManager.Instance.OnStreakBroken -= OnStreakBroken;
            }
        }

        private void Update()
        {
            UpdateGlowPulse();
        }

        private void UpdateStreakDisplay(int streak)
        {
            if (streakText != null)
            {
                streakText.text = streak.ToString();

                if (streak > 0)
                {
                    punchTween?.Kill();
                    streakText.transform.localScale = Vector3.one;
                    punchTween = streakText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
                }
            }

            if (streakFillBar != null)
            {
                float fill = Mathf.Clamp01((float)streak / maxStreakForBar);
                streakFillBar.DOFillAmount(fill, 0.2f).SetEase(Ease.OutQuad);
            }
        }

        private void UpdateMultiplierDisplay(int multiplier)
        {
            if (multiplierText != null)
            {
                multiplierText.text = $"x{multiplier}";
                multiplierText.color = GetMultiplierColor(multiplier);

                if (multiplier > lastMultiplier)
                {
                    multiplierText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5, 0.5f);
                }
            }

            if (streakFillBar != null)
            {
                streakFillBar.color = GetMultiplierColor(multiplier);
            }

            UpdateGlowIntensity(multiplier);
            lastMultiplier = multiplier;
        }

        private Color GetMultiplierColor(int multiplier)
        {
            switch (multiplier)
            {
                case 1: return streak1xColor;
                case 2: return streak2xColor;
                case 3: return streak3xColor;
                case 4: return streak4xColor;
                case 5: return streak5xColor;
                default: return streak5xColor;
            }
        }

        private void UpdateGlowIntensity(int multiplier)
        {
            if (glowImage == null) return;

            glowTween?.Kill();

            if (multiplier <= 1)
            {
                glowTween = DOTween.To(() => glowImage.color.a, SetGlowAlpha, 0f, 0.3f);
            }
            else
            {
                glowImage.color = new Color(
                    GetMultiplierColor(multiplier).r,
                    GetMultiplierColor(multiplier).g,
                    GetMultiplierColor(multiplier).b,
                    glowImage.color.a
                );
            }
        }

        private void UpdateGlowPulse()
        {
            if (glowImage == null) return;
            if (StreakManager.Instance == null) return;

            float intensity = StreakManager.Instance.StreakIntensity;
            if (intensity <= 0) return;

            float pulse = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, 
                (Mathf.Sin(Time.time * glowPulseSpeed * intensity) + 1f) / 2f);
            
            SetGlowAlpha(pulse * intensity);
        }

        private void SetGlowAlpha(float alpha)
        {
            if (glowImage == null) return;
            Color c = glowImage.color;
            c.a = alpha;
            glowImage.color = c;
        }

        private void OnStreakBroken()
        {
            if (streakText != null)
            {
                streakText.transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90f, false, true);
                streakText.DOColor(Color.red, 0.1f).OnComplete(() =>
                {
                    streakText.DOColor(streak1xColor, 0.3f);
                });
            }

            glowTween?.Kill();
            if (glowImage != null)
            {
                glowTween = DOTween.To(() => glowImage.color.a, SetGlowAlpha, 0f, 0.3f);
            }
        }
    }
}
