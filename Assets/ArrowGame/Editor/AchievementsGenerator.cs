using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ArrowGame.Editor
{
    public class AchievementsGenerator : EditorWindow
    {
        [MenuItem("ArrowGame/Generate Default Achievements")]
        public static void GenerateAchievements()
        {
            string folderPath = "Assets/ScriptableObjects/Achievements";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var achievements = GetDefaultAchievements();
            var config = ScriptableObject.CreateInstance<Achievements.AchievementsConfig>();
            config.achievements = new List<Achievements.AchievementDefinition>();

            foreach (var data in achievements)
            {
                var def = ScriptableObject.CreateInstance<Achievements.AchievementDefinition>();
                def.id = data.id;
                def.title = data.title;
                def.description = data.description;
                def.category = data.category;
                def.conditionType = data.conditionType;
                def.targetValue = data.targetValue;
                def.unlocksSkinId = data.unlocksSkinId;
                def.isHidden = data.isHidden;

                string assetPath = $"{folderPath}/{data.id}.asset";
                AssetDatabase.CreateAsset(def, assetPath);
                config.achievements.Add(def);
            }

            string configPath = $"{folderPath}/AchievementsConfig.asset";
            AssetDatabase.CreateAsset(config, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;

            Debug.Log($"Created {achievements.Count} achievements at {folderPath}");
        }

        private static List<AchievementTemplate> GetDefaultAchievements()
        {
            return new List<AchievementTemplate>
            {
                new AchievementTemplate
                {
                    id = "core_5",
                    title = "Sharpshooter",
                    description = "Hit Core 5 times in a row",
                    category = Achievements.AchievementCategory.Precision,
                    conditionType = Achievements.AchievementConditionType.CoreHitsInRow,
                    targetValue = 5,
                    unlocksSkinId = "pulse"
                },
                new AchievementTemplate
                {
                    id = "core_10",
                    title = "Bullseye Master",
                    description = "Hit Core 10 times in a row",
                    category = Achievements.AchievementCategory.Precision,
                    conditionType = Achievements.AchievementConditionType.CoreHitsInRow,
                    targetValue = 10
                },
                new AchievementTemplate
                {
                    id = "core_total_100",
                    title = "Core Collector",
                    description = "Hit Core 100 times total",
                    category = Achievements.AchievementCategory.Precision,
                    conditionType = Achievements.AchievementConditionType.CoreHitsTotal,
                    targetValue = 100
                },
                new AchievementTemplate
                {
                    id = "perfect_run",
                    title = "Perfectionist",
                    description = "Complete a run without missing (10+ rings)",
                    category = Achievements.AchievementCategory.Precision,
                    conditionType = Achievements.AchievementConditionType.PerfectRun,
                    targetValue = 1,
                    unlocksSkinId = "razor"
                },

                new AchievementTemplate
                {
                    id = "streak_25",
                    title = "On Fire",
                    description = "Reach a streak of 25",
                    category = Achievements.AchievementCategory.Flow,
                    conditionType = Achievements.AchievementConditionType.StreakReached,
                    targetValue = 25
                },
                new AchievementTemplate
                {
                    id = "streak_50",
                    title = "Unstoppable",
                    description = "Reach a streak of 50",
                    category = Achievements.AchievementCategory.Flow,
                    conditionType = Achievements.AchievementConditionType.StreakReached,
                    targetValue = 50,
                    unlocksSkinId = "phase"
                },
                new AchievementTemplate
                {
                    id = "multiplier_5",
                    title = "Maximum Power",
                    description = "Reach x5 multiplier",
                    category = Achievements.AchievementCategory.Flow,
                    conditionType = Achievements.AchievementConditionType.MaxMultiplier,
                    targetValue = 5
                },
                new AchievementTemplate
                {
                    id = "no_outer_30",
                    title = "Consistent",
                    description = "Pass 30 rings without hitting Outer",
                    category = Achievements.AchievementCategory.Flow,
                    conditionType = Achievements.AchievementConditionType.RingsWithoutOuter,
                    targetValue = 30
                },

                new AchievementTemplate
                {
                    id = "low_speed",
                    title = "Living on Edge",
                    description = "Survive at speed below 5",
                    category = Achievements.AchievementCategory.Risk,
                    conditionType = Achievements.AchievementConditionType.SurviveLowSpeed,
                    targetValue = 1
                },
                new AchievementTemplate
                {
                    id = "comeback",
                    title = "Comeback King",
                    description = "Recover 10 speed after dropping low",
                    category = Achievements.AchievementCategory.Risk,
                    conditionType = Achievements.AchievementConditionType.ComebackFromLowSpeed,
                    targetValue = 1
                },

                new AchievementTemplate
                {
                    id = "rings_500",
                    title = "Ring Runner",
                    description = "Pass 500 rings total",
                    category = Achievements.AchievementCategory.Endurance,
                    conditionType = Achievements.AchievementConditionType.TotalRingsPassed,
                    targetValue = 500
                },
                new AchievementTemplate
                {
                    id = "rings_2000",
                    title = "Ring Master",
                    description = "Pass 2000 rings total",
                    category = Achievements.AchievementCategory.Endurance,
                    conditionType = Achievements.AchievementConditionType.TotalRingsPassed,
                    targetValue = 2000,
                    unlocksSkinId = "void"
                },
                new AchievementTemplate
                {
                    id = "score_10000",
                    title = "Score Hunter",
                    description = "Reach 10,000 points in one run",
                    category = Achievements.AchievementCategory.Endurance,
                    conditionType = Achievements.AchievementConditionType.SingleRunScore,
                    targetValue = 10000
                },
                new AchievementTemplate
                {
                    id = "score_50000",
                    title = "High Scorer",
                    description = "Reach 50,000 points in one run",
                    category = Achievements.AchievementCategory.Endurance,
                    conditionType = Achievements.AchievementConditionType.SingleRunScore,
                    targetValue = 50000
                },
                new AchievementTemplate
                {
                    id = "games_10",
                    title = "Getting Started",
                    description = "Play 10 games",
                    category = Achievements.AchievementCategory.Endurance,
                    conditionType = Achievements.AchievementConditionType.GamesPlayed,
                    targetValue = 10
                },
                new AchievementTemplate
                {
                    id = "games_100",
                    title = "Dedicated",
                    description = "Play 100 games",
                    category = Achievements.AchievementCategory.Endurance,
                    conditionType = Achievements.AchievementConditionType.GamesPlayed,
                    targetValue = 100
                }
            };
        }

        private class AchievementTemplate
        {
            public string id;
            public string title;
            public string description;
            public Achievements.AchievementCategory category;
            public Achievements.AchievementConditionType conditionType;
            public int targetValue;
            public string unlocksSkinId = "";
            public bool isHidden = false;
        }
    }
}
