using UnityEngine;
using DG.Tweening;

namespace ArrowGame.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject gameHUD;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject noLivesPopup;
        [SerializeField] private GameObject livesShopPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(Core.GameManager.Instance.CurrentState);
            }
        }

        private void OnEnable()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(Core.GameState state)
        {
            HideAllPanels();

            switch (state)
            {
                case Core.GameState.MainMenu:
                    ShowPanel(mainMenuPanel);
                    break;
                case Core.GameState.Run:
                    ShowPanel(gameHUD);
                    break;
                case Core.GameState.GameOver:
                    ShowPanel(gameOverScreen);
                    break;
            }
        }

        private void HideAllPanels()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(gameHUD, false);
            SetPanelActive(gameOverScreen, false);
            SetPanelActive(noLivesPopup, false);
        }

        private void ShowPanel(GameObject panel)
        {
            SetPanelActive(panel, true);
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }

        public void ShowNoLivesPopup()
        {
            if (IAP.LivesShopUI.Instance != null)
            {
                IAP.LivesShopUI.Instance.Show();
            }
            else if (noLivesPopup != null)
            {
                noLivesPopup.SetActive(true);
            
                CanvasGroup canvasGroup = noLivesPopup.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.DOFade(1f, 0.3f);
                }
            }
        }

        public void HideNoLivesPopup()
        {
            if (IAP.LivesShopUI.Instance != null)
            {
                IAP.LivesShopUI.Instance.Hide();
            }
            else
            {
                SetPanelActive(noLivesPopup, false);
            }
        }

        public void ShowLivesShop()
        {
            if (IAP.LivesShopUI.Instance != null)
            {
                IAP.LivesShopUI.Instance.Show();
            }
        }

        public void HideLivesShop()
        {
            if (IAP.LivesShopUI.Instance != null)
            {
                IAP.LivesShopUI.Instance.Hide();
            }
        }
    }
}
