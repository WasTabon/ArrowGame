using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Ring
{
    public class RingSlowdownController : MonoBehaviour
    {
        public static RingSlowdownController Instance { get; private set; }

        [Header("Slowdown Settings")]
        [SerializeField] private float slowdownRate = 2f;
        [SerializeField] private float minSpeedMultiplier = 0f;
        [SerializeField] private float speedRecoveryDuration = 0.3f;

        private RingController currentTargetRing;
        private float originalSpeed;
        private float currentMultiplier = 1f;
        private bool isSlowingDown = false;
        private Tween recoveryTween;

        public bool IsSlowingDown => isSlowingDown;
        public float CurrentMultiplier => currentMultiplier;

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

        private void Update()
        {
            UpdateTargetRing();
        }

        private void UpdateTargetRing()
        {
            if (RingSpawner.Instance == null) return;

            RingController nextRing = RingSpawner.Instance.GetNextRing();

            if (nextRing != currentTargetRing)
            {
                if (currentTargetRing != null && isSlowingDown)
                {
                    RestoreRingSpeed(currentTargetRing);
                }

                currentTargetRing = nextRing;

                if (currentTargetRing != null)
                {
                    originalSpeed = currentTargetRing.RotationSpeed;
                    currentMultiplier = 1f;
                }
            }
        }

        private void HandleInputStart()
        {
            recoveryTween?.Kill();
            isSlowingDown = true;
            currentMultiplier = 1f;

            if (currentTargetRing != null)
            {
                originalSpeed = currentTargetRing.RotationSpeed;
            }
        }

        private void HandleInputHold(float holdDuration)
        {
            if (currentTargetRing == null) return;
            if (!isSlowingDown) return;

            currentMultiplier -= slowdownRate * Time.deltaTime;
            currentMultiplier = Mathf.Max(currentMultiplier, minSpeedMultiplier);

            float newSpeed = originalSpeed * currentMultiplier;
            currentTargetRing.SetRotationSpeed(newSpeed);

            Debug.Log($"Slowdown: multiplier={currentMultiplier:F2}, speed={newSpeed:F1}, original={originalSpeed:F1}");
        }

        private void HandleInputEnd()
        {
            isSlowingDown = false;

            if (currentTargetRing != null)
            {
                RestoreRingSpeed(currentTargetRing);
            }
        }

        private void RestoreRingSpeed(RingController ring)
        {
            recoveryTween?.Kill();

            float targetSpeed = originalSpeed;

            recoveryTween = DOTween.To(
                () => ring.RotationSpeed,
                x => ring.SetRotationSpeed(x),
                targetSpeed,
                speedRecoveryDuration
            ).SetEase(Ease.OutQuad);
        }
    }
}
