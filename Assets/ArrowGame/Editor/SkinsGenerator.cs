using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ArrowGame.Editor
{
    public class SkinsGenerator : EditorWindow
    {
        [MenuItem("ArrowGame/Generate Default Skins")]
        public static void GenerateSkins()
        {
            string folderPath = "Assets/ScriptableObjects/Skins";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var skinsData = GetDefaultSkins();
            var config = ScriptableObject.CreateInstance<Skins.SkinsConfig>();
            config.skins = new List<Skins.NeedleSkinDefinition>();

            foreach (var data in skinsData)
            {
                var skin = ScriptableObject.CreateInstance<Skins.NeedleSkinDefinition>();
                skin.id = data.id;
                skin.displayName = data.displayName;
                skin.description = data.description;
                skin.unlockedByDefault = data.unlockedByDefault;
                skin.unlockedByAchievementId = data.achievementId;
                skin.hasGlow = data.hasGlow;
                skin.glowColor = data.glowColor;
                skin.glowIntensity = data.glowIntensity;

                string assetPath = $"{folderPath}/{data.id}.asset";
                AssetDatabase.CreateAsset(skin, assetPath);
                config.skins.Add(skin);

                if (data.unlockedByDefault && config.defaultSkin == null)
                {
                    config.defaultSkin = skin;
                }
            }

            string configPath = $"{folderPath}/SkinsConfig.asset";
            AssetDatabase.CreateAsset(config, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;

            Debug.Log($"Created {skinsData.Count} skins at {folderPath}");
        }

        private static List<SkinTemplate> GetDefaultSkins()
        {
            return new List<SkinTemplate>
            {
                new SkinTemplate
                {
                    id = "standard",
                    displayName = "Standard",
                    description = "The classic needle. Simple and effective.",
                    unlockedByDefault = true,
                    hasGlow = false,
                },
                new SkinTemplate
                {
                    id = "pulse",
                    displayName = "Pulse",
                    description = "A needle with a pulsating glow effect.",
                    unlockedByDefault = false,
                    achievementId = "core_5",
                    hasGlow = true,
                    glowColor = new Color(0.3f, 0.7f, 1f),
                    glowIntensity = 1.5f,
                },
                new SkinTemplate
                {
                    id = "razor",
                    displayName = "Razor",
                    description = "Sleek and sharp. For precision masters.",
                    unlockedByDefault = false,
                    achievementId = "perfect_run",
                    hasGlow = true,
                    glowColor = new Color(1f, 0.3f, 0.3f),
                    glowIntensity = 1.2f,
                },
                new SkinTemplate
                {
                    id = "phase",
                    displayName = "Phase",
                    description = "Semi-transparent with a ghostly trail.",
                    unlockedByDefault = false,
                    achievementId = "streak_50",
                    hasGlow = true,
                    glowColor = new Color(0.7f, 0.3f, 1f),
                    glowIntensity = 1.8f,
                },
                new SkinTemplate
                {
                    id = "void",
                    displayName = "Void",
                    description = "The ultimate prestige. Darkness incarnate.",
                    unlockedByDefault = false,
                    achievementId = "rings_2000",
                    hasGlow = true,
                    glowColor = new Color(0.1f, 0f, 0.2f),
                    glowIntensity = 2f,
                }
            };
        }

        private class SkinTemplate
        {
            public string id;
            public string displayName;
            public string description;
            public bool unlockedByDefault;
            public string achievementId = "";
            public bool hasGlow;
            public Color glowColor = Color.white;
            public float glowIntensity = 1f;
        }
    }
}
