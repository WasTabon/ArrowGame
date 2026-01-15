using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ArrowGame.UI
{
    public class SlowdownVignette : MonoBehaviour
    {
        public static SlowdownVignette Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Image vignetteImage;

        [Header("Settings")]
        [SerializeField] private float targetAlpha = 0.5f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.3f;

        private Tween fadeTween;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (vignetteImage != null)
            {
                SetAlpha(0f);
            }
        }

        private void Start()
        {
            SubscribeToInput();
        }

        private void OnEnable()
        {
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            if (Input.InputController.Instance != null)
            {
                Input.InputController.Instance.OnInputStart += HandleInputStart;
                Input.InputController.Instance.OnInputEnd += HandleInputEnd;
            }
        }

        private void UnsubscribeFromInput()
        {
            if (Input.InputController.Instance != null)
            {
                Input.InputController.Instance.OnInputStart -= HandleInputStart;
                Input.InputController.Instance.OnInputEnd -= HandleInputEnd;
            }
        }

        private void HandleInputStart()
        {
            FadeIn();
        }

        private void HandleInputEnd()
        {
            FadeOut();
        }

        public void FadeIn()
        {
            fadeTween?.Kill();
            fadeTween = DOTween.To(() => vignetteImage.color.a, SetAlpha, targetAlpha, fadeInDuration)
                .SetEase(Ease.OutQuad);
        }

        public void FadeOut()
        {
            fadeTween?.Kill();
            fadeTween = DOTween.To(() => vignetteImage.color.a, SetAlpha, 0f, fadeOutDuration)
                .SetEase(Ease.OutQuad);
        }

        private void SetAlpha(float alpha)
        {
            if (vignetteImage == null) return;
            Color color = vignetteImage.color;
            color.a = alpha;
            vignetteImage.color = color;
        }
    }
}