using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ArrowGame.Feedback
{
    public class HitFeedbackManager : MonoBehaviour
    {
        public static HitFeedbackManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private HitFeedbackConfig config;
        [SerializeField] private Image flashImage;

        private Tween flashTween;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (flashImage != null)
            {
                flashImage.color = Color.clear;
            }
        }

        private void Start()
        {
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
            if (config == null) return;

            PlayFlash(result.Zone);
            PlayCameraShake(result.Zone);
            PlayRingPunch(result.Ring);
            PlayHaptic(result.Zone);

            Debug.Log($"Hit! Zone: {result.Zone}, Distance: {result.Distance:F2}");
        }

        private void PlayFlash(Hit.HitZone zone)
        {
            if (flashImage == null) return;

            flashTween?.Kill();

            Color targetColor = config.GetFlashColor(zone);
            flashImage.color = targetColor;

            flashTween = flashImage.DOColor(Color.clear, config.flashDuration)
                .SetEase(Ease.OutQuad);
        }

        private void PlayCameraShake(Hit.HitZone zone)
        {
            if (Camera.CameraController.Instance == null) return;

            float strength = config.GetShakeStrength(zone);
            if (strength > 0)
            {
                Camera.CameraController.Instance.ShakeCamera(config.shakeDuration, strength);
            }
        }

        private void PlayRingPunch(Ring.RingController ring)
        {
            if (ring == null) return;

            ring.transform.DOPunchScale(
                Vector3.one * config.scalePunchStrength,
                config.scalePunchDuration,
                5,
                0.5f
            );
        }

        private void PlayHaptic(Hit.HitZone zone)
        {
            if (!config.hapticEnabled) return;

            switch (zone)
            {
                case Hit.HitZone.Core:
                    HapticFeedback.Success();
                    break;
                case Hit.HitZone.Inner:
                    HapticFeedback.MediumImpact();
                    break;
                case Hit.HitZone.Middle:
                    HapticFeedback.LightImpact();
                    break;
                case Hit.HitZone.Outer:
                    HapticFeedback.LightImpact();
                    break;
                case Hit.HitZone.Miss:
                    HapticFeedback.Error();
                    break;
            }
        }
    }
}
