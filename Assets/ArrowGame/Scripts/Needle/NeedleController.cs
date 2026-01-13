using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Needle
{
    public class NeedleController : MonoBehaviour
    {
        public static NeedleController Instance { get; private set; }

        [SerializeField] private Core.GameConfig config;

        private float currentSpeed;
        private float baseY;
        private float baseX;
        private float timeOffset;

        public float CurrentSpeed => currentSpeed;
        public Vector3 Position => transform.position;

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
                Debug.LogError("GameConfig not assigned to NeedleController!");
                return;
            }

            currentSpeed = config.baseNeedleSpeed;
            baseX = transform.position.x;
            baseY = transform.position.y;
            timeOffset = Random.Range(0f, 100f);
        }

        private void Update()
        {
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            MoveForward();
            ApplyFloatingOffset();
        }

        private void MoveForward()
        {
            Vector3 pos = transform.position;
            pos.z += currentSpeed * Time.deltaTime;
            transform.position = pos;
        }

        private void ApplyFloatingOffset()
        {
            float time = Time.time + timeOffset;

            float offsetX = Mathf.Sin(time * config.floatFrequencyX) * config.floatAmplitudeX;
            float offsetY = Mathf.Sin(time * config.floatFrequencyY) * config.floatAmplitudeY;

            Vector3 pos = transform.position;
            pos.x = baseX + offsetX;
            pos.y = baseY + offsetY;
            transform.position = pos;
        }

        public void SetSpeed(float newSpeed)
        {
            float targetSpeed = Mathf.Clamp(newSpeed, config.minNeedleSpeed, config.maxNeedleSpeed);
            DOTween.To(() => currentSpeed, x => currentSpeed = x, targetSpeed, 0.3f)
                .SetEase(Ease.OutQuad);
        }

        public void AddSpeed(float amount)
        {
            SetSpeed(currentSpeed + amount);
        }

        public void ResetNeedle()
        {
            transform.position = new Vector3(baseX, baseY, 0f);
            currentSpeed = config.baseNeedleSpeed;
        }
    }
}
