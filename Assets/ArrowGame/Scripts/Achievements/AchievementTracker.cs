using UnityEngine;

namespace ArrowGame.Achievements
{
    public class AchievementTracker : MonoBehaviour
    {
        public static AchievementTracker Instance { get; private set; }

        private int coreHitsInRow;
        private int innerOrBetterInRow;
        private int ringsWithoutOuter;
        private int totalRingsThisRun;
        private int missesThisRun;
        private float lowestSpeedReached;
        private bool reachedLowSpeed;

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
            SubscribeToEvents();
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

            if (Speed.SpeedController.Instance != null)
            {
                Speed.SpeedController.Instance.OnSpeedChanged += OnSpeedChanged;
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

            if (Speed.SpeedController.Instance != null)
            {
                Speed.SpeedController.Instance.OnSpeedChanged -= OnSpeedChanged;
            }
        }

        private void OnGameStateChanged(Core.GameState state)
        {
            if (state == Core.GameState.Run)
            {
                StartRun();
            }
            else if (state == Core.GameState.GameOver || state == Core.GameState.Results)
            {
                EndRun();
            }
        }

        private void StartRun()
        {
            coreHitsInRow = 0;
            innerOrBetterInRow = 0;
            ringsWithoutOuter = 0;
            totalRingsThisRun = 0;
            missesThisRun = 0;
            lowestSpeedReached = float.MaxValue;
            reachedLowSpeed = false;

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.ReportValue(AchievementConditionType.GamesPlayed, 1, true);
            }
        }

        private void EndRun()
        {
            if (AchievementManager.Instance == null) return;

            if (Score.ScoreManager.Instance != null)
            {
                int score = Score.ScoreManager.Instance.CurrentScore;
                AchievementManager.Instance.ReportValue(AchievementConditionType.SingleRunScore, score);
                AchievementManager.Instance.ReportValue(AchievementConditionType.TotalScore, score, true);
            }

            AchievementManager.Instance.ReportValue(AchievementConditionType.SingleRunRings, totalRingsThisRun);

            if (totalRingsThisRun >= 10 && missesThisRun == 0)
            {
                AchievementManager.Instance.ReportValue(AchievementConditionType.PerfectRun, 1, true);
            }
        }

        private void OnHit(Hit.HitResult result)
        {
            if (AchievementManager.Instance == null) return;

            totalRingsThisRun++;
            AchievementManager.Instance.ReportValue(AchievementConditionType.TotalRingsPassed, 1, true);

            switch (result.Zone)
            {
                case Hit.HitZone.Core:
                    coreHitsInRow++;
                    innerOrBetterInRow++;
                    ringsWithoutOuter++;
                    AchievementManager.Instance.ReportValue(AchievementConditionType.CoreHitsInRow, coreHitsInRow);
                    AchievementManager.Instance.ReportValue(AchievementConditionType.CoreHitsTotal, 1, true);
                    AchievementManager.Instance.ReportValue(AchievementConditionType.InnerOrBetterInRow, innerOrBetterInRow);
                    AchievementManager.Instance.ReportValue(AchievementConditionType.RingsWithoutOuter, ringsWithoutOuter);
                    break;

                case Hit.HitZone.Inner:
                    coreHitsInRow = 0;
                    innerOrBetterInRow++;
                    ringsWithoutOuter++;
                    AchievementManager.Instance.ReportValue(AchievementConditionType.InnerOrBetterInRow, innerOrBetterInRow);
                    AchievementManager.Instance.ReportValue(AchievementConditionType.RingsWithoutOuter, ringsWithoutOuter);
                    break;

                case Hit.HitZone.Middle:
                    coreHitsInRow = 0;
                    innerOrBetterInRow = 0;
                    ringsWithoutOuter++;
                    AchievementManager.Instance.ReportValue(AchievementConditionType.RingsWithoutOuter, ringsWithoutOuter);
                    break;

                case Hit.HitZone.Outer:
                    coreHitsInRow = 0;
                    innerOrBetterInRow = 0;
                    ringsWithoutOuter = 0;
                    break;

                case Hit.HitZone.Miss:
                    coreHitsInRow = 0;
                    innerOrBetterInRow = 0;
                    ringsWithoutOuter = 0;
                    missesThisRun++;
                    break;
            }
        }

        private void OnStreakChanged(int streak)
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.ReportValue(AchievementConditionType.StreakReached, streak);
            }
        }

        private void OnMultiplierChanged(int multiplier)
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.ReportValue(AchievementConditionType.MaxMultiplier, multiplier);
            }
        }

        private void OnSpeedChanged(float speed)
        {
            if (AchievementManager.Instance == null) return;

            if (speed < lowestSpeedReached)
            {
                lowestSpeedReached = speed;
            }

            if (speed <= 5f && !reachedLowSpeed)
            {
                reachedLowSpeed = true;
                AchievementManager.Instance.ReportValue(AchievementConditionType.SurviveLowSpeed, 1);
            }

            if (reachedLowSpeed && speed >= lowestSpeedReached + 10f)
            {
                AchievementManager.Instance.ReportValue(AchievementConditionType.ComebackFromLowSpeed, 1);
            }
        }
    }
}
