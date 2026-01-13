using UnityEngine;

namespace ArrowGame.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ArrowGame/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Needle Settings")]
        public float baseNeedleSpeed = 10f;
        public float minNeedleSpeed = 0f;
        public float maxNeedleSpeed = 30f;

        [Header("Needle Float Settings")]
        public float floatAmplitudeX = 0.3f;
        public float floatAmplitudeY = 0.2f;
        public float floatFrequencyX = 1.5f;
        public float floatFrequencyY = 2f;

        [Header("Camera Settings")]
        public Vector3 cameraOffset = new Vector3(0f, 2f, -8f);
        public float cameraSmoothSpeed = 5f;
        public float cameraLookAhead = 3f;
    }
}
