using UnityEngine;
using System;

namespace ArrowGame.Score
{
    public class StreakManager : MonoBehaviour
    {
        public static StreakManager Instance { get; private set; }

        [SerializeField] private ScoreConfig config;

        private int currentStreak;
        private int bestStreak;
        private int currentMultiplier = 1;

        private const string BEST_STREAK_KEY = "ArrowGame_BestStreak";

        public int CurrentStreak => currentStreak;
        public int BestStreak => bestStreak;
        public int CurrentMultiplier => currentMultiplier;
        public float StreakIntensity => config != null ? config.GetStreakIntensity(currentStreak) : 0f;

        public event Action<int> OnStreakChanged;
        public event Action<int> OnMultiplierChanged;
        public event Action OnStreakBroken;
        public event Action<int> OnBestStreakBeaten;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("ScoreConfig not assigned to StreakManager!");
                return;
            }

            LoadBestStreak();

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += HandleHit;
            }

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnEnable()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += HandleHit;
            }

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit -= HandleHit;
            }

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(Core.GameState state)
        {
            if (state == Core.GameState.Run)
            {
                ResetStreak();
            }
            else if (state == Core.GameState.GameOver)
            {
                CheckBestStreak();
            }
        }

        private void HandleHit(Hit.HitResult result)
        {
            switch (result.Zone)
            {
                case Hit.HitZone.Core:
                case Hit.HitZone.Inner:
                case Hit.HitZone.Middle:
                    IncreaseStreak();
                    break;

                case Hit.HitZone.Outer:
                    if (config.outerBreaksStreak)
                    {
                        BreakStreak();
                    }
                    else if (config.outerWeakensStreak)
                    {
                        WeakenStreak(config.outerStreakPenalty);
                    }
                    break;

                case Hit.HitZone.Miss:
                    BreakStreak();
                    break;
            }
        }

        private void IncreaseStreak()
        {
            currentStreak++;
            OnStreakChanged?.Invoke(currentStreak);

            int newMultiplier = config.GetMultiplierForStreak(currentStreak);
            if (newMultiplier != currentMultiplier)
            {
                currentMultiplier = newMultiplier;
                OnMultiplierChanged?.Invoke(currentMultiplier);
            }
        }

        private void WeakenStreak(int penalty)
        {
            int oldMultiplier = currentMultiplier;

            currentStreak = Mathf.Max(0, currentStreak - penalty);
            OnStreakChanged?.Invoke(currentStreak);

            int newMultiplier = config.GetMultiplierForStreak(currentStreak);
            if (newMultiplier != oldMultiplier)
            {
                currentMultiplier = newMultiplier;
                OnMultiplierChanged?.Invoke(currentMultiplier);
            }
        }

        private void BreakStreak()
        {
            if (currentStreak > 0)
            {
                CheckBestStreak();
                currentStreak = 0;
                currentMultiplier = 1;
                OnStreakChanged?.Invoke(currentStreak);
                OnMultiplierChanged?.Invoke(currentMultiplier);
                OnStreakBroken?.Invoke();
            }
        }

        public void ResetStreak()
        {
            currentStreak = 0;
            currentMultiplier = 1;
            OnStreakChanged?.Invoke(currentStreak);
            OnMultiplierChanged?.Invoke(currentMultiplier);
        }

        private void CheckBestStreak()
        {
            if (currentStreak > bestStreak)
            {
                bestStreak = currentStreak;
                SaveBestStreak();
                OnBestStreakBeaten?.Invoke(bestStreak);
            }
        }

        private void LoadBestStreak()
        {
            bestStreak = PlayerPrefs.GetInt(BEST_STREAK_KEY, 0);
        }

        private void SaveBestStreak()
        {
            PlayerPrefs.SetInt(BEST_STREAK_KEY, bestStreak);
            PlayerPrefs.Save();
        }

        public void ResetBestStreak()
        {
            bestStreak = 0;
            PlayerPrefs.DeleteKey(BEST_STREAK_KEY);
        }
    }
}
