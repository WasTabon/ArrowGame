using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ArrowGame.Score
{
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI pointsPopupText;

        [Header("Animation")]
        [SerializeField] private float punchScale = 1.2f;
        [SerializeField] private float punchDuration = 0.2f;
        [SerializeField] private float popupDuration = 0.8f;
        [SerializeField] private float popupMoveY = 50f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color multiplierColor = Color.yellow;

        private Tween punchTween;
        private Tween popupTween;
        private int displayedScore;

        private void Start()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                ScoreManager.Instance.OnPointsEarned += ShowPointsPopup;
                ScoreManager.Instance.OnHighScoreBeaten += OnHighScoreBeaten;

                UpdateScoreDisplay(ScoreManager.Instance.CurrentScore);
                UpdateHighScoreDisplay(ScoreManager.Instance.HighScore);
            }

            if (pointsPopupText != null)
            {
                pointsPopupText.alpha = 0f;
            }
        }

        private void OnEnable()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                ScoreManager.Instance.OnPointsEarned += ShowPointsPopup;
                ScoreManager.Instance.OnHighScoreBeaten += OnHighScoreBeaten;
            }
        }

        private void OnDisable()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                ScoreManager.Instance.OnPointsEarned -= ShowPointsPopup;
                ScoreManager.Instance.OnHighScoreBeaten -= OnHighScoreBeaten;
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText == null) return;

            DOTween.To(() => displayedScore, x =>
            {
                displayedScore = x;
                scoreText.text = displayedScore.ToString("N0");
            }, score, 0.3f).SetEase(Ease.OutQuad);

            punchTween?.Kill();
            scoreText.transform.localScale = Vector3.one;
            punchTween = scoreText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
        }

        private void UpdateHighScoreDisplay(int highScore)
        {
            if (highScoreText != null)
            {
                highScoreText.text = $"BEST: {highScore:N0}";
            }
        }

        private void ShowPointsPopup(int points, int multiplier)
        {
            if (pointsPopupText == null) return;

            popupTween?.Kill();

            string text = multiplier > 1 ? $"+{points} x{multiplier}" : $"+{points}";
            pointsPopupText.text = text;
            pointsPopupText.color = multiplier > 1 ? multiplierColor : normalColor;

            pointsPopupText.transform.localPosition = Vector3.zero;
            pointsPopupText.alpha = 1f;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(pointsPopupText.transform.DOLocalMoveY(popupMoveY, popupDuration).SetEase(Ease.OutQuad));
            sequence.Join(pointsPopupText.DOFade(0f, popupDuration).SetEase(Ease.InQuad));

            popupTween = sequence;
        }

        private void OnHighScoreBeaten(int newHighScore)
        {
            UpdateHighScoreDisplay(newHighScore);

            if (highScoreText != null)
            {
                highScoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f);
                highScoreText.DOColor(Color.yellow, 0.2f).OnComplete(() =>
                {
                    highScoreText.DOColor(normalColor, 0.5f);
                });
            }
        }
    }
}
