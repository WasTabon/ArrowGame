using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Camera
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [SerializeField] private Core.GameConfig config;
        [SerializeField] private Transform target;

        private Vector3 velocity = Vector3.zero;

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
                Debug.LogError("GameConfig not assigned to CameraController!");
                return;
            }

            if (target == null)
            {
                var needle = Needle.NeedleController.Instance;
                if (needle != null)
                    target = needle.transform;
            }

            if (target != null)
            {
                Vector3 desiredPosition = target.position + config.cameraOffset;
                desiredPosition.z += config.cameraLookAhead;
                transform.position = desiredPosition;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            FollowTarget();
        }

        private void FollowTarget()
        {
            Vector3 desiredPosition = target.position + config.cameraOffset;
            desiredPosition.z = target.position.z + config.cameraOffset.z + config.cameraLookAhead;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                1f / config.cameraSmoothSpeed
            );
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void ShakeCamera(float duration, float strength, int vibrato = 10)
        {
            transform.DOShakePosition(duration, strength, vibrato)
                .SetUpdate(true);
        }

        public void ImpulseForward(float amount, float duration)
        {
            transform.DOPunchPosition(new Vector3(0, 0, amount), duration, 5, 0.5f)
                .SetUpdate(true);
        }
    }
}
