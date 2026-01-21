using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace ArrowGame.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Content")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image tutorialImage;
        [SerializeField] private GameObject handIcon;

        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;

        [Header("Steps")]
        [SerializeField] private TutorialStep[] steps;

        [Header("Animation")]
        [SerializeField] private float fadeIn = 0.3f;
        [SerializeField] private float fadeOut = 0.2f;

        private const string TUTORIAL_COMPLETED_KEY = "ArrowGame_TutorialCompleted";
        private int currentStep;
        private Action onComplete;
        private Tween handTween;

        public bool IsCompleted => PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY, 0) == 1;

        [Serializable]
        public class TutorialStep
        {
            public string title;
            [TextArea] public string description;
            public Sprite image;
            public bool showHandAnimation;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
        }

        private void Start()
        {
            nextButton?.onClick.AddListener(NextStep);
            skipButton?.onClick.AddListener(Skip);
        }

        public void StartTutorial(Action onCompleteCallback = null)
        {
            onComplete = onCompleteCallback;
            currentStep = 0;

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
                ShowStep(currentStep);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.DOFade(1f, fadeIn).SetUpdate(true);
                }
            }
        }

        public void StartIfNotCompleted(Action onCompleteCallback = null)
        {
            if (!IsCompleted)
            {
                StartTutorial(onCompleteCallback);
            }
            else
            {
                onCompleteCallback?.Invoke();
            }
        }

        private void ShowStep(int index)
        {
            if (steps == null || index >= steps.Length) return;

            var step = steps[index];

            if (titleText != null)
                titleText.text = step.title;

            if (descriptionText != null)
                descriptionText.text = step.description;

            if (tutorialImage != null)
            {
                tutorialImage.sprite = step.image;
                tutorialImage.gameObject.SetActive(step.image != null);
            }

            if (handIcon != null)
            {
                handIcon.SetActive(step.showHandAnimation);
                if (step.showHandAnimation)
                {
                    AnimateHand();
                }
            }

            var nextText = nextButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (nextText != null)
            {
                nextText.text = (index == steps.Length - 1) ? "START" : "NEXT";
            }
        }

        private void AnimateHand()
        {
            if (handIcon == null) return;

            handTween?.Kill();

            RectTransform rect = handIcon.GetComponent<RectTransform>();
            Vector2 startPos = rect.anchoredPosition;

            Sequence seq = DOTween.Sequence();
            seq.Append(rect.DOScale(0.9f, 0.3f));
            seq.Append(rect.DOScale(1f, 0.3f));
            seq.SetLoops(-1);
            seq.SetUpdate(true);

            handTween = seq;
        }

        private void NextStep()
        {
            Feedback.HapticFeedback.LightImpact();

            currentStep++;

            if (currentStep >= steps.Length)
            {
                Complete();
            }
            else
            {
                ShowStep(currentStep);
            }
        }

        private void Skip()
        {
            Complete();
        }

        private void Complete()
        {
            PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
            PlayerPrefs.Save();

            handTween?.Kill();

            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, fadeOut)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        tutorialPanel.SetActive(false);
                        onComplete?.Invoke();
                    });
            }
            else
            {
                tutorialPanel?.SetActive(false);
                onComplete?.Invoke();
            }
        }

        public void ResetTutorial()
        {
            PlayerPrefs.DeleteKey(TUTORIAL_COMPLETED_KEY);
            PlayerPrefs.Save();
        }
    }
}
