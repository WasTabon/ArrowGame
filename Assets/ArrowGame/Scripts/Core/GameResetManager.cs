using UnityEngine;
using System;

namespace ArrowGame.Core
{
    public class GameResetManager : MonoBehaviour
    {
        public static GameResetManager Instance { get; private set; }

        public event Action OnGameReset;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ResetAllData()
        {
            if (Lives.LivesManager.Instance != null)
            {
                Lives.LivesManager.Instance.ResetAllData();
            }

            if (Achievements.AchievementManager.Instance != null)
            {
                Achievements.AchievementManager.Instance.ResetAll();
            }

            if (Stats.StatisticsManager.Instance != null)
            {
                Stats.StatisticsManager.Instance.ResetStats();
            }

            if (Tutorial.TutorialManager.Instance != null)
            {
                Tutorial.TutorialManager.Instance.ResetTutorial();
            }

            PlayerPrefs.DeleteKey("ArrowGame_SelectedSkin");

            var skinKeys = new string[] { "standard", "pulse", "razor", "phase", "void" };
            foreach (var key in skinKeys)
            {
                PlayerPrefs.DeleteKey($"ArrowGame_Skin_{key}");
            }

            PlayerPrefs.Save();

            OnGameReset?.Invoke();

            Debug.Log("[GameReset] All data reset");
        }

        public void ResetProgressOnly()
        {
            if (Achievements.AchievementManager.Instance != null)
            {
                Achievements.AchievementManager.Instance.ResetAll();
            }

            if (Stats.StatisticsManager.Instance != null)
            {
                Stats.StatisticsManager.Instance.ResetStats();
            }

            PlayerPrefs.Save();

            Debug.Log("[GameReset] Progress reset");
        }
    }
}
