using UnityEngine;
using DG.Tweening;
using System;

namespace ArrowGame.Speed
{
    public class SpeedController : MonoBehaviour
    {
        public static SpeedController Instance { get; private set; }

        [SerializeField] private SpeedConfig config;

        private float currentSpeed;
        private Tween speedTween;

        public float CurrentSpeed => currentSpeed;
        public float NormalizedSpeed => Mathf.InverseLerp(config.minSpeed, config.maxSpeed, currentSpeed);

        public event Action<float> OnSpeedChanged;
        public event Action OnSpeedReachedZero;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("SpeedConfig not assigned to SpeedController!");
                return;
            }

            currentSpeed = config.startSpeed;
            ApplySpeedToNeedle();

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += HandleHit;
            }
        }

        private void OnEnable()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += HandleHit;
            }
        }

        private void OnDisable()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit -= HandleHit;
            }
        }

        private void HandleHit(Hit.HitResult result)
        {
            float speedChange = config.GetSpeedChange(result.Zone);
            ChangeSpeed(speedChange);
        }

        public void ChangeSpeed(float amount)
        {
            float targetSpeed = Mathf.Clamp(currentSpeed + amount, config.minSpeed, config.maxSpeed);

            speedTween?.Kill();
            speedTween = DOTween.To(() => currentSpeed, SetSpeed, targetSpeed, config.speedChangeDuration)
                .SetEase(Ease.OutQuad);
        }

        private void SetSpeed(float speed)
        {
            currentSpeed = speed;
            ApplySpeedToNeedle();
            OnSpeedChanged?.Invoke(currentSpeed);

            if (currentSpeed <= config.minSpeed)
            {
                OnSpeedReachedZero?.Invoke();
            }
        }

        private void ApplySpeedToNeedle()
        {
            if (Needle.NeedleController.Instance != null)
            {
                Needle.NeedleController.Instance.SetSpeed(currentSpeed);
            }
        }

        public void ResetSpeed()
        {
            speedTween?.Kill();
            currentSpeed = config.startSpeed;
            ApplySpeedToNeedle();
            OnSpeedChanged?.Invoke(currentSpeed);
        }
    }
}
