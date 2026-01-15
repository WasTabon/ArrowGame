using UnityEngine;

namespace ArrowGame.GameOver
{
    [CreateAssetMenu(fileName = "GameOverConfig", menuName = "ArrowGame/Game Over Config")]
    public class GameOverConfig : ScriptableObject
    {
        [Header("Time Slow")]
        public float slowMotionTimeScale = 0.2f;
        public float slowMotionDuration = 1.5f;
        public float timeScaleRecoveryDuration = 0.5f;

        [Header("Camera")]
        public float zoomOutAmount = 2f;
        public float zoomDuration = 1f;

        [Header("Screen")]
        public float fadeToBlackDuration = 0.5f;
        public float delayBeforeGameOverScreen = 0.5f;
    }
}
