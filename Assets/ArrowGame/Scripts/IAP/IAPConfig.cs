using UnityEngine;

namespace ArrowGame.IAP
{
    [CreateAssetMenu(fileName = "IAPConfig", menuName = "ArrowGame/IAP Config")]
    public class IAPConfig : ScriptableObject
    {
        [System.Serializable]
        public class IAPProduct
        {
            public string productId;
            public string displayName;
            public int livesAmount;
            public bool isConsumable = true;
        }

        [Header("Products")]
        public IAPProduct[] products = new IAPProduct[]
        {
            new IAPProduct { productId = "com.arrowgame.lives_1", displayName = "1 Life", livesAmount = 1 },
        };

        public IAPProduct GetProductById(string productId)
        {
            foreach (var product in products)
            {
                if (product.productId == productId)
                    return product;
            }
            return null;
        }
    }
}
