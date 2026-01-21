using UnityEngine;
using System;

namespace ArrowGame.Stats
{
    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager Instance { get; private set; }

        private const string STATS_KEY = "ArrowGame_Statistics";

        [System.Serializable]
        public class PlayerStats
        {
            public int totalGamesPlayed;
            public int totalRingsPassed;
            public int totalCoreHits;
            public int totalInnerHits;
            public int totalMiddleHits;
            public int totalOuterHits;
            public int totalMisses;
            public long totalScore;
            public int highScore;
            public int longestStreak;
            public int highestMultiplier;
            public float totalPlayTimeSeconds;
            public int perfectRuns;
            public string firstPlayDate;
            public string lastPlayDate;
        }

        private PlayerStats stats;

        public PlayerStats Stats => stats;

        public event Action OnStatsUpdated;

        private float sessionStartTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadStats();
        }

        private void Start()
        {
            SubscribeToEvents();
            sessionStartTime = Time.realtimeSinceStartup;

            if (string.IsNullOrEmpty(stats.firstPlayDate))
            {
                stats.firstPlayDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += OnHit;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnStreakChanged += OnStreakChanged;
                Score.StreakManager.Instance.OnMultiplierChanged += OnMultiplierChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit -= OnHit;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnStreakChanged -= OnStreakChanged;
                Score.StreakManager.Instance.OnMultiplierChanged -= OnMultiplierChanged;
            }
        }

        private void OnGameStateChanged(Core.GameState state)
        {
            if (state == Core.GameState.Run)
            {
                stats.totalGamesPlayed++;
                stats.lastPlayDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else if (state == Core.GameState.GameOver || state == Core.GameState.Results)
            {
                OnRunEnded();
            }
        }

        private void OnHit(Hit.HitResult result)
        {
            stats.totalRingsPassed++;

            switch (result.Zone)
            {
                case Hit.HitZone.Core:
                    stats.totalCoreHits++;
                    break;
                case Hit.HitZone.Inner:
                    stats.totalInnerHits++;
                    break;
                case Hit.HitZone.Middle:
                    stats.totalMiddleHits++;
                    break;
                case Hit.HitZone.Outer:
                    stats.totalOuterHits++;
                    break;
                case Hit.HitZone.Miss:
                    stats.totalMisses++;
                    break;
            }

            OnStatsUpdated?.Invoke();
        }

        private void OnStreakChanged(int streak)
        {
            if (streak > stats.longestStreak)
            {
                stats.longestStreak = streak;
            }
        }

        private void OnMultiplierChanged(int multiplier)
        {
            if (multiplier > stats.highestMultiplier)
            {
                stats.highestMultiplier = multiplier;
            }
        }

        private void OnRunEnded()
        {
            if (Score.ScoreManager.Instance != null)
            {
                int score = Score.ScoreManager.Instance.CurrentScore;
                stats.totalScore += score;

                if (score > stats.highScore)
                {
                    stats.highScore = score;
                }
            }

            SaveStats();
            OnStatsUpdated?.Invoke();
        }

        public void RecordPerfectRun()
        {
            stats.perfectRuns++;
            SaveStats();
        }

        public float GetAccuracy()
        {
            int total = stats.totalRingsPassed;
            if (total == 0) return 0f;

            int hits = total - stats.totalMisses;
            return (float)hits / total * 100f;
        }

        public float GetCoreAccuracy()
        {
            int total = stats.totalRingsPassed - stats.totalMisses;
            if (total == 0) return 0f;

            return (float)stats.totalCoreHits / total * 100f;
        }

        public TimeSpan GetTotalPlayTime()
        {
            float currentSession = Time.realtimeSinceStartup - sessionStartTime;
            return TimeSpan.FromSeconds(stats.totalPlayTimeSeconds + currentSession);
        }

        private void LoadStats()
        {
            string json = PlayerPrefs.GetString(STATS_KEY, "");
            if (!string.IsNullOrEmpty(json))
            {
                stats = JsonUtility.FromJson<PlayerStats>(json);
            }
            else
            {
                stats = new PlayerStats();
            }
        }

        private void SaveStats()
        {
            stats.totalPlayTimeSeconds += Time.realtimeSinceStartup - sessionStartTime;
            sessionStartTime = Time.realtimeSinceStartup;

            string json = JsonUtility.ToJson(stats);
            PlayerPrefs.SetString(STATS_KEY, json);
            PlayerPrefs.Save();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                SaveStats();
            }
        }

        private void OnApplicationQuit()
        {
            SaveStats();
        }

        public void ResetStats()
        {
            stats = new PlayerStats();
            stats.firstPlayDate = DateTime.Now.ToString("yyyy-MM-dd");
            PlayerPrefs.DeleteKey(STATS_KEY);
            PlayerPrefs.Save();
            OnStatsUpdated?.Invoke();
        }
    }
}
