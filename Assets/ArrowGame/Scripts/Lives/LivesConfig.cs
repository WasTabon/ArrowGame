using UnityEngine;

namespace ArrowGame.Lives
{
    [CreateAssetMenu(fileName = "LivesConfig", menuName = "ArrowGame/Lives Config")]
    public class LivesConfig : ScriptableObject
    {
        [Header("Lives Settings")]
        public int maxLives = 5;
        public int startingLives = 5;

        [Header("Regeneration")]
        public float regenerationTimeMinutes = 20f;

        public float RegenerationTimeSeconds => regenerationTimeMinutes * 60f;
    }
}
