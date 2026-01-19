using UnityEngine;
using DG.Tweening;

namespace ArrowGame.UI
{
    public enum UIAnimationType
    {
        Fade,
        Scale,
        SlideFromLeft,
        SlideFromRight,
        SlideFromTop,
        SlideFromBottom,
        FadeAndScale
    }

    public class UIAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private UIAnimationType animationType = UIAnimationType.FadeAndScale;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private float delay = 0f;
        [SerializeField] private Ease easeType = Ease.OutBack;

        [Header("Slide Settings")]
        [SerializeField] private float slideDistance = 100f;

        [Header("Scale Settings")]
        [SerializeField] private float scaleFrom = 0f;

        [Header("Auto Play")]
        [SerializeField] private bool playOnEnable = true;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector2 originalPosition;
        private Vector3 originalScale;
        private Tween animationTween;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                originalPosition = rectTransform.anchoredPosition;
                originalScale = rectTransform.localScale;
            }
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                PlayAnimation();
            }
        }

        private void OnDisable()
        {
            animationTween?.Kill();
        }

        public void PlayAnimation()
        {
            animationTween?.Kill();

            switch (animationType)
            {
                case UIAnimationType.Fade:
                    PlayFade();
                    break;
                case UIAnimationType.Scale:
                    PlayScale();
                    break;
                case UIAnimationType.SlideFromLeft:
                    PlaySlide(Vector2.left);
                    break;
                case UIAnimationType.SlideFromRight:
                    PlaySlide(Vector2.right);
                    break;
                case UIAnimationType.SlideFromTop:
                    PlaySlide(Vector2.up);
                    break;
                case UIAnimationType.SlideFromBottom:
                    PlaySlide(Vector2.down);
                    break;
                case UIAnimationType.FadeAndScale:
                    PlayFadeAndScale();
                    break;
            }
        }

        private void PlayFade()
        {
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            animationTween = canvasGroup.DOFade(1f, duration)
                .SetDelay(delay)
                .SetEase(easeType)
                .SetUpdate(true);
        }

        private void PlayScale()
        {
            if (rectTransform == null) return;

            rectTransform.localScale = Vector3.one * scaleFrom;
            animationTween = rectTransform.DOScale(originalScale, duration)
                .SetDelay(delay)
                .SetEase(easeType)
                .SetUpdate(true);
        }

        private void PlaySlide(Vector2 direction)
        {
            if (rectTransform == null) return;

            Vector2 startPos = originalPosition + direction * slideDistance;
            rectTransform.anchoredPosition = startPos;

            animationTween = rectTransform.DOAnchorPos(originalPosition, duration)
                .SetDelay(delay)
                .SetEase(easeType)
                .SetUpdate(true);
        }

        private void PlayFadeAndScale()
        {
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one * scaleFrom;
            }

            Sequence seq = DOTween.Sequence();
            seq.Append(canvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad));
            
            if (rectTransform != null)
            {
                seq.Join(rectTransform.DOScale(originalScale, duration).SetEase(easeType));
            }

            seq.SetDelay(delay).SetUpdate(true);
            animationTween = seq;
        }

        public void ResetAnimation()
        {
            animationTween?.Kill();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = originalPosition;
                rectTransform.localScale = originalScale;
            }
        }
    }
}
