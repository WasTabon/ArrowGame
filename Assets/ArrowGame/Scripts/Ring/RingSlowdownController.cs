using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace ArrowGame.Ring
{
    public class RingSlowdownController : MonoBehaviour
    {
        public static RingSlowdownController Instance { get; private set; }

        [Header("Slowdown Strength")]
        [SerializeField] private float minSpeedMultiplier = 0.05f;
        [SerializeField] private float minMoveSpeedMultiplier = 0.1f;

        [Header("Timing")]
        [SerializeField] private float slowdownDuration = 0.3f;
        [SerializeField] private float recoveryDuration = 0.3f;

        private Dictionary<RingController, float> originalSpeeds = new Dictionary<RingController, float>();
        private bool isSlowingDown = false;

        public bool IsSlowingDown => isSlowingDown;

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
            SubscribeToInput();
        }

        private void OnEnable()
        {
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            if (Input.InputController.Instance != null)
            {
                Input.InputController.Instance.OnInputStart += HandleInputStart;
                Input.InputController.Instance.OnInputHold += HandleInputHold;
                Input.InputController.Instance.OnInputEnd += HandleInputEnd;
            }
        }

        private void UnsubscribeFromInput()
        {
            if (Input.InputController.Instance != null)
            {
                Input.InputController.Instance.OnInputStart -= HandleInputStart;
                Input.InputController.Instance.OnInputHold -= HandleInputHold;
                Input.InputController.Instance.OnInputEnd -= HandleInputEnd;
            }
        }

        private void HandleInputStart()
        {
            isSlowingDown = true;
            originalSpeeds.Clear();

            if (RingSpawner.Instance == null) return;

            foreach (var ring in RingSpawner.Instance.ActiveRings)
            {
                SlowDownRing(ring);
            }
        }

        private void HandleInputHold(float holdDuration)
        {
            if (!isSlowingDown) return;
            if (RingSpawner.Instance == null) return;

            foreach (var ring in RingSpawner.Instance.ActiveRings)
            {
                if (!originalSpeeds.ContainsKey(ring))
                {
                    SlowDownRing(ring);
                }
            }
        }

        private void HandleInputEnd()
        {
            isSlowingDown = false;

            if (RingSpawner.Instance == null) return;

            foreach (var ring in RingSpawner.Instance.ActiveRings)
            {
                if (ring == null || ring.IsPassed) continue;

                ring.SetTrailActive(false);
                ring.SetMoveSpeedMultiplier(1f, recoveryDuration);

                if (originalSpeeds.ContainsKey(ring))
                {
                    ring.RestoreSpeedSmooth(originalSpeeds[ring], recoveryDuration);
                }
            }

            originalSpeeds.Clear();
        }

        private void SlowDownRing(RingController ring)
        {
            if (ring == null || ring.IsPassed) return;

            originalSpeeds[ring] = ring.RotationSpeed;
            
            float targetSpeed = originalSpeeds[ring] * minSpeedMultiplier;
            ring.SlowDownSmooth(targetSpeed, slowdownDuration);
            ring.SetMoveSpeedMultiplier(minMoveSpeedMultiplier, slowdownDuration);
            ring.SetTrailActive(true);
        }
    }
}