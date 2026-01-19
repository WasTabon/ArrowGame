using UnityEngine;
using UnityEngine.UI;

namespace ArrowGame.Lives
{
    public class GameStartController : MonoBehaviour
    {
        public static GameStartController Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private Button playButton;

        [Header("Countdown")]
        [SerializeField] private bool useCountdown = true;

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
            if (playButton != null)
            {
                playButton.onClick.AddListener(TryStartGame);
            }
        }

        public void TryStartGame()
        {
            if (LivesManager.Instance == null)
            {
                StartGameWithCountdown();
                return;
            }

            if (LivesManager.Instance.TryUseLife())
            {
                StartGameWithCountdown();
            }
            else
            {
                ShowNoLivesPopup();
            }
        }

        private void StartGameWithCountdown()
        {
            if (useCountdown && UI.CountdownController.Instance != null)
            {
                UI.UIManager.Instance?.HideMainMenu();
                
                UI.CountdownController.Instance.StartCountdown(() =>
                {
                    StartGame();
                });
            }
            else
            {
                StartGame();
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
            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.ShowNoLivesPopup();
            }
        }
    }
}
