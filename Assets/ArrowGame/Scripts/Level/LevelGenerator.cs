using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        public static LevelGenerator Instance { get; private set; }

        [Header("Corridor Settings")]
        [SerializeField] private GameObject corridorPrefab;
        [SerializeField] private float segmentLength = 22f;
        [SerializeField] private Vector3 firstSegmentPosition = new Vector3(0.5f, -13.4f, 20.6f);

        [Header("Generation Settings")]
        [SerializeField] private int activeSegments = 20;
        [SerializeField] private int segmentsAhead = 15;
        [SerializeField] private int segmentsBehind = 5;

        private Queue<GameObject> segmentPool = new Queue<GameObject>();
        private List<GameObject> activeSegmentsList = new List<GameObject>();
        private float lastSpawnedZ;
        private float firstActiveZ;

        private Transform needleTransform;

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
            if (corridorPrefab == null)
            {
                Debug.LogError("Corridor Prefab not assigned to LevelGenerator!");
                return;
            }

            InitializePool();
            SpawnInitialSegments();
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

            CheckAndGenerateSegments();
        }

        private void InitializePool()
        {
            for (int i = 0; i < activeSegments; i++)
            {
                GameObject segment = Instantiate(corridorPrefab, transform);
                segment.SetActive(false);
                segmentPool.Enqueue(segment);
            }
        }

        private void SpawnInitialSegments()
        {
            lastSpawnedZ = firstSegmentPosition.z - segmentLength;
            firstActiveZ = firstSegmentPosition.z;

            for (int i = 0; i < activeSegments; i++)
            {
                SpawnSegment();
            }
        }

        private void SpawnSegment()
        {
            GameObject segment;

            if (segmentPool.Count > 0)
            {
                segment = segmentPool.Dequeue();
            }
            else
            {
                segment = Instantiate(corridorPrefab, transform);
            }

            lastSpawnedZ += segmentLength;

            Vector3 position = firstSegmentPosition;
            position.z = lastSpawnedZ;
            segment.transform.position = position;

            segment.SetActive(true);
            activeSegmentsList.Add(segment);
        }

        private void RecycleSegment(GameObject segment)
        {
            segment.SetActive(false);
            activeSegmentsList.Remove(segment);
            segmentPool.Enqueue(segment);
        }

        private void CheckAndGenerateSegments()
        {
            float needleZ = needleTransform.position.z;

            float aheadThreshold = needleZ + (segmentsAhead * segmentLength);
            while (lastSpawnedZ < aheadThreshold)
            {
                SpawnSegment();
            }

            float behindThreshold = needleZ - (segmentsBehind * segmentLength);
            for (int i = activeSegmentsList.Count - 1; i >= 0; i--)
            {
                if (activeSegmentsList[i].transform.position.z < behindThreshold)
                {
                    RecycleSegment(activeSegmentsList[i]);
                }
            }
        }

        public void ResetLevel()
        {
            foreach (var segment in activeSegmentsList)
            {
                segment.SetActive(false);
                segmentPool.Enqueue(segment);
            }
            activeSegmentsList.Clear();

            lastSpawnedZ = firstSegmentPosition.z - segmentLength;
            firstActiveZ = firstSegmentPosition.z;

            SpawnInitialSegments();
        }
    }
}
