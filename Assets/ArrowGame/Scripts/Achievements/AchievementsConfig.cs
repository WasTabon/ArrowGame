using UnityEngine;
using System.Collections.Generic;

namespace ArrowGame.Achievements
{
    [CreateAssetMenu(fileName = "AchievementsConfig", menuName = "ArrowGame/Achievements Config")]
    public class AchievementsConfig : ScriptableObject
    {
        public List<AchievementDefinition> achievements = new List<AchievementDefinition>();

        public AchievementDefinition GetById(string id)
        {
            return achievements.Find(a => a.id == id);
        }

        public List<AchievementDefinition> GetByCategory(AchievementCategory category)
        {
            return achievements.FindAll(a => a.category == category);
        }

        public List<AchievementDefinition> GetByConditionType(AchievementConditionType type)
        {
            return achievements.FindAll(a => a.conditionType == type);
        }
    }
}
