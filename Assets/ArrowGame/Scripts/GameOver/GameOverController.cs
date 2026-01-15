using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ArrowGame.GameOver
{
    public class GameOverController : MonoBehaviour
    {
        public static GameOverController Instance { get; private set; }

        [SerializeField] private GameOverConfig config;
        [SerializeField] private Image fadeImage;
        [SerializeField] private GameObject gameOverScreen;

        private Sequence gameOverSequence;

        public event Action OnGameOverStarted;
        public event Action OnGameOverComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (fadeImage != null)
            {
                fadeImage.color = Color.clear;
                fadeImage.gameObject.SetActive(false);
            }

            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(false);
            }
        }

        private void Start()
        {
            if (Speed.SpeedController.Instance != null)
            {
                Speed.SpeedController.Instance.OnSpeedReachedZero += TriggerGameOver;
            }
        }

        private void OnEnable()
        {
            if (Speed.SpeedController.Instance != null)
            {
                Speed.SpeedController.Instance.OnSpeedReachedZero += TriggerGameOver;
            }
        }

        private void OnDisable()
        {
            if (Speed.SpeedController.Instance != null)
            {
                Speed.SpeedController.Instance.OnSpeedReachedZero -= TriggerGameOver;
            }
        }

        public void TriggerGameOver()
        {
            if (Core.GameManager.Instance.CurrentState == Core.GameState.GameOver) return;

            Core.GameManager.Instance.GameOver();
            OnGameOverStarted?.Invoke();

            PlayGameOverSequence();
        }

        private void PlayGameOverSequence()
        {
            gameOverSequence?.Kill();
            gameOverSequence = DOTween.Sequence();

            gameOverSequence.Append(
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, config.slowMotionTimeScale, config.slowMotionDuration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
            );

            if (fadeImage != null)
            {
                fadeImage.gameObject.SetActive(true);
                gameOverSequence.Join(
                    fadeImage.DOColor(Color.black, config.fadeToBlackDuration)
                        .SetDelay(config.slowMotionDuration - config.fadeToBlackDuration)
                        .SetUpdate(true)
                );
            }

            gameOverSequence.AppendCallback(() =>
            {
                Time.timeScale = 1f;
                ShowGameOverScreen();
            });

            gameOverSequence.SetUpdate(true);
        }

        private void ShowGameOverScreen()
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(true);
                
                CanvasGroup canvasGroup = gameOverScreen.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
                }
            }

            OnGameOverComplete?.Invoke();
        }

        public void RestartGame()
        {
            gameOverSequence?.Kill();
            Time.timeScale = 1f;

            if (fadeImage != null)
            {
                fadeImage.color = Color.clear;
                fadeImage.gameObject.SetActive(false);
            }

            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(false);
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }

        public void GoToMainMenu()
        {
            gameOverSequence?.Kill();
            Time.timeScale = 1f;

            Core.GameManager.Instance.GoToMainMenu();
        }

        private void OnDestroy()
        {
            gameOverSequence?.Kill();
            Time.timeScale = 1f;
        }
    }
}
