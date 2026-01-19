using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ArrowGame.UI
{
    public class PauseController : MonoBehaviour
    {
        public static PauseController Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.2f;
        [SerializeField] private float fadeOutDuration = 0.15f;

        private bool isPaused;
        private Tween fadeTween;

        public bool IsPaused => isPaused;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        private void Start()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(TogglePause);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(Restart);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (Core.GameManager.Instance != null)
                {
                    var state = Core.GameManager.Instance.CurrentState;
                    if (state == Core.GameState.Run || state == Core.GameState.Pause)
                    {
                        TogglePause();
                    }
                }
            }
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (isPaused) return;
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            isPaused = true;
            Time.timeScale = 0f;
            Core.GameManager.Instance.PauseGame();

            ShowPausePanel();
        }

        public void Resume()
        {
            if (!isPaused) return;

            isPaused = false;
            
            HidePausePanel(() =>
            {
                Time.timeScale = 1f;
                Core.GameManager.Instance?.ResumeGame();
            });
        }

        private void ShowPausePanel()
        {
            if (pausePanel == null) return;

            fadeTween?.Kill();
            pausePanel.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                fadeTween = canvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true);
            }
        }

        private void HidePausePanel(System.Action onComplete = null)
        {
            if (pausePanel == null)
            {
                onComplete?.Invoke();
                return;
            }

            fadeTween?.Kill();

            if (canvasGroup != null)
            {
                fadeTween = canvasGroup.DOFade(0f, fadeOutDuration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        pausePanel.SetActive(false);
                        onComplete?.Invoke();
                    });
            }
            else
            {
                pausePanel.SetActive(false);
                onComplete?.Invoke();
            }
        }

        private void Restart()
        {
            isPaused = false;
            Time.timeScale = 1f;
            
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.FadeIn(() =>
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                    );
                });
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }

        private void GoToMainMenu()
        {
            isPaused = false;
            Time.timeScale = 1f;
            
            HidePausePanel(() =>
            {
                Core.GameManager.Instance?.GoToMainMenu();
            });
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
