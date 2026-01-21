using UnityEngine;
using System.Collections.Generic;

namespace ArrowGame.Optimization
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
            public bool expandable = true;
        }

        [SerializeField] private List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, Pool> poolConfigs;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePools();
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            poolConfigs = new Dictionary<string, Pool>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                poolConfigs[pool.tag] = pool;

                GameObject container = new GameObject($"Pool_{pool.tag}");
                container.transform.SetParent(transform);

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = CreatePooledObject(pool.prefab, container.transform);
                    objectPool.Enqueue(obj);
                }

                poolDictionary[pool.tag] = objectPool;
            }
        }

        private GameObject CreatePooledObject(GameObject prefab, Transform parent)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);

            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnPoolCreate();

            return obj;
        }

        public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            Queue<GameObject> pool = poolDictionary[tag];
            GameObject obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (poolConfigs[tag].expandable)
            {
                Transform container = transform.Find($"Pool_{tag}");
                obj = CreatePooledObject(poolConfigs[tag].prefab, container);
            }
            else
            {
                return null;
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnPoolSpawn();

            return obj;
        }

        public void Despawn(string tag, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Destroy(obj);
                return;
            }

            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnPoolDespawn();

            obj.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }

        public void DespawnAll(string tag)
        {
            if (!poolDictionary.ContainsKey(tag)) return;

            Transform container = transform.Find($"Pool_{tag}");
            if (container == null) return;

            foreach (Transform child in container)
            {
                if (child.gameObject.activeSelf)
                {
                    Despawn(tag, child.gameObject);
                }
            }
        }
    }

    public interface IPoolable
    {
        void OnPoolCreate();
        void OnPoolSpawn();
        void OnPoolDespawn();
    }
}
