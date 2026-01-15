using UnityEngine;

namespace ArrowGame.Ring
{
    public class RingPassDetector : MonoBehaviour
    {
        public static RingPassDetector Instance { get; private set; }

        public event System.Action<RingController> OnNeedlePassedRing;

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
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"OnTriggerEnter! other = {other.name}, parent = {other.transform.parent?.name}");
    
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            RingController ring = other.GetComponentInParent<RingController>();
            Debug.Log($"RingController = {ring}");
    
            if (ring != null && !ring.IsPassed)
            {
                OnPassedThroughRing(ring);
            }
        }

        private void OnPassedThroughRing(RingController ring)
        {
            ring.MarkAsPassed();
            OnNeedlePassedRing?.Invoke(ring);
        }
    }
}