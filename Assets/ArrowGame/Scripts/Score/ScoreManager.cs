using UnityEngine;
using System;

namespace ArrowGame.Score
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [SerializeField] private ScoreConfig config;

        private int currentScore;
        private int highScore;
        private int lastPointsEarned;

        private const string HIGH_SCORE_KEY = "ArrowGame_HighScore";

        public int CurrentScore => currentScore;
        public int HighScore => highScore;
        public int LastPointsEarned => lastPointsEarned;

        public event Action<int> OnScoreChanged;
        public event Action<int, int> OnPointsEarned;
        public event Action<int> OnHighScoreBeaten;

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
                Debug.LogError("ScoreConfig not assigned to ScoreManager!");
                return;
            }

            LoadHighScore();

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
                ResetScore();
            }
            else if (state == Core.GameState.GameOver)
            {
                CheckHighScore();
            }
        }

        private void HandleHit(Hit.HitResult result)
        {
            if (result.Zone == Hit.HitZone.Miss) return;

            int basePoints = config.GetPointsForZone(result.Zone);
            int multiplier = 1;

            if (StreakManager.Instance != null)
            {
                multiplier = config.GetMultiplierForStreak(StreakManager.Instance.CurrentStreak);
            }

            int totalPoints = basePoints * multiplier;
            lastPointsEarned = totalPoints;

            AddScore(totalPoints);
            OnPointsEarned?.Invoke(totalPoints, multiplier);
        }

        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
        }

        public void ResetScore()
        {
            currentScore = 0;
            lastPointsEarned = 0;
            OnScoreChanged?.Invoke(currentScore);
        }

        private void CheckHighScore()
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                SaveHighScore();
                OnHighScoreBeaten?.Invoke(highScore);
            }
        }

        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        public void ResetHighScore()
        {
            highScore = 0;
            PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        }
    }
}
