using System;
using UnityEngine;

namespace ArrowGame.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Run,
        Pause,
        GameOver,
        Results
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        [SerializeField] private GameState currentState = GameState.Boot;

        public GameState CurrentState => currentState;

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
            SetState(GameState.Run);
        }

        public void SetState(GameState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            OnGameStateChanged?.Invoke(currentState);
        }

        public void StartRun()
        {
            SetState(GameState.Run);
        }

        public void PauseGame()
        {
            if (currentState == GameState.Run)
                SetState(GameState.Pause);
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Pause)
                SetState(GameState.Run);
        }

        public void GameOver()
        {
            SetState(GameState.GameOver);
        }

        public void ShowResults()
        {
            SetState(GameState.Results);
        }

        public void GoToMainMenu()
        {
            SetState(GameState.MainMenu);
        }
    }
}
