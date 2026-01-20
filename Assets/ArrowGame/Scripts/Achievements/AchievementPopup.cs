using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace ArrowGame.Achievements
{
    public class AchievementPopup : MonoBehaviour
    {
        public static AchievementPopup Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject skinUnlockBadge;
        [SerializeField] private TextMeshProUGUI skinNameText;

        [Header("Animation")]
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private float slideDistance = 80f;

        private Queue<AchievementDefinition> queue = new Queue<AchievementDefinition>();
        private bool isShowing;
        private Sequence currentSequence;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (popupPanel != null)
            {
                popupPanel.SetActive(false);
            }
        }

        private void Start()
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked += Enqueue;
            }
        }

        private void OnEnable()
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked += Enqueue;
            }
        }

        private void OnDisable()
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked -= Enqueue;
            }
        }

        public void Enqueue(AchievementDefinition def)
        {
            queue.Enqueue(def);

            if (!isShowing)
            {
                ShowNext();
            }
        }

        private void ShowNext()
        {
            if (queue.Count == 0)
            {
                isShowing = false;
                return;
            }

            isShowing = true;
            var def = queue.Dequeue();
            Show(def);
        }

        private void Show(AchievementDefinition def)
        {
            if (popupPanel == null) return;

            if (iconImage != null)
            {
                iconImage.sprite = def.icon;
                iconImage.gameObject.SetActive(def.icon != null);
            }

            if (titleText != null)
            {
                titleText.text = def.title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = def.description;
            }

            bool hasSkin = !string.IsNullOrEmpty(def.unlocksSkinId);
            if (skinUnlockBadge != null)
            {
                skinUnlockBadge.SetActive(hasSkin);
            }
            if (skinNameText != null && hasSkin)
            {
                skinNameText.text = def.unlocksSkinId;
            }

            PlayAnimation();
            PlaySound();
            Feedback.HapticFeedback.Success();
        }

        private void PlayAnimation()
        {
            currentSequence?.Kill();

            popupPanel.SetActive(true);
            RectTransform rect = popupPanel.GetComponent<RectTransform>();

            Vector2 targetPos = rect.anchoredPosition;
            Vector2 startPos = targetPos + Vector2.up * slideDistance;
            rect.anchoredPosition = startPos;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            currentSequence = DOTween.Sequence();

            currentSequence.Append(rect.DOAnchorPos(targetPos, fadeInDuration).SetEase(Ease.OutBack));
            if (canvasGroup != null)
            {
                currentSequence.Join(canvasGroup.DOFade(1f, fadeInDuration));
            }

            currentSequence.AppendInterval(displayDuration);

            if (canvasGroup != null)
            {
                currentSequence.Append(canvasGroup.DOFade(0f, fadeOutDuration));
            }

            currentSequence.OnComplete(() =>
            {
                popupPanel.SetActive(false);
                rect.anchoredPosition = targetPos;
                ShowNext();
            });

            currentSequence.SetUpdate(true);
        }

        private void PlaySound()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayButtonClick();
            }
        }

        public void HideImmediate()
        {
            currentSequence?.Kill();
            if (popupPanel != null)
            {
                popupPanel.SetActive(false);
            }
            queue.Clear();
            isShowing = false;
        }
    }
}
