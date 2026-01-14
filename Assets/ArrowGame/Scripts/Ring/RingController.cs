using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Ring
{
    [RequireComponent(typeof(BoxCollider))]
    public class RingController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private int rotationDirection = 1;
        [SerializeField] private Vector3 triggerSize = new Vector3(5f, 5f, 1f);

        private bool isActive = true;
        private bool isPassed = false;
        private Tween spawnTween;
        private BoxCollider triggerCollider;

        public float RotationSpeed => rotationSpeed;
        public bool IsPassed => isPassed;

        public event System.Action<RingController> OnRingPassed;

        private void Awake()
        {
            SetupTrigger();
        }

        private void SetupTrigger()
        {
            triggerCollider = GetComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = triggerSize;
        }

        private void Update()
        {
            if (!isActive || isPassed) return;
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            Rotate();
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * rotationDirection * Time.deltaTime);
        }

        public void Initialize(float speed, int direction)
        {
            rotationSpeed = speed;
            rotationDirection = direction;
            isActive = true;
            isPassed = false;

            PlaySpawnAnimation();
        }

        private void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            spawnTween = transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack);
        }

        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        public void SlowDown(float multiplier)
        {
            rotationSpeed *= multiplier;
        }

        public void StopRotation()
        {
            rotationSpeed = 0f;
        }

        public void ResumeRotation(float speed)
        {
            DOTween.To(() => rotationSpeed, x => rotationSpeed = x, speed, 0.2f)
                .SetEase(Ease.OutQuad);
        }

        public void MarkAsPassed()
        {
            if (isPassed) return;

            isPassed = true;
            isActive = false;
            OnRingPassed?.Invoke(this);

            PlayPassedAnimation();
        }

        private void PlayPassedAnimation()
        {
            transform.DOScale(Vector3.one * 1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, 0.1f)
                        .SetEase(Ease.InQuad);
                });
        }

        public void Deactivate()
        {
            spawnTween?.Kill();
            isActive = false;
            transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void ResetRing()
        {
            spawnTween?.Kill();
            isActive = true;
            isPassed = false;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            spawnTween?.Kill();
        }
    }
}