using ArrowGame.Needle;
using UnityEngine;

namespace ArrowGame.Feedback
{
    public class HitParticles : MonoBehaviour
    {
        public static HitParticles Instance { get; private set; }

        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem coreParticles;
        [SerializeField] private ParticleSystem innerParticles;
        [SerializeField] private ParticleSystem middleParticles;
        [SerializeField] private ParticleSystem outerParticles;
        [SerializeField] private ParticleSystem missParticles;

        [Header("Settings")]
        [SerializeField] private bool usePooling = true;
        [SerializeField] private int poolSize = 5;

        private ParticleSystem[] corePool;
        private ParticleSystem[] innerPool;
        private ParticleSystem[] middlePool;
        private ParticleSystem[] outerPool;
        private ParticleSystem[] missPool;

        private int coreIndex;
        private int innerIndex;
        private int middleIndex;
        private int outerIndex;
        private int missIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (usePooling)
            {
                InitializePools();
            }
        }

        private void InitializePools()
        {
            if (coreParticles != null) corePool = CreatePool(coreParticles, "CorePool");
            if (innerParticles != null) innerPool = CreatePool(innerParticles, "InnerPool");
            if (middleParticles != null) middlePool = CreatePool(middleParticles, "MiddlePool");
            if (outerParticles != null) outerPool = CreatePool(outerParticles, "OuterPool");
            if (missParticles != null) missPool = CreatePool(missParticles, "MissPool");
        }

        private ParticleSystem[] CreatePool(ParticleSystem prefab, string poolName)
        {
            GameObject poolParent = new GameObject(poolName);
            poolParent.transform.SetParent(transform);

            ParticleSystem[] pool = new ParticleSystem[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                ParticleSystem ps = Instantiate(prefab, poolParent.transform);
                ps.gameObject.SetActive(true);
                ps.Stop();
                pool[i] = ps;
            }

            return pool;
        }

        public void PlayForZone(Hit.HitZone zone, Vector3 position)
        {
            switch (zone)
            {
                case Hit.HitZone.Core:
                    PlayFromPool(corePool, ref coreIndex, position);
                    break;
                case Hit.HitZone.Inner:
                    PlayFromPool(innerPool, ref innerIndex, position);
                    break;
                case Hit.HitZone.Middle:
                    PlayFromPool(middlePool, ref middleIndex, position);
                    break;
                case Hit.HitZone.Outer:
                    PlayFromPool(outerPool, ref outerIndex, position);
                    break;
                case Hit.HitZone.Miss:
                    PlayFromPool(missPool, ref missIndex, position);
                    break;
            }
        }

        private void PlayFromPool(ParticleSystem[] pool, ref int index, Vector3 position)
        {
            if (pool == null || pool.Length == 0) return;

            ParticleSystem ps = pool[index];
            ps.transform.position = position;

            var main = ps.main;

            ps.Play();

            index = (index + 1) % pool.Length;
        }

        public void PlayAtNeedle(Hit.HitZone zone)
        {
            if (NeedleController.Instance != null)
            {
                PlayForZone(zone, NeedleController.Instance.transform.position);
            }
        }

        public void SetParticleColor(ParticleSystem ps, Color color)
        {
            if (ps == null) return;
            var main = ps.main;
            main.startColor = color;
        }
    }
}
