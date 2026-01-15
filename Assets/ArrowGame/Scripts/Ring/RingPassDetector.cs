using UnityEngine;

namespace ArrowGame.Ring
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class RingPassDetector : MonoBehaviour
    {
        public static RingPassDetector Instance { get; private set; }

        public event System.Action<RingController> OnNeedlePassedRing;
        public event System.Action<RingController> OnNeedleMissedRing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupCollider();
        }

        private void SetupCollider()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            SphereCollider col = GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.3f;
        }

        private void Update()
        {
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            CheckMissedRings();
        }

        private void CheckMissedRings()
        {
            if (RingSpawner.Instance == null) return;

            float needleZ = transform.position.z;

            foreach (var ring in RingSpawner.Instance.ActiveRings)
            {
                if (ring == null || ring.IsPassed) continue;

                float ringZ = ring.transform.position.z;

                if (needleZ > ringZ + 1f)
                {
                    ring.MarkAsPassed();
                    OnNeedleMissedRing?.Invoke(ring);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            RingController ring = other.GetComponentInParent<RingController>();
            if (ring != null && !ring.IsPassed)
            {
                ring.MarkAsPassed();
                OnNeedlePassedRing?.Invoke(ring);
            }
        }
    }
}