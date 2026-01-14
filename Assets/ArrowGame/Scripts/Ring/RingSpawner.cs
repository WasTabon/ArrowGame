using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Ring
{
    public class RingSpawner : MonoBehaviour
    {
        public static RingSpawner Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject ringPrefab;
        [SerializeField] private RingConfig config;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 10;

        [Header("Spawn Position Z")]
        [SerializeField] private float spawnZOffset = 0f;

        private Queue<GameObject> ringPool = new Queue<GameObject>();
        private List<RingController> activeRings = new List<RingController>();

        private float lastSpawnedZ = 0f;
        private float nextSpawnDistance;
        private int ringsSpawned = 0;

        private Transform needleTransform;

        public List<RingController> ActiveRings => activeRings;
        public RingConfig Config => config;

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
            if (ringPrefab == null)
            {
                Debug.LogError("Ring Prefab not assigned to RingSpawner!");
                return;
            }

            if (config == null)
            {
                Debug.LogError("RingConfig not assigned to RingSpawner!");
                return;
            }

            InitializePool();
            CalculateNextSpawnDistance();
        }

        private void Update()
        {
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            if (needleTransform == null)
            {
                var needle = Needle.NeedleController.Instance;
                if (needle != null)
                    needleTransform = needle.transform;
                return;
            }

            CheckAndSpawnRings();
            CheckAndRecycleRings();
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject ring = Instantiate(ringPrefab, transform);
                ring.SetActive(false);
                ringPool.Enqueue(ring);
            }
        }

        private void CheckAndSpawnRings()
        {
            float needleZ = needleTransform.position.z;
            float spawnThreshold = needleZ + config.spawnDistanceAhead;

            while (lastSpawnedZ < spawnThreshold)
            {
                SpawnRing();
            }
        }

        private void SpawnRing()
        {
            GameObject ringObj;

            if (ringPool.Count > 0)
            {
                ringObj = ringPool.Dequeue();
            }
            else
            {
                ringObj = Instantiate(ringPrefab, transform);
            }

            lastSpawnedZ += nextSpawnDistance;
            ringsSpawned++;

            float spawnX = Random.Range(config.spawnMinX, config.spawnMaxX);
            float spawnY = Random.Range(config.spawnMinY, config.spawnMaxY);
            Vector3 position = new Vector3(spawnX, spawnY, lastSpawnedZ + spawnZOffset);

            ringObj.transform.position = position;
            ringObj.transform.rotation = Quaternion.identity;
            ringObj.transform.localScale = Vector3.one * config.ringScale;

            ringObj.SetActive(true);

            RingController controller = ringObj.GetComponent<RingController>();
            if (controller != null)
            {
                float speed = CalculateRotationSpeed();
                int direction = Random.value > 0.5f ? 1 : -1;
                controller.Initialize(speed, direction, config);
                activeRings.Add(controller);
            }

            CalculateNextSpawnDistance();
        }

        private float CalculateRotationSpeed()
        {
            float speed = config.baseRotationSpeed + (ringsSpawned * config.rotationSpeedIncreasePerRing);
            return Mathf.Min(speed, config.maxRotationSpeed);
        }

        private void CalculateNextSpawnDistance()
        {
            nextSpawnDistance = Random.Range(config.minDistanceBetweenRings, config.maxDistanceBetweenRings);
        }

        private void CheckAndRecycleRings()
        {
            float needleZ = needleTransform.position.z;
            float recycleThreshold = needleZ - 20f;

            for (int i = activeRings.Count - 1; i >= 0; i--)
            {
                RingController ring = activeRings[i];
                if (ring.transform.position.z < recycleThreshold)
                {
                    RecycleRing(ring);
                }
            }
        }

        private void RecycleRing(RingController ring)
        {
            activeRings.Remove(ring);
            ring.ResetRing();
            ring.gameObject.SetActive(false);
            ringPool.Enqueue(ring.gameObject);
        }

        public RingController GetNextRing()
        {
            if (needleTransform == null) return null;

            float needleZ = needleTransform.position.z;
            RingController closest = null;
            float closestDistance = float.MaxValue;

            foreach (var ring in activeRings)
            {
                if (ring.IsPassed) continue;

                float distance = ring.transform.position.z - needleZ;
                if (distance > 0 && distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = ring;
                }
            }

            return closest;
        }

        public void ResetSpawner()
        {
            foreach (var ring in activeRings)
            {
                ring.ResetRing();
                ring.gameObject.SetActive(false);
                ringPool.Enqueue(ring.gameObject);
            }
            activeRings.Clear();

            lastSpawnedZ = 0f;
            ringsSpawned = 0;
            CalculateNextSpawnDistance();
        }

        private void OnDrawGizmos()
        {
            if (config == null) return;

            Gizmos.color = Color.cyan;

            float centerX = (config.spawnMinX + config.spawnMaxX) / 2f;
            float centerY = (config.spawnMinY + config.spawnMaxY) / 2f;
            float width = config.spawnMaxX - config.spawnMinX;
            float height = config.spawnMaxY - config.spawnMinY;

            float z = 0f;
            if (needleTransform != null)
            {
                z = needleTransform.position.z + 30f;
            }

            Vector3 center = new Vector3(centerX, centerY, z);
            Vector3 size = new Vector3(width, height, 0.1f);

            Gizmos.DrawWireCube(center, size);

            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.DrawCube(center, size);
        }
    }
}