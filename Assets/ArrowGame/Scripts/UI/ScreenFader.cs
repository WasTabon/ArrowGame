using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ArrowGame.UI
{
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private Color fadeColor = Color.black;

        private Tween fadeTween;

        public bool IsFading { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (fadeImage != null)
            {
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
                fadeImage.raycastTarget = false;
            }

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }

        public void FadeIn(Action onComplete = null)
        {
            FadeIn(fadeDuration, onComplete);
        }

        public void FadeIn(float duration, Action onComplete = null)
        {
            IsFading = true;
            fadeTween?.Kill();

            if (fadeImage != null)
            {
                fadeImage.raycastTarget = true;
                fadeTween = fadeImage.DOFade(1f, duration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        IsFading = false;
                        onComplete?.Invoke();
                    });
            }
            else if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.blocksRaycasts = true;
                fadeTween = fadeCanvasGroup.DOFade(1f, duration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        IsFading = false;
                        onComplete?.Invoke();
                    });
            }
        }

        public void FadeOut(Action onComplete = null)
        {
            FadeOut(fadeDuration, onComplete);
        }

        public void FadeOut(float duration, Action onComplete = null)
        {
            IsFading = true;
            fadeTween?.Kill();

            if (fadeImage != null)
            {
                fadeTween = fadeImage.DOFade(0f, duration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        fadeImage.raycastTarget = false;
                        IsFading = false;
                        onComplete?.Invoke();
                    });
            }
            else if (fadeCanvasGroup != null)
            {
                fadeTween = fadeCanvasGroup.DOFade(0f, duration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        fadeCanvasGroup.blocksRaycasts = false;
                        IsFading = false;
                        onComplete?.Invoke();
                    });
            }
        }

        public void FadeInOut(float holdDuration, Action onFadedIn = null, Action onComplete = null)
        {
            FadeIn(() =>
            {
                onFadedIn?.Invoke();
                DOVirtual.DelayedCall(holdDuration, () =>
                {
                    FadeOut(onComplete);
                }).SetUpdate(true);
            });
        }

        public void SetFadeImmediate(float alpha)
        {
            fadeTween?.Kill();

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
                fadeImage.raycastTarget = alpha > 0f;
            }
            else if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = alpha;
                fadeCanvasGroup.blocksRaycasts = alpha > 0f;
            }
        }
    }
}
