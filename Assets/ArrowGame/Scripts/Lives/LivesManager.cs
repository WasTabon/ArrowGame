using UnityEngine;
using System;

namespace ArrowGame.Lives
{
    public class LivesManager : MonoBehaviour
    {
        public static LivesManager Instance { get; private set; }

        [SerializeField] private LivesConfig config;

        private int currentLives;
        private float regenTimer;
        private DateTime lastRegenTime;

        private const string LIVES_KEY = "ArrowGame_Lives";
        private const string REGEN_TIME_KEY = "ArrowGame_RegenTime";

        public int CurrentLives => currentLives;
        public int MaxLives => config.maxLives;
        public float RegenTimer => regenTimer;
        public float RegenProgress => 1f - (regenTimer / config.RegenerationTimeSeconds);
        public bool CanPlay => currentLives > 0;
        public bool IsFullLives => currentLives >= config.maxLives;

        public event Action<int> OnLivesChanged;
        public event Action<float> OnRegenTimerUpdated;
        public event Action OnLifeRegenerated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("LivesConfig not assigned to LivesManager!");
                return;
            }

            LoadState();
            CalculateOfflineRegeneration();
        }

        private void Update()
        {
            if (currentLives < config.maxLives)
            {
                UpdateRegenTimer();
            }
        }

        private void UpdateRegenTimer()
        {
            regenTimer -= Time.deltaTime;
            OnRegenTimerUpdated?.Invoke(regenTimer);

            if (regenTimer <= 0f)
            {
                RegenerateLife();
            }
        }

        [ContextMenu("Debug: Set Lives to 0")]
        public void DebugSetLivesZero()
        {
            DebugSetLives(0);
        }
        
        public void DebugSetLives(int amount)
        {
            currentLives = Mathf.Clamp(amount, 0, config.maxLives);
            
            if (currentLives < config.maxLives && regenTimer <= 0)
            {
                regenTimer = config.RegenerationTimeSeconds;
            }
            
            OnLivesChanged?.Invoke(currentLives);
            SaveState();
            Debug.Log($"[DEBUG] Lives set to: {currentLives}");
        }
        
        private void RegenerateLife()
        {
            currentLives = Mathf.Min(currentLives + 1, config.maxLives);
            OnLivesChanged?.Invoke(currentLives);
            OnLifeRegenerated?.Invoke();

            if (currentLives < config.maxLives)
            {
                regenTimer = config.RegenerationTimeSeconds;
                lastRegenTime = DateTime.Now;
            }
            else
            {
                regenTimer = 0f;
            }

            SaveState();
        }

        private void CalculateOfflineRegeneration()
        {
            if (currentLives >= config.maxLives) return;

            string savedTimeStr = PlayerPrefs.GetString(REGEN_TIME_KEY, "");
            if (string.IsNullOrEmpty(savedTimeStr)) return;

            if (DateTime.TryParse(savedTimeStr, out DateTime savedTime))
            {
                TimeSpan elapsed = DateTime.Now - savedTime;
                float elapsedSeconds = (float)elapsed.TotalSeconds;

                int livesFromOffline = Mathf.FloorToInt(elapsedSeconds / config.RegenerationTimeSeconds);
                float remainingSeconds = elapsedSeconds % config.RegenerationTimeSeconds;

                if (livesFromOffline > 0)
                {
                    currentLives = Mathf.Min(currentLives + livesFromOffline, config.maxLives);
                    OnLivesChanged?.Invoke(currentLives);
                }

                if (currentLives < config.maxLives)
                {
                    regenTimer = config.RegenerationTimeSeconds - remainingSeconds;
                    lastRegenTime = DateTime.Now;
                }
                else
                {
                    regenTimer = 0f;
                }

                SaveState();
            }
        }

        public bool TryUseLife()
        {
            if (currentLives <= 0)
            {
                return false;
            }

            bool wasFullLives = currentLives >= config.maxLives;

            currentLives--;
            OnLivesChanged?.Invoke(currentLives);

            if (wasFullLives && currentLives < config.maxLives)
            {
                regenTimer = config.RegenerationTimeSeconds;
                lastRegenTime = DateTime.Now;
            }

            SaveState();
            return true;
        }

        public void AddLife(int amount = 1)
        {
            currentLives = Mathf.Min(currentLives + amount, config.maxLives);
            OnLivesChanged?.Invoke(currentLives);

            if (currentLives >= config.maxLives)
            {
                regenTimer = 0f;
            }

            SaveState();
        }

        public void RefillLives()
        {
            currentLives = config.maxLives;
            regenTimer = 0f;
            OnLivesChanged?.Invoke(currentLives);
            SaveState();
        }

        private void SaveState()
        {
            PlayerPrefs.SetInt(LIVES_KEY, currentLives);
            PlayerPrefs.SetString(REGEN_TIME_KEY, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        private void LoadState()
        {
            if (PlayerPrefs.HasKey(LIVES_KEY))
            {
                currentLives = PlayerPrefs.GetInt(LIVES_KEY);
            }
            else
            {
                currentLives = config.startingLives;
            }

            regenTimer = config.RegenerationTimeSeconds;
            OnLivesChanged?.Invoke(currentLives);
        }

        public void ResetAllData()
        {
            PlayerPrefs.DeleteKey(LIVES_KEY);
            PlayerPrefs.DeleteKey(REGEN_TIME_KEY);
            currentLives = config.startingLives;
            regenTimer = 0f;
            OnLivesChanged?.Invoke(currentLives);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveState();
            }
            else
            {
                CalculateOfflineRegeneration();
            }
        }

        private void OnApplicationQuit()
        {
            SaveState();
        }
    }
}
