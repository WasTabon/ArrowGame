using UnityEngine;
using System;
using System.Collections.Generic;

namespace ArrowGame.Achievements
{
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [SerializeField] private AchievementsConfig config;

        private HashSet<string> unlockedIds = new HashSet<string>();
        private Dictionary<string, int> progressData = new Dictionary<string, int>();

        private const string UNLOCKED_KEY = "ArrowGame_UnlockedAchievements";
        private const string PROGRESS_KEY = "ArrowGame_AchievementProgress";

        public event Action<AchievementDefinition> OnAchievementUnlocked;

        public AchievementsConfig Config => config;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("AchievementsConfig not assigned to AchievementManager!");
                return;
            }

            LoadData();
        }

        public bool IsUnlocked(string achievementId)
        {
            return unlockedIds.Contains(achievementId);
        }

        public int GetProgress(string achievementId)
        {
            return progressData.TryGetValue(achievementId, out int val) ? val : 0;
        }

        public float GetProgressNormalized(string achievementId)
        {
            var def = config.GetById(achievementId);
            if (def == null || def.targetValue <= 0) return 0f;
            return Mathf.Clamp01((float)GetProgress(achievementId) / def.targetValue);
        }

        public void SetProgress(string achievementId, int value)
        {
            if (IsUnlocked(achievementId)) return;

            var def = config.GetById(achievementId);
            if (def == null) return;

            progressData[achievementId] = value;

            if (value >= def.targetValue)
            {
                Unlock(achievementId);
            }

            SaveData();
        }

        public void AddProgress(string achievementId, int amount = 1)
        {
            int current = GetProgress(achievementId);
            SetProgress(achievementId, current + amount);
        }

        public void ReportValue(AchievementConditionType type, int value, bool additive = false)
        {
            var list = config.GetByConditionType(type);
            foreach (var def in list)
            {
                if (IsUnlocked(def.id)) continue;

                if (additive)
                {
                    AddProgress(def.id, value);
                }
                else
                {
                    int current = GetProgress(def.id);
                    if (value > current)
                    {
                        SetProgress(def.id, value);
                    }
                }
            }
        }

        private void Unlock(string achievementId)
        {
            if (unlockedIds.Contains(achievementId)) return;

            var def = config.GetById(achievementId);
            if (def == null) return;

            unlockedIds.Add(achievementId);
            progressData[achievementId] = def.targetValue;

            if (!string.IsNullOrEmpty(def.unlocksSkinId))
            {
                UnlockSkin(def.unlocksSkinId);
            }

            SaveData();

            Debug.Log($"[Achievement] Unlocked: {def.title}");
            OnAchievementUnlocked?.Invoke(def);
        }

        private void UnlockSkin(string skinId)
        {
            string key = $"ArrowGame_Skin_{skinId}";
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        public bool IsSkinUnlocked(string skinId)
        {
            if (string.IsNullOrEmpty(skinId)) return false;
            string key = $"ArrowGame_Skin_{skinId}";
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public int GetUnlockedCount()
        {
            return unlockedIds.Count;
        }

        public int GetTotalCount()
        {
            return config.achievements.Count;
        }

        public List<AchievementDefinition> GetAllAchievements()
        {
            return config.achievements;
        }

        public List<AchievementDefinition> GetUnlocked()
        {
            return config.achievements.FindAll(a => IsUnlocked(a.id));
        }

        public List<AchievementDefinition> GetLocked()
        {
            return config.achievements.FindAll(a => !IsUnlocked(a.id));
        }

        private void LoadData()
        {
            string unlockedStr = PlayerPrefs.GetString(UNLOCKED_KEY, "");
            if (!string.IsNullOrEmpty(unlockedStr))
            {
                string[] ids = unlockedStr.Split(',');
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id))
                        unlockedIds.Add(id);
                }
            }

            string progressStr = PlayerPrefs.GetString(PROGRESS_KEY, "");
            if (!string.IsNullOrEmpty(progressStr))
            {
                string[] pairs = progressStr.Split(';');
                foreach (var pair in pairs)
                {
                    string[] kv = pair.Split(':');
                    if (kv.Length == 2 && int.TryParse(kv[1], out int val))
                    {
                        progressData[kv[0]] = val;
                    }
                }
            }
        }

        private void SaveData()
        {
            string unlockedStr = string.Join(",", unlockedIds);
            PlayerPrefs.SetString(UNLOCKED_KEY, unlockedStr);

            List<string> pairs = new List<string>();
            foreach (var kvp in progressData)
            {
                pairs.Add($"{kvp.Key}:{kvp.Value}");
            }
            PlayerPrefs.SetString(PROGRESS_KEY, string.Join(";", pairs));

            PlayerPrefs.Save();
        }

        public void ResetAll()
        {
            unlockedIds.Clear();
            progressData.Clear();
            PlayerPrefs.DeleteKey(UNLOCKED_KEY);
            PlayerPrefs.DeleteKey(PROGRESS_KEY);
            PlayerPrefs.Save();
        }
    }
}
