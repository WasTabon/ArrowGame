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
        private Tween moveTween;
        private BoxCollider triggerCollider;

        private float moveSpeed;
        private float minX, maxX, minY, maxY;
        private float pauseDurationMin, pauseDurationMax;
        private float ringZ;
        private bool moveToNeedleNext = true;

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

        public void Initialize(float speed, int direction, RingConfig config)
        {
            rotationSpeed = speed;
            rotationDirection = direction;
            isActive = true;
            isPassed = false;

            minX = config.spawnMinX;
            maxX = config.spawnMaxX;
            minY = config.spawnMinY;
            maxY = config.spawnMaxY;
            pauseDurationMin = config.movePauseDurationMin;
            pauseDurationMax = config.movePauseDurationMax;

            moveSpeed = Random.Range(config.minMoveSpeed, config.maxMoveSpeed);
            ringZ = transform.position.z;
            moveToNeedleNext = true;

            PlaySpawnAnimation();
            StartMovement();
        }

        private void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            spawnTween = transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack);
        }

        private void StartMovement()
        {
            MoveToNextPoint();
        }

        private void MoveToNextPoint()
        {
            if (!isActive || isPassed) return;

            Vector3 targetPos;

            if (moveToNeedleNext)
            {
                targetPos = GetNeedleTargetPosition();
            }
            else
            {
                targetPos = GetRandomTargetPosition();
            }

            moveToNeedleNext = !moveToNeedleNext;

            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;

            if (duration < 0.1f) duration = 0.1f;

            moveTween = transform.DOMove(targetPos, duration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (!isActive || isPassed) return;

                    float pause = Random.Range(pauseDurationMin, pauseDurationMax);
                    DOVirtual.DelayedCall(pause, MoveToNextPoint);
                });
        }

        private Vector3 GetNeedleTargetPosition()
        {
            var needle = Needle.NeedleController.Instance;
            if (needle != null)
            {
                Vector3 needlePos = needle.transform.position;
                return new Vector3(needlePos.x, needlePos.y, ringZ);
            }

            return GetRandomTargetPosition();
        }

        private Vector3 GetRandomTargetPosition()
        {
            float targetX = Random.Range(minX, maxX);
            float targetY = Random.Range(minY, maxY);
            return new Vector3(targetX, targetY, ringZ);
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
            moveTween?.Kill();
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
            moveTween?.Kill();
            isActive = false;
            transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void ResetRing()
        {
            spawnTween?.Kill();
            moveTween?.Kill();
            isActive = true;
            isPassed = false;
            moveToNeedleNext = true;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            spawnTween?.Kill();
            moveTween?.Kill();
        }
    }
}