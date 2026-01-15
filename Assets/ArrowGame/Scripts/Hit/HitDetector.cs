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
            Debug.Log($"HitDetector Start. RingPassDetector.Instance = {Ring.RingPassDetector.Instance}");
    
            if (Ring.RingPassDetector.Instance != null)
            {
                Ring.RingPassDetector.Instance.OnNeedlePassedRing += HandleRingPassed;
                Debug.Log("HitDetector subscribed to OnNeedlePassedRing");
            }
            else
            {
                Debug.LogError("RingPassDetector.Instance is NULL!");
            }
        }

        private void OnEnable()
        {
            if (Ring.RingPassDetector.Instance != null)
            {
                Ring.RingPassDetector.Instance.OnNeedlePassedRing += HandleRingPassed;
            }
        }

        private void OnDisable()
        {
            if (Ring.RingPassDetector.Instance != null)
            {
                Ring.RingPassDetector.Instance.OnNeedlePassedRing -= HandleRingPassed;
            }
        }

        private void HandleRingPassed(Ring.RingController ring)
        {
            Debug.Log($"HandleRingPassed called! Ring = {ring}");
            
            if (config == null)
            {
                Debug.LogError("HitZoneConfig not assigned to HitDetector!");
                return;
            }

            HitResult result = CalculateHit(ring);
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
