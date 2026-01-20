using UnityEngine;
using System;
using System.Collections.Generic;

namespace ArrowGame.Skins
{
    public class SkinManager : MonoBehaviour
    {
        public static SkinManager Instance { get; private set; }

        [SerializeField] private SkinsConfig config;

        private string selectedSkinId;
        private NeedleSkinDefinition currentSkin;
    
        private const string SELECTED_SKIN_KEY = "ArrowGame_SelectedSkin";

        public event Action<NeedleSkinDefinition> OnSkinChanged;

        public NeedleSkinDefinition CurrentSkin => currentSkin;
        public SkinsConfig Config => config;

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
                Debug.LogError("SkinsConfig not assigned to SkinManager!");
                return;
            }

            LoadSelectedSkin();
        }

        public bool IsSkinUnlocked(string skinId)
        {
            var skin = config.GetById(skinId);
            if (skin == null) return false;

            if (skin.unlockedByDefault) return true;

            if (!string.IsNullOrEmpty(skin.unlockedByAchievementId))
            {
                if (Achievements.AchievementManager.Instance != null)
                {
                    return Achievements.AchievementManager.Instance.IsSkinUnlocked(skinId);
                }
            }

            string key = $"ArrowGame_Skin_{skinId}";
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public bool SelectSkin(string skinId)
        {
            if (!IsSkinUnlocked(skinId))
            {
                Debug.Log($"Skin {skinId} is locked!");
                return false;
            }

            var skin = config.GetById(skinId);
            if (skin == null)
            {
                Debug.LogError($"Skin {skinId} not found!");
                return false;
            }

            selectedSkinId = skinId;
            currentSkin = skin;

            PlayerPrefs.SetString(SELECTED_SKIN_KEY, skinId);
            PlayerPrefs.Save();

            OnSkinChanged?.Invoke(currentSkin);
            return true;
        }

        public List<NeedleSkinDefinition> GetAllSkins()
        {
            return config.skins;
        }

        public List<NeedleSkinDefinition> GetUnlockedSkins()
        {
            return config.skins.FindAll(s => IsSkinUnlocked(s.id));
        }

        public List<NeedleSkinDefinition> GetLockedSkins()
        {
            return config.skins.FindAll(s => !IsSkinUnlocked(s.id));
        }

        public void UnlockSkin(string skinId)
        {
            string key = $"ArrowGame_Skin_{skinId}";
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        private void LoadSelectedSkin()
        {
            selectedSkinId = PlayerPrefs.GetString(SELECTED_SKIN_KEY, "");

            if (string.IsNullOrEmpty(selectedSkinId) || !IsSkinUnlocked(selectedSkinId))
            {
                currentSkin = config.GetDefault();
                selectedSkinId = currentSkin?.id ?? "";
            }
            else
            {
                currentSkin = config.GetById(selectedSkinId);
                if (currentSkin == null)
                {
                    currentSkin = config.GetDefault();
                    selectedSkinId = currentSkin?.id ?? "";
                }
            }
        }
    }
}
