using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace ArrowGame.UI
{
    public class CountdownController : MonoBehaviour
    {
        public static CountdownController Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject countdownPanel;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private int countdownFrom = 3;
        [SerializeField] private float intervalDuration = 0.8f;
        [SerializeField] private string goText = "GO!";

        [Header("Animation")]
        [SerializeField] private float scaleFrom = 2f;
        [SerializeField] private float scaleTo = 1f;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        private Sequence countdownSequence;
        private bool isCountingDown;

        public bool IsCountingDown => isCountingDown;
        public event Action OnCountdownComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (countdownPanel != null)
            {
                countdownPanel.SetActive(false);
            }
        }

        public void StartCountdown(Action onComplete = null)
        {
            if (isCountingDown) return;

            isCountingDown = true;
            countdownSequence?.Kill();

            if (countdownPanel != null)
            {
                countdownPanel.SetActive(true);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            countdownSequence = DOTween.Sequence();

            for (int i = countdownFrom; i >= 1; i--)
            {
                int num = i;
                countdownSequence.AppendCallback(() => ShowNumber(num.ToString()));
                countdownSequence.AppendInterval(intervalDuration);
            }

            countdownSequence.AppendCallback(() => ShowNumber(goText));
            countdownSequence.AppendInterval(0.5f);

            countdownSequence.AppendCallback(() =>
            {
                HideCountdown();
                isCountingDown = false;
                OnCountdownComplete?.Invoke();
                onComplete?.Invoke();
            });

            countdownSequence.SetUpdate(true);
        }

        private void ShowNumber(string text)
        {
            if (countdownText == null) return;

            countdownText.text = text;
            countdownText.transform.localScale = Vector3.one * scaleFrom;
            countdownText.alpha = 1f;

            Sequence seq = DOTween.Sequence();
            seq.Append(countdownText.transform.DOScale(scaleTo, scaleDuration).SetEase(Ease.OutBack));
            seq.Join(countdownText.DOFade(0.8f, scaleDuration));
            seq.Append(countdownText.DOFade(0f, fadeOutDuration));
            seq.SetUpdate(true);

            Feedback.HapticFeedback.LightImpact();

            if (Audio.AudioManager.Instance != null)
            {
                if (text == goText)
                {
                    Audio.AudioManager.Instance.PlayCountdownGo();
                }
                else
                {
                    Audio.AudioManager.Instance.PlayCountdownTick();
                }
            }
        }

        private void HideCountdown()
        {
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 0.2f)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        if (countdownPanel != null)
                        {
                            countdownPanel.SetActive(false);
                        }
                    });
            }
            else if (countdownPanel != null)
            {
                countdownPanel.SetActive(false);
            }
        }

        public void CancelCountdown()
        {
            countdownSequence?.Kill();
            isCountingDown = false;
            
            if (countdownPanel != null)
            {
                countdownPanel.SetActive(false);
            }
        }
    }
}
