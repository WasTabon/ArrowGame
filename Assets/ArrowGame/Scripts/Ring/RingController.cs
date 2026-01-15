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

        private ParticleSystem[] particles;
        
        private bool isActive = true;
        private bool isPassed = false;
        private Tween spawnTween;
        private Tween speedTween;
        private Tween moveSpeedTween;
        private BoxCollider triggerCollider;

        private float moveSpeed;
        private float currentMoveSpeedMultiplier = 1f;
        private float minX, maxX, minY, maxY;
        private float ringZ;
        private bool moveToNeedleNext = true;

        private Vector3 moveTargetPosition;
        private bool isMoving = false;

        public float RotationSpeed => rotationSpeed;
        public bool IsPassed => isPassed;

        public event System.Action<RingController> OnRingPassed;

        private void Awake()
        {
            SetupTrigger();
            
            particles = GetComponentsInChildren<ParticleSystem>(true);
            SetTrailActive(false);
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
            Move();
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * rotationDirection * Time.deltaTime);
        }

        private void Move()
        {
            if (!isMoving) return;

            float step = moveSpeed * currentMoveSpeedMultiplier * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, step);

            if (Vector3.Distance(transform.position, moveTargetPosition) < 0.01f)
            {
                SetNextMoveTarget();
            }
        }

        public void Initialize(float speed, int direction, RingConfig config)
        {
            rotationSpeed = speed;
            rotationDirection = direction;
            isActive = true;
            isPassed = false;
            currentMoveSpeedMultiplier = 1f;

            minX = config.spawnMinX;
            maxX = config.spawnMaxX;
            minY = config.spawnMinY;
            maxY = config.spawnMaxY;

            moveSpeed = Random.Range(config.minMoveSpeed, config.maxMoveSpeed);
            ringZ = transform.position.z;
            moveToNeedleNext = true;

            PlaySpawnAnimation();
            SetNextMoveTarget();
            isMoving = true;
        }

        private void SetNextMoveTarget()
        {
            if (moveToNeedleNext)
            {
                moveTargetPosition = GetNeedleTargetPosition();
            }
            else
            {
                moveTargetPosition = GetRandomTargetPosition();
            }

            moveToNeedleNext = !moveToNeedleNext;
        }

        private void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            spawnTween = transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack);
        }

        public void SetTrailActive(bool active)
        {
            if (particles == null) return;
    
            foreach (var ps in particles)
            {
                if (ps != null)
                {
                    if (active)
                    {
                        ps.Play();
                    }
                    else
                    {
                        ps.Stop();
                    }
                }
            }
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

        public void SlowDownSmooth(float targetSpeed, float duration)
        {
            speedTween?.Kill();
            speedTween = DOTween.To(() => rotationSpeed, x => rotationSpeed = x, targetSpeed, duration)
                .SetEase(Ease.OutQuad);
        }

        public void RestoreSpeedSmooth(float targetSpeed, float duration)
        {
            speedTween?.Kill();
            speedTween = DOTween.To(() => rotationSpeed, x => rotationSpeed = x, targetSpeed, duration)
                .SetEase(Ease.OutQuad);
        }

        public void SetMoveSpeedMultiplier(float targetMultiplier, float duration)
        {
            moveSpeedTween?.Kill();
            moveSpeedTween = DOTween.To(() => currentMoveSpeedMultiplier, x => currentMoveSpeedMultiplier = x, targetMultiplier, duration)
                .SetEase(Ease.OutQuad);
        }

        public void MarkAsPassed()
        {
            if (isPassed) return;

            isPassed = true;
            isActive = false;
            isMoving = false;
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
            speedTween?.Kill();
            moveSpeedTween?.Kill();
            isActive = false;
            isMoving = false;
            transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void ResetRing()
        {
            spawnTween?.Kill();
            speedTween?.Kill();
            moveSpeedTween?.Kill();
            isActive = true;
            isPassed = false;
            isMoving = false;
            moveToNeedleNext = true;
            currentMoveSpeedMultiplier = 1f;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            spawnTween?.Kill();
            speedTween?.Kill();
            moveSpeedTween?.Kill();
        }
    }
}