using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.IAP
{
    public class IAPButton : MonoBehaviour
    {
        [Header("Product")]
        [SerializeField] private string productId;

        [Header("UI References")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI livesAmountText;

        [Header("Config Reference")]
        [SerializeField] private IAPConfig config;

        private void Start()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }

            UpdateUI();

            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.OnStoreInitialized += UpdateUI;
            }
        }

        private void OnDestroy()
        {
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.OnStoreInitialized -= UpdateUI;
            }
        }

        private void UpdateUI()
        {
            if (config == null) return;

            var product = config.GetProductById(productId);
            if (product == null) return;

            if (titleText != null)
            {
                titleText.text = product.displayName;
            }

            if (livesAmountText != null)
            {
                livesAmountText.text = $"+{product.livesAmount}";
            }

            if (priceText != null && IAPManager.Instance != null)
            {
                string price = IAPManager.Instance.GetLocalizedPrice(productId);
                priceText.text = string.IsNullOrEmpty(price) ? "$0.99" : price;
            }
        }

        private void OnButtonClick()
        {
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.BuyProduct(productId);
            }
        }

        public void SetProductId(string id)
        {
            productId = id;
            UpdateUI();
        }
    }
}
