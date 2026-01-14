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

        [Header("Spawn Area")]
        public float spawnMinX = -2f;
        public float spawnMaxX = 2f;
        public float spawnMinY = -1f;
        public float spawnMaxY = 2f;

        [Header("Ring Movement")]
        public float minMoveSpeed = 0.5f;
        public float maxMoveSpeed = 2f;
        public float movePauseDurationMin = 0.2f;
        public float movePauseDurationMax = 0.8f;

        [Header("Visual Settings")]
        public float ringScale = 3f;
        public float spawnAnimationDuration = 0.3f;
    }
}