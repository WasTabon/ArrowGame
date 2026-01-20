using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Feedback
{
    public class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Default Settings")]
        [SerializeField] private float defaultDuration = 0.2f;
        [SerializeField] private float defaultStrength = 0.3f;
        [SerializeField] private int defaultVibrato = 10;
        [SerializeField] private float defaultRandomness = 90f;

        private Vector3 originalPosition;
        private Tween shakeTween;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (cameraTransform == null)
            {
                cameraTransform = UnityEngine.Camera.main.transform;
            }

            if (cameraTransform != null)
            {
                originalPosition = cameraTransform.localPosition;
            }
        }

        public void Shake()
        {
            Shake(defaultDuration, defaultStrength, defaultVibrato, defaultRandomness);
        }

        public void Shake(float duration, float strength, int vibrato = 10, float randomness = 90f)
        {
            if (cameraTransform == null) return;

            shakeTween?.Kill();
    
            Vector3 currentPos = cameraTransform.localPosition;

            shakeTween = cameraTransform.DOShakePosition(duration, strength, vibrato, randomness, false, true)
                .OnComplete(() => cameraTransform.localPosition = currentPos);
        }

        public void ShakeLight()
        {
            Shake(0.1f, 0.15f, 8, 90f);
        }

        public void ShakeMedium()
        {
            Shake(0.2f, 0.3f, 12, 90f);
        }

        public void ShakeHeavy()
        {
            Shake(0.3f, 0.5f, 15, 90f);
        }

        public void ShakeOnHit(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core:
                    ShakeMedium();
                    break;
                case Hit.HitZone.Inner:
                    ShakeLight();
                    break;
                case Hit.HitZone.Miss:
                    ShakeHeavy();
                    break;
            }
        }

        private void OnDestroy()
        {
            shakeTween?.Kill();
            if (cameraTransform != null)
            {
                cameraTransform.localPosition = originalPosition;
            }
        }
    }
}
