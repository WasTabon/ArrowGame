using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ArrowGame.Lives
{
    public class LivesDisplay : MonoBehaviour
    {
        [Header("Lives Display")]
        [SerializeField] private TextMeshProUGUI livesText;
        [SerializeField] private Image[] lifeIcons;
        [SerializeField] private Color activeLifeColor = Color.white;
        [SerializeField] private Color emptyLifeColor = new Color(1f, 1f, 1f, 0.3f);

        [Header("Timer Display")]
        [SerializeField] private GameObject timerContainer;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerFillBar;

        [Header("Animation")]
        [SerializeField] private float punchScale = 1.3f;
        [SerializeField] private float punchDuration = 0.3f;

        private bool initialized = false;

        private void Start()
        {
            TryInitialize();
        }

        private void Update()
        {
            if (!initialized)
            {
                TryInitialize();
            }
        }

        private void TryInitialize()
        {
            if (LivesManager.Instance != null)
            {
                LivesManager.Instance.OnLivesChanged += UpdateLivesDisplay;
                LivesManager.Instance.OnRegenTimerUpdated += UpdateTimerDisplay;
                LivesManager.Instance.OnLifeRegenerated += PlayRegenAnimation;

                UpdateLivesDisplay(LivesManager.Instance.CurrentLives);
                UpdateTimerVisibility();
                initialized = true;
            }
        }

        private void OnDisable()
        {
            if (LivesManager.Instance != null)
            {
                LivesManager.Instance.OnLivesChanged -= UpdateLivesDisplay;
                LivesManager.Instance.OnRegenTimerUpdated -= UpdateTimerDisplay;
                LivesManager.Instance.OnLifeRegenerated -= PlayRegenAnimation;
            }
        }

        public void UpdateLivesDisplay(int lives)
        {
            if (lifeIcons != null)
            {
                for (int i = 0; i < lifeIcons.Length; i++)
                {
                    if (lifeIcons[i] != null)
                    {
                        lifeIcons[i].color = i < lives ? activeLifeColor : emptyLifeColor;
                        bool isActive = i < lives;
                        Debug.Log($"Life is active: {isActive}");
                    }
                }
            }

            UpdateTimerVisibility();
        }

        private void UpdateTimerDisplay(float remainingSeconds)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
                int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }

            if (timerFillBar != null && LivesManager.Instance != null)
            {
                timerFillBar.fillAmount = LivesManager.Instance.RegenProgress;
            }
        }

        private void UpdateTimerVisibility()
        {
            if (timerContainer != null && LivesManager.Instance != null)
            {
                bool showTimer = !LivesManager.Instance.IsFullLives;
                timerContainer.SetActive(showTimer);
            }
        }

        private void PlayRegenAnimation()
        {
            if (livesText != null)
            {
                livesText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
            }

            if (lifeIcons != null && LivesManager.Instance != null)
            {
                int newLifeIndex = LivesManager.Instance.CurrentLives - 1;
                if (newLifeIndex >= 0 && newLifeIndex < lifeIcons.Length && lifeIcons[newLifeIndex] != null)
                {
                    lifeIcons[newLifeIndex].transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
                }
            }
        }
    }
}
