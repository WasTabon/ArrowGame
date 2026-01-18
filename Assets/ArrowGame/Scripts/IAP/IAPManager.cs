#if UNITY_PURCHASING
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System;

namespace ArrowGame.IAP
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        public static IAPManager Instance { get; private set; }

        [SerializeField] private IAPConfig config;

        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        public bool IsInitialized => storeController != null && extensionProvider != null;

        public event Action<string> OnPurchaseSuccess;
        public event Action<string, string> OnPurchaseFailed;
        public event Action OnStoreInitialized;
        public event Action<string> OnStoreInitializeFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("IAPConfig not assigned to IAPManager!");
                return;
            }

            InitializePurchasing();
        }

        private void InitializePurchasing()
        {
            if (IsInitialized) return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in config.products)
            {
                builder.AddProduct(product.productId, 
                    product.isConsumable ? ProductType.Consumable : ProductType.NonConsumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
            
            Debug.Log("IAP: Store initialized successfully");
            OnStoreInitialized?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"IAP: Store initialization failed: {error}");
            OnStoreInitializeFailed?.Invoke(error.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"IAP: Store initialization failed: {error} - {message}");
            OnStoreInitializeFailed?.Invoke($"{error}: {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string productId = args.purchasedProduct.definition.id;
            
            Debug.Log($"IAP: Processing purchase: {productId}");

            var product = config.GetProductById(productId);
            if (product != null)
            {
                if (Lives.LivesManager.Instance != null)
                {
                    Lives.LivesManager.Instance.AddLife(product.livesAmount);
                    Debug.Log($"IAP: Added {product.livesAmount} lives");
                }
                
                OnPurchaseSuccess?.Invoke(productId);
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"IAP: Purchase failed - {product.definition.id}: {failureReason}");
            OnPurchaseFailed?.Invoke(product.definition.id, failureReason.ToString());
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"IAP: Purchase failed - {product.definition.id}: {failureDescription.message}");
            OnPurchaseFailed?.Invoke(product.definition.id, failureDescription.message);
        }

        public void BuyProduct(string productId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("IAP: Store not initialized");
                OnPurchaseFailed?.Invoke(productId, "Store not initialized");
                return;
            }

            Product product = storeController.products.WithID(productId);
            
            if (product != null && product.availableToPurchase)
            {
                Debug.Log($"IAP: Initiating purchase: {productId}");
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogWarning($"IAP: Product not available: {productId}");
                OnPurchaseFailed?.Invoke(productId, "Product not available");
            }
        }

        public string GetLocalizedPrice(string productId)
        {
            if (!IsInitialized) return "";

            Product product = storeController.products.WithID(productId);
            if (product != null)
            {
                return product.metadata.localizedPriceString;
            }
            return "";
        }

        public void RestorePurchases()
        {
#if UNITY_IOS
            if (!IsInitialized)
            {
                Debug.LogWarning("IAP: Store not initialized");
                return;
            }

            Debug.Log("IAP: Restoring purchases...");
            extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result, error) =>
            {
                if (result)
                {
                    Debug.Log("IAP: Restore purchases succeeded");
                }
                else
                {
                    Debug.LogWarning($"IAP: Restore purchases failed: {error}");
                }
            });
#endif
        }
    }
}
#else
using UnityEngine;
using System;

namespace ArrowGame.IAP
{
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance { get; private set; }

        [SerializeField] private IAPConfig config;

        public bool IsInitialized => false;

        public event Action<string> OnPurchaseSuccess;
        public event Action<string, string> OnPurchaseFailed;
        public event Action OnStoreInitialized;
        public event Action<string> OnStoreInitializeFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.LogWarning("IAP: Unity Purchasing not enabled. Install via Package Manager.");
        }

        public void BuyProduct(string productId)
        {
            Debug.LogWarning($"IAP: Cannot buy {productId} - Unity Purchasing not enabled");
            
#if UNITY_EDITOR
            var product = config.GetProductById(productId);
            if (product != null && Lives.LivesManager.Instance != null)
            {
                Lives.LivesManager.Instance.AddLife(product.livesAmount);
                Debug.Log($"IAP [EDITOR]: Simulated purchase - Added {product.livesAmount} lives");
                OnPurchaseSuccess?.Invoke(productId);
            }
#else
            OnPurchaseFailed?.Invoke(productId, "Unity Purchasing not enabled");
#endif
        }

        public string GetLocalizedPrice(string productId)
        {
            return "$0.99";
        }

        public void RestorePurchases()
        {
            Debug.LogWarning("IAP: Cannot restore - Unity Purchasing not enabled");
        }
    }
}
#endif
