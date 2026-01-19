using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ArrowGame.UI
{
    public class ResultsScreen : MonoBehaviour
    {
        public static ResultsScreen Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private GameObject newHighScoreBadge;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI bestStreakText;
        [SerializeField] private TextMeshProUGUI coreHitsText;
        [SerializeField] private TextMeshProUGUI innerHitsText;
        [SerializeField] private TextMeshProUGUI middleHitsText;
        [SerializeField] private TextMeshProUGUI outerHitsText;
        [SerializeField] private TextMeshProUGUI missesText;
        [SerializeField] private TextMeshProUGUI totalRingsText;
        [SerializeField] private TextMeshProUGUI accuracyText;

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Animation")]
        [SerializeField] private float showDelay = 0.5f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float statAnimationDelay = 0.1f;

        private int coreHits;
        private int innerHits;
        private int middleHits;
        private int outerHits;
        private int misses;
        private int sessionBestStreak;
        private bool isNewHighScore;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (resultsPanel != null)
            {
                resultsPanel.SetActive(false);
            }

            if (newHighScoreBadge != null)
            {
                newHighScoreBadge.SetActive(false);
            }
        }

        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += TrackHit;
            }

            if (Score.ScoreManager.Instance != null)
            {
                Score.ScoreManager.Instance.OnHighScoreBeaten += OnHighScoreBeaten;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnStreakChanged += TrackStreak;
            }

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += OnStateChanged;
            }
        }

        private void OnEnable()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit += TrackHit;
            }

            if (Score.ScoreManager.Instance != null)
            {
                Score.ScoreManager.Instance.OnHighScoreBeaten += OnHighScoreBeaten;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnStreakChanged += TrackStreak;
            }

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += OnStateChanged;
            }
        }

        private void OnDisable()
        {
            if (Hit.HitDetector.Instance != null)
            {
                Hit.HitDetector.Instance.OnHit -= TrackHit;
            }

            if (Score.ScoreManager.Instance != null)
            {
                Score.ScoreManager.Instance.OnHighScoreBeaten -= OnHighScoreBeaten;
            }

            if (Score.StreakManager.Instance != null)
            {
                Score.StreakManager.Instance.OnStreakChanged -= TrackStreak;
            }

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged -= OnStateChanged;
            }
        }

        private void OnStateChanged(Core.GameState state)
        {
            if (state == Core.GameState.Run)
            {
                ResetStats();
            }
            else if (state == Core.GameState.GameOver)
            {
                DOVirtual.DelayedCall(showDelay, Show).SetUpdate(true);
            }
        }

        private void TrackHit(Hit.HitResult result)
        {
            switch (result.Zone)
            {
                case Hit.HitZone.Core: coreHits++; break;
                case Hit.HitZone.Inner: innerHits++; break;
                case Hit.HitZone.Middle: middleHits++; break;
                case Hit.HitZone.Outer: outerHits++; break;
                case Hit.HitZone.Miss: misses++; break;
            }
        }

        private void TrackStreak(int streak)
        {
            if (streak > sessionBestStreak)
            {
                sessionBestStreak = streak;
            }
        }

        private void OnHighScoreBeaten(int newHighScore)
        {
            isNewHighScore = true;
        }

        private void ResetStats()
        {
            coreHits = 0;
            innerHits = 0;
            middleHits = 0;
            outerHits = 0;
            misses = 0;
            sessionBestStreak = 0;
            isNewHighScore = false;
        }

        public void Show()
        {
            if (resultsPanel == null) return;

            resultsPanel.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true);
            }

            PopulateResults();
            AnimateStats();
        }

        public void Hide()
        {
            if (resultsPanel == null) return;

            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 0.2f)
                    .SetUpdate(true)
                    .OnComplete(() => resultsPanel.SetActive(false));
            }
            else
            {
                resultsPanel.SetActive(false);
            }
        }

        private void PopulateResults()
        {
            int finalScore = Score.ScoreManager.Instance != null ? Score.ScoreManager.Instance.CurrentScore : 0;
            int highScore = Score.ScoreManager.Instance != null ? Score.ScoreManager.Instance.HighScore : 0;

            if (finalScoreText != null)
            {
                finalScoreText.text = "0";
            }

            if (highScoreText != null)
            {
                highScoreText.text = $"BEST: {highScore:N0}";
            }

            if (newHighScoreBadge != null)
            {
                newHighScoreBadge.SetActive(isNewHighScore);
            }

            int totalRings = coreHits + innerHits + middleHits + outerHits;
            int successfulHits = coreHits + innerHits + middleHits + outerHits;
            int totalAttempts = successfulHits + misses;
            float accuracy = totalAttempts > 0 ? (float)successfulHits / totalAttempts * 100f : 0f;

            if (bestStreakText != null) bestStreakText.text = "0";
            if (coreHitsText != null) coreHitsText.text = "0";
            if (innerHitsText != null) innerHitsText.text = "0";
            if (middleHitsText != null) middleHitsText.text = "0";
            if (outerHitsText != null) outerHitsText.text = "0";
            if (missesText != null) missesText.text = "0";
            if (totalRingsText != null) totalRingsText.text = "0";
            if (accuracyText != null) accuracyText.text = "0%";
        }

        private void AnimateStats()
        {
            int finalScore = Score.ScoreManager.Instance != null ? Score.ScoreManager.Instance.CurrentScore : 0;
            int totalRings = coreHits + innerHits + middleHits + outerHits;
            int successfulHits = totalRings;
            int totalAttempts = successfulHits + misses;
            float accuracy = totalAttempts > 0 ? (float)successfulHits / totalAttempts * 100f : 0f;

            Sequence seq = DOTween.Sequence();
            float delay = 0f;

            if (finalScoreText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => finalScoreText.text = x.ToString("N0"), finalScore, 0.8f).SetEase(Ease.OutQuad));
                seq.Insert(delay, finalScoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f));
            }

            delay += statAnimationDelay;

            if (bestStreakText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => bestStreakText.text = x.ToString(), sessionBestStreak, 0.5f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (coreHitsText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => coreHitsText.text = x.ToString(), coreHits, 0.4f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (innerHitsText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => innerHitsText.text = x.ToString(), innerHits, 0.4f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (middleHitsText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => middleHitsText.text = x.ToString(), middleHits, 0.4f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (outerHitsText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => outerHitsText.text = x.ToString(), outerHits, 0.4f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (missesText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => missesText.text = x.ToString(), misses, 0.4f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (totalRingsText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0, x => totalRingsText.text = x.ToString(), totalRings, 0.4f).SetEase(Ease.OutQuad));
            }

            delay += statAnimationDelay;

            if (accuracyText != null)
            {
                seq.Insert(delay, DOTween.To(() => 0f, x => accuracyText.text = $"{x:F1}%", accuracy, 0.4f).SetEase(Ease.OutQuad));
            }

            if (isNewHighScore && newHighScoreBadge != null)
            {
                seq.AppendCallback(() =>
                {
                    newHighScoreBadge.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 5, 0.5f);
                });
            }

            seq.SetUpdate(true);
        }

        private void OnRestartClicked()
        {
            Hide();
            
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.FadeIn(() =>
                {
                    Time.timeScale = 1f;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                    );
                });
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }

        private void OnMainMenuClicked()
        {
            Hide();
            Time.timeScale = 1f;
            Core.GameManager.Instance?.GoToMainMenu();
        }
    }
}
