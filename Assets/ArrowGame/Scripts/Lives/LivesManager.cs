using UnityEngine;
using System;

namespace ArrowGame.Lives
{
    public class LivesManager : MonoBehaviour
    {
        public static LivesManager Instance { get; private set; }

        public LivesDisplay _livesDisplay;
        
        [SerializeField] private LivesConfig config;

        private int currentLives;
        private float regenTimer;

        private const string LIVES_KEY = "ArrowGame_Lives";

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
            Debug.Log($"[LivesManager] Awake. Instance before: {Instance}");
    
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
    
            Debug.Log("[LivesManager] Instance set");
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("LivesConfig not assigned to LivesManager!");
                return;
            }

            Debug.Log($"[LivesManager] Start. Config: {config}");
            
            LoadState();
        }

        private void Update()
        {
            if (currentLives < config.maxLives && regenTimer > 0f)
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
            }
            else
            {
                regenTimer = 0f;
            }

            SaveState();
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
            }

            SaveState();
            
            Debug.Log($"[LivesManager] Life used. Current: {currentLives}");
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
            PlayerPrefs.Save();
            Debug.Log($"[LivesManager] Saved: lives={currentLives}");
        }

        private void LoadState()
        {
            if (PlayerPrefs.HasKey(LIVES_KEY))
            {
                currentLives = PlayerPrefs.GetInt(LIVES_KEY);
                Debug.Log($"[LivesManager] Loaded from PlayerPrefs: {currentLives}");
            }
            else
            {
                currentLives = config.startingLives;
                SaveState();
                Debug.Log($"[LivesManager] First launch, set to startingLives: {currentLives}");
            }

            // Устанавливаем таймер только если жизни неполные
            if (currentLives < config.maxLives)
            {
                regenTimer = config.RegenerationTimeSeconds;
            }
            else
            {
                regenTimer = 0f;
            }

            OnLivesChanged?.Invoke(currentLives);
        }

        public void ResetAllData()
        {
            PlayerPrefs.DeleteKey(LIVES_KEY);
            PlayerPrefs.Save();
            currentLives = config.startingLives;
            regenTimer = 0f;
            OnLivesChanged?.Invoke(currentLives);
            Debug.Log("[LivesManager] Data reset");
        }

        private void OnApplicationQuit()
        {
            SaveState();
        }
    }
}