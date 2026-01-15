using UnityEngine;
using System;

namespace ArrowGame.Hit
{
    public class HitDetector : MonoBehaviour
    {
        public static HitDetector Instance { get; private set; }

        [SerializeField] private HitZoneConfig config;

        public event Action<HitResult> OnHit;

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
            if (Ring.RingPassDetector.Instance != null)
            {
                Ring.RingPassDetector.Instance.OnNeedlePassedRing += HandleRingPassed;
                Ring.RingPassDetector.Instance.OnNeedleMissedRing += HandleRingMissed;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (Ring.RingPassDetector.Instance != null)
            {
                Ring.RingPassDetector.Instance.OnNeedlePassedRing -= HandleRingPassed;
                Ring.RingPassDetector.Instance.OnNeedleMissedRing -= HandleRingMissed;
            }
        }

        private void HandleRingPassed(Ring.RingController ring)
        {
            if (config == null)
            {
                Debug.LogError("HitZoneConfig not assigned to HitDetector!");
                return;
            }

            HitResult result = CalculateHit(ring);
            OnHit?.Invoke(result);
        }

        private void HandleRingMissed(Ring.RingController ring)
        {
            HitResult result = new HitResult
            {
                Zone = HitZone.Miss,
                Distance = float.MaxValue,
                HitPosition = Needle.NeedleController.Instance.transform.position,
                Ring = ring
            };

            OnHit?.Invoke(result);
        }

        private HitResult CalculateHit(Ring.RingController ring)
        {
            Vector3 needlePos = Needle.NeedleController.Instance.transform.position;
            Vector3 ringPos = ring.transform.position;

            float distanceX = needlePos.x - ringPos.x;
            float distanceY = needlePos.y - ringPos.y;
            float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

            HitZone zone = config.GetZoneByDistance(distance);

            return new HitResult
            {
                Zone = zone,
                Distance = distance,
                HitPosition = needlePos,
                Ring = ring
            };
        }
    }
}