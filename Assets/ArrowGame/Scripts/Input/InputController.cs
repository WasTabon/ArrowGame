using UnityEngine;
using System;

namespace ArrowGame.Input
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }

        public event Action OnInputStart;
        public event Action OnInputEnd;
        public event Action<float> OnInputHold;

        private bool isHolding = false;
        private float holdDuration = 0f;

        public bool IsHolding => isHolding;
        public float HoldDuration => holdDuration;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Run) return;

            HandleInput();
        }

        private void HandleInput()
        {
            bool inputDown = UnityEngine.Input.GetMouseButtonDown(0) || GetTouchBegan();
            bool inputHeld = UnityEngine.Input.GetMouseButton(0) || GetTouchHeld();
            bool inputUp = UnityEngine.Input.GetMouseButtonUp(0) || GetTouchEnded();

            if (inputDown && !isHolding)
            {
                isHolding = true;
                holdDuration = 0f;
                OnInputStart?.Invoke();
            }

            if (inputHeld && isHolding)
            {
                holdDuration += Time.deltaTime;
                OnInputHold?.Invoke(holdDuration);
            }

            if (inputUp && isHolding)
            {
                isHolding = false;
                OnInputEnd?.Invoke();
                holdDuration = 0f;
            }
        }

        private bool GetTouchBegan()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                return UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began;
            }
            return false;
        }

        private bool GetTouchHeld()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                TouchPhase phase = UnityEngine.Input.GetTouch(0).phase;
                return phase == TouchPhase.Stationary || phase == TouchPhase.Moved;
            }
            return false;
        }

        private bool GetTouchEnded()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                TouchPhase phase = UnityEngine.Input.GetTouch(0).phase;
                return phase == TouchPhase.Ended || phase == TouchPhase.Canceled;
            }
            return false;
        }
    }
}
