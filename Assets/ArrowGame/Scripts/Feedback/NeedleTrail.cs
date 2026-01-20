using ArrowGame.Speed;
using UnityEngine;

namespace ArrowGame.Feedback
{
    [RequireComponent(typeof(TrailRenderer))]
    public class NeedleTrail : MonoBehaviour
    {
        public static NeedleTrail Instance { get; private set; }

        [Header("Trail Settings")]
        [SerializeField] private float baseWidth = 0.1f;
        [SerializeField] private float maxWidth = 0.3f;
        [SerializeField] private float baseTime = 0.2f;
        [SerializeField] private float maxTime = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color streak2xColor = Color.cyan;
        [SerializeField] private Color streak3xColor = Color.green;
        [SerializeField] private Color streak4xColor = Color.yellow;
        [SerializeField] private Color streak5xColor = new Color(1f, 0.5f, 0f);

        [Header("Speed Response")]
        [SerializeField] private float minSpeed = 5f;
        [SerializeField] private float maxSpeed = 25f;

        private TrailRenderer trailRenderer;
        private int currentMultiplier = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            trailRenderer = GetComponent<TrailRenderer>();
        }

        private void Start()
        {
            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnMultiplierChanged += OnMultiplierChanged;
            }

            if (SpeedController.Instance != null)
            {
                SpeedController.Instance.OnSpeedChanged += OnSpeedChanged;
            }

            UpdateTrailColor(1);
        }

        private void OnEnable()
        {
            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnMultiplierChanged += OnMultiplierChanged;
            }

            if (SpeedController.Instance != null)
            {
                SpeedController.Instance.OnSpeedChanged += OnSpeedChanged;
            }
        }

        private void OnDisable()
        {
            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnMultiplierChanged -= OnMultiplierChanged;
            }

            if (SpeedController.Instance != null)
            {
                SpeedController.Instance.OnSpeedChanged -= OnSpeedChanged;
            }
        }

        private void OnMultiplierChanged(int multiplier)
        {
            currentMultiplier = multiplier;
            UpdateTrailColor(multiplier);
        }

        private void OnSpeedChanged(float speed)
        {
            UpdateTrailSize(speed);
        }

        private void UpdateTrailColor(int multiplier)
        {
            if (trailRenderer == null) return;

            Color color;
            switch (multiplier)
            {
                case 2: color = streak2xColor; break;
                case 3: color = streak3xColor; break;
                case 4: color = streak4xColor; break;
                case 5: color = streak5xColor; break;
                default: color = normalColor; break;
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(color, 0f),
                    new GradientColorKey(color, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );

            trailRenderer.colorGradient = gradient;
        }

        private void UpdateTrailSize(float speed)
        {
            if (trailRenderer == null) return;

            float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);

            trailRenderer.startWidth = Mathf.Lerp(baseWidth, maxWidth, t);
            trailRenderer.time = Mathf.Lerp(baseTime, maxTime, t);
        }

        public void SetEnabled(bool enabled)
        {
            if (trailRenderer != null)
            {
                trailRenderer.emitting = enabled;
            }
        }

        public void ClearTrail()
        {
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }

        public void Flash(Color color, float duration = 0.2f)
        {
            if (trailRenderer == null) return;

            Color originalColor = GetCurrentColor();
            SetTrailColor(color);

            StartCoroutine(FlashCoroutine(originalColor, duration));
        }

        private System.Collections.IEnumerator FlashCoroutine(Color originalColor, float duration)
        {
            yield return new WaitForSeconds(duration);
            UpdateTrailColor(currentMultiplier);
        }

        private void SetTrailColor(Color color)
        {
            if (trailRenderer == null) return;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(color, 0f),
                    new GradientColorKey(color, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );

            trailRenderer.colorGradient = gradient;
        }

        private Color GetCurrentColor()
        {
            switch (currentMultiplier)
            {
                case 2: return streak2xColor;
                case 3: return streak3xColor;
                case 4: return streak4xColor;
                case 5: return streak5xColor;
                default: return normalColor;
            }
        }
    }
}
