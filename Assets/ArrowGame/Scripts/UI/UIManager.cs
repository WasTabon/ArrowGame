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
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject noLivesPopup;

        [Header("Transition Settings")]
        [SerializeField] private float panelFadeDuration = 0.25f;

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
            switch (state)
            {
                case Core.GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case Core.GameState.Run:
                    ShowGameHUD();
                    break;
                case Core.GameState.Pause:
                    break;
                case Core.GameState.GameOver:
                    break;
            }
        }

        private void ShowMainMenu()
        {
            HideAllPanels();
            ShowPanelAnimated(mainMenuPanel);
        }

        public void HideMainMenu()
        {
            HidePanelAnimated(mainMenuPanel);
        }

        private void ShowGameHUD()
        {
            HidePanelImmediate(mainMenuPanel);
            HidePanelImmediate(noLivesPopup);
            ShowPanelAnimated(gameHUD);
        }

        private void HideAllPanels()
        {
            HidePanelImmediate(mainMenuPanel);
            HidePanelImmediate(gameHUD);
            HidePanelImmediate(pausePanel);
            HidePanelImmediate(noLivesPopup);
        }

        private void ShowPanelAnimated(GameObject panel)
        {
            if (panel == null) return;

            panel.SetActive(true);

            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.DOFade(1f, panelFadeDuration).SetUpdate(true);
            }
        }

        private void HidePanelAnimated(GameObject panel, System.Action onComplete = null)
        {
            if (panel == null)
            {
                onComplete?.Invoke();
                return;
            }

            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOFade(0f, panelFadeDuration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        panel.SetActive(false);
                        onComplete?.Invoke();
                    });
            }
            else
            {
                panel.SetActive(false);
                onComplete?.Invoke();
            }
        }

        private void HidePanelImmediate(GameObject panel)
        {
            if (panel == null) return;
            panel.SetActive(false);
        }

        public void ShowNoLivesPopup()
        {
            if (noLivesPopup != null)
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
            if (noLivesPopup != null)
            {
                CanvasGroup canvasGroup = noLivesPopup.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0f, 0.2f).OnComplete(() =>
                    {
                        noLivesPopup.SetActive(false);
                    });
                }
                else
                {
                    noLivesPopup.SetActive(false);
                }
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
