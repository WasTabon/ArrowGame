using UnityEngine;

namespace ArrowGame.Achievements
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "ArrowGame/Achievement")]
    public class AchievementDefinition : ScriptableObject
    {
        [Header("Info")]
        public string id;
        public string title;
        [TextArea] public string description;
        public Sprite icon;
        public AchievementCategory category;

        [Header("Condition")]
        public AchievementConditionType conditionType;
        public int targetValue;

        [Header("Reward")]
        public string unlocksSkinId;

        [Header("Display")]
        public bool isHidden;
    }
}
