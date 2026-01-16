using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ArrowGame.Lives
{
    public class GameStartController : MonoBehaviour
    {
        public static GameStartController Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject noLivesPopup;
        [SerializeField] private Button playButton;
        [SerializeField] private Button closePopupButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (noLivesPopup != null)
            {
                noLivesPopup.SetActive(false);
            }
        }

        private void Start()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(TryStartGame);
            }

            if (closePopupButton != null)
            {
                closePopupButton.onClick.AddListener(CloseNoLivesPopup);
            }
        }

        public void TryStartGame()
        {
            if (LivesManager.Instance == null)
            {
                StartGame();
                return;
            }

            if (LivesManager.Instance.TryUseLife())
            {
                StartGame();
            }
            else
            {
                ShowNoLivesPopup();
            }
        }

        private void StartGame()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.StartRun();
            }
        }

        private void ShowNoLivesPopup()
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

        private void CloseNoLivesPopup()
        {
            if (noLivesPopup != null)
            {
                CanvasGroup canvasGroup = noLivesPopup.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
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
    }
}
