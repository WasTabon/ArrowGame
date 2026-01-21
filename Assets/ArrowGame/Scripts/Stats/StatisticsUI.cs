using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace ArrowGame.Stats
{
    public class StatisticsUI : MonoBehaviour
    {
        public static StatisticsUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("General Stats")]
        [SerializeField] private TextMeshProUGUI gamesPlayedText;
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI playTimeText;

        [Header("Ring Stats")]
        [SerializeField] private TextMeshProUGUI totalRingsText;
        [SerializeField] private TextMeshProUGUI coreHitsText;
        [SerializeField] private TextMeshProUGUI innerHitsText;
        [SerializeField] private TextMeshProUGUI middleHitsText;
        [SerializeField] private TextMeshProUGUI outerHitsText;
        [SerializeField] private TextMeshProUGUI missesText;
        [SerializeField] private TextMeshProUGUI accuracyText;

        [Header("Best Stats")]
        [SerializeField] private TextMeshProUGUI longestStreakText;
        [SerializeField] private TextMeshProUGUI highestMultiplierText;
        [SerializeField] private TextMeshProUGUI perfectRunsText;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button resetButton;

        [Header("Animation")]
        [SerializeField] private float fadeIn = 0.25f;
        [SerializeField] private float fadeOut = 0.2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (panel != null)
                panel.SetActive(false);
        }

        private void Start()
        {
            closeButton?.onClick.AddListener(Hide);
            resetButton?.onClick.AddListener(OnResetClicked);
        }

        public void Show()
        {
            if (panel == null) return;

            panel.SetActive(true);
            Refresh();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, fadeIn).SetUpdate(true);
            }
        }

        public void Hide()
        {
            if (panel == null) return;

            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, fadeOut)
                    .SetUpdate(true)
                    .OnComplete(() => panel.SetActive(false));
            }
            else
            {
                panel.SetActive(false);
            }
        }

        private void Refresh()
        {
            if (StatisticsManager.Instance == null) return;

            var stats = StatisticsManager.Instance.Stats;

            SetText(gamesPlayedText, stats.totalGamesPlayed.ToString("N0"));
            SetText(totalScoreText, stats.totalScore.ToString("N0"));
            SetText(highScoreText, stats.highScore.ToString("N0"));

            TimeSpan playTime = StatisticsManager.Instance.GetTotalPlayTime();
            SetText(playTimeText, FormatPlayTime(playTime));

            SetText(totalRingsText, stats.totalRingsPassed.ToString("N0"));
            SetText(coreHitsText, stats.totalCoreHits.ToString("N0"));
            SetText(innerHitsText, stats.totalInnerHits.ToString("N0"));
            SetText(middleHitsText, stats.totalMiddleHits.ToString("N0"));
            SetText(outerHitsText, stats.totalOuterHits.ToString("N0"));
            SetText(missesText, stats.totalMisses.ToString("N0"));

            float accuracy = StatisticsManager.Instance.GetAccuracy();
            SetText(accuracyText, $"{accuracy:F1}%");

            SetText(longestStreakText, stats.longestStreak.ToString("N0"));
            SetText(highestMultiplierText, $"x{stats.highestMultiplier}");
            SetText(perfectRunsText, stats.perfectRuns.ToString("N0"));
        }

        private void SetText(TextMeshProUGUI text, string value)
        {
            if (text != null)
                text.text = value;
        }

        private string FormatPlayTime(TimeSpan time)
        {
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}h {time.Minutes}m";
            else if (time.TotalMinutes >= 1)
                return $"{time.Minutes}m {time.Seconds}s";
            else
                return $"{time.Seconds}s";
        }

        private void OnResetClicked()
        {
            StatisticsManager.Instance?.ResetStats();
            Refresh();
            Feedback.HapticFeedback.Warning();
        }
    }
}
