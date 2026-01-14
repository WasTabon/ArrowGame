using UnityEngine;

namespace ArrowGame.Ring
{
    [CreateAssetMenu(fileName = "RingConfig", menuName = "ArrowGame/Ring Config")]
    public class RingConfig : ScriptableObject
    {
        [Header("Rotation Settings")]
        public float baseRotationSpeed = 90f;
        public float maxRotationSpeed = 360f;
        public float rotationSpeedIncreasePerRing = 2f;

        [Header("Spawn Settings")]
        public float spawnDistanceAhead = 50f;
        public float minDistanceBetweenRings = 15f;
        public float maxDistanceBetweenRings = 25f;

        [Header("Visual Settings")]
        public float ringScale = 3f;
        public float spawnAnimationDuration = 0.3f;
    }
}
