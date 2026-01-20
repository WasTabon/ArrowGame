using ArrowGame.Input;
using UnityEngine;

namespace ArrowGame.Feedback
{
    public class GameFeelManager : MonoBehaviour
    {
        public static GameFeelManager Instance { get; private set; }

        [Header("Features Toggle")]
        [SerializeField] private bool enableScreenShake = true;
        [SerializeField] private bool enableHitStop = true;
        [SerializeField] private bool enableParticles = true;
        [SerializeField] private bool enableSlowMoAudio = true;

        [Header("Screen Shake Settings")]
        [SerializeField] private bool shakeOnCore = true;
        [SerializeField] private bool shakeOnInner = true;
        [SerializeField] private bool shakeOnMiss = true;

        [Header("Hit Stop Settings")]
        [SerializeField] private bool hitStopOnCore = true;
        [SerializeField] private bool hitStopOnInner = false;
        [SerializeField] private bool hitStopOnMiss = true;

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
            SubscribeToEvents();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += OnHit;
            }

            if (InputController.Instance != null)
            {
                InputController.Instance.OnInputStart += OnSlowMoStart;
                InputController.Instance.OnInputEnd += OnSlowMoEnd;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit -= OnHit;
            }

            if (InputController.Instance != null)
            {
                InputController.Instance.OnInputStart -= OnSlowMoStart;
                InputController.Instance.OnInputEnd -= OnSlowMoEnd;
            }
        }

        private void OnHit(Hit.HitResult result)
        {
            if (enableScreenShake)
            {
                HandleScreenShake(result.Zone);
            }

            if (enableHitStop)
            {
                HandleHitStop(result.Zone);
            }

            if (enableParticles)
            {
                HandleParticles(result.Zone);
            }

            HandleHaptics(result.Zone);
        }

        private void HandleScreenShake(Hit.HitZone zone)
        {
            if (ScreenShake.Instance == null) return;

            bool shouldShake = false;
            switch (zone)
            {
                case Hit.HitZone.Core:
                    shouldShake = shakeOnCore;
                    break;
                case Hit.HitZone.Inner:
                    shouldShake = shakeOnInner;
                    break;
                case Hit.HitZone.Miss:
                    shouldShake = shakeOnMiss;
                    break;
            }

            if (shouldShake)
            {
                ScreenShake.Instance.ShakeOnHit(zone);
            }
        }

        private void HandleHitStop(Hit.HitZone zone)
        {
            if (HitStop.Instance == null) return;

            bool shouldStop = false;
            switch (zone)
            {
                case Hit.HitZone.Core:
                    shouldStop = hitStopOnCore;
                    break;
                case Hit.HitZone.Inner:
                    shouldStop = hitStopOnInner;
                    break;
                case Hit.HitZone.Miss:
                    shouldStop = hitStopOnMiss;
                    break;
            }

            if (shouldStop)
            {
                HitStop.Instance.DoHitStopForZone(zone);
            }
        }

        private void HandleParticles(Hit.HitZone zone)
        {
            if (HitParticles.Instance != null)
            {
                HitParticles.Instance.PlayAtNeedle(zone);
            }
        }

        private void HandleHaptics(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core:
                    HapticFeedback.HeavyImpact();
                    break;
                case Hit.HitZone.Inner:
                    HapticFeedback.MediumImpact();
                    break;
                case Hit.HitZone.Middle:
                    HapticFeedback.LightImpact();
                    break;
                case Hit.HitZone.Miss:
                    HapticFeedback.Error();
                    break;
            }
        }

        private void OnSlowMoStart()
        {
            if (enableSlowMoAudio && Audio.AudioSlowMotion.Instance != null)
            {
                Audio.AudioSlowMotion.Instance.EnableSlowMotion();
            }
        }

        private void OnSlowMoEnd()
        {
            if (enableSlowMoAudio && Audio.AudioSlowMotion.Instance != null)
            {
                Audio.AudioSlowMotion.Instance.DisableSlowMotion();
            }
        }

        public void SetScreenShakeEnabled(bool enabled)
        {
            enableScreenShake = enabled;
        }

        public void SetHitStopEnabled(bool enabled)
        {
            enableHitStop = enabled;
        }

        public void SetParticlesEnabled(bool enabled)
        {
            enableParticles = enabled;
        }

        public void SetSlowMoAudioEnabled(bool enabled)
        {
            enableSlowMoAudio = enabled;
        }
    }
}
