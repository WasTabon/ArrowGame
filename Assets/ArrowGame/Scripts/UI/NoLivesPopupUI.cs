using UnityEngine;
using UnityEngine.UI;

namespace ArrowGame.UI
{
    public class NoLivesPopupUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button buyLivesButton;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            if (buyLivesButton != null)
            {
                buyLivesButton.onClick.AddListener(OnBuyLivesClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }
        }

        private void OnBuyLivesClicked()
        {
            UIManager.Instance?.HideNoLivesPopup();
            UIManager.Instance?.ShowLivesShop();
        }

        private void OnCloseClicked()
        {
            UIManager.Instance?.HideNoLivesPopup();
        }
    }
}
