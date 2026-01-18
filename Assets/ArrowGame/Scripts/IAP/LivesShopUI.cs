using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ArrowGame.IAP
{
    public class LivesShopUI : MonoBehaviour
    {
        public static LivesShopUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("UI Elements")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button restoreButton;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }

            if (canvasGroup == null && shopPanel != null)
            {
                canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (restoreButton != null)
            {
                restoreButton.onClick.AddListener(OnRestoreClick);
                
#if !UNITY_IOS
                restoreButton.gameObject.SetActive(false);
#endif
            }

            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.OnPurchaseSuccess += OnPurchaseSuccess;
                IAPManager.Instance.OnPurchaseFailed += OnPurchaseFailed;
            }
        }

        private void OnDestroy()
        {
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.OnPurchaseSuccess -= OnPurchaseSuccess;
                IAPManager.Instance.OnPurchaseFailed -= OnPurchaseFailed;
            }
        }

        public void Show()
        {
            if (shopPanel == null) return;

            shopPanel.SetActive(true);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true);
            }

            if (statusText != null)
            {
                statusText.text = "";
            }
        }

        public void Hide()
        {
            if (shopPanel == null) return;

            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, fadeOutDuration)
                    .SetUpdate(true)
                    .OnComplete(() => shopPanel.SetActive(false));
            }
            else
            {
                shopPanel.SetActive(false);
            }
        }

        private void OnRestoreClick()
        {
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.RestorePurchases();
                
                if (statusText != null)
                {
                    statusText.text = "Restoring purchases...";
                }
            }
        }

        private void OnPurchaseSuccess(string productId)
        {
            if (statusText != null)
            {
                statusText.text = "Purchase successful!";
                statusText.DOFade(0f, 1f).SetDelay(2f).SetUpdate(true);
            }

            DOVirtual.DelayedCall(1f, Hide).SetUpdate(true);
        }

        private void OnPurchaseFailed(string productId, string reason)
        {
            if (statusText != null)
            {
                statusText.text = $"Purchase failed: {reason}";
            }
        }
    }
}
