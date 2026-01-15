using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ArrowGame.Speed
{
    public class SpeedDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private Image speedBar;
        [SerializeField] private Image speedBarBackground;

        [Header("Colors")]
        [SerializeField] private Color highSpeedColor = Color.green;
        [SerializeField] private Color midSpeedColor = Color.yellow;
        [SerializeField] private Color lowSpeedColor = Color.red;

        [Header("Animation")]
        [SerializeField] private float punchScale = 1.2f;
        [SerializeField] private float punchDuration = 0.2f;

        private Tween barTween;
        private Tween punchTween;

        private void Start()
        {
            if (SpeedController.Instance != null)
            {
                SpeedController.Instance.OnSpeedChanged += UpdateDisplay;
                UpdateDisplay(SpeedController.Instance.CurrentSpeed);
            }
        }

        private void OnEnable()
        {
            if (SpeedController.Instance != null)
            {
                SpeedController.Instance.OnSpeedChanged += UpdateDisplay;
            }
        }

        private void OnDisable()
        {
            if (SpeedController.Instance != null)
            {
                SpeedController.Instance.OnSpeedChanged -= UpdateDisplay;
            }
        }

        private void UpdateDisplay(float speed)
        {
            if (speedText != null)
            {
                speedText.text = $"{speed:F1}";
                
                punchTween?.Kill();
                speedText.transform.localScale = Vector3.one;
                punchTween = speedText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
            }

            if (speedBar != null && SpeedController.Instance != null)
            {
                float normalized = SpeedController.Instance.NormalizedSpeed;
                
                barTween?.Kill();
                barTween = speedBar.DOFillAmount(normalized, 0.3f).SetEase(Ease.OutQuad);

                Color targetColor = GetColorBySpeed(normalized);
                speedBar.DOColor(targetColor, 0.3f);
            }
        }

        private Color GetColorBySpeed(float normalized)
        {
            if (normalized > 0.6f)
            {
                return Color.Lerp(midSpeedColor, highSpeedColor, (normalized - 0.6f) / 0.4f);
            }
            else if (normalized > 0.3f)
            {
                return Color.Lerp(lowSpeedColor, midSpeedColor, (normalized - 0.3f) / 0.3f);
            }
            else
            {
                return lowSpeedColor;
            }
        }
    }
}
