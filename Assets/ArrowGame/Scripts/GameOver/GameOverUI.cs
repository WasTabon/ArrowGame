using UnityEngine;
using UnityEngine.UI;

namespace ArrowGame.GameOver
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }
        }

        private void OnRestartClicked()
        {
            if (GameOverController.Instance != null)
            {
                GameOverController.Instance.RestartGame();
            }
        }

        private void OnMainMenuClicked()
        {
            if (GameOverController.Instance != null)
            {
                GameOverController.Instance.GoToMainMenu();
            }
        }
    }
}
