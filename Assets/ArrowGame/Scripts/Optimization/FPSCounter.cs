using UnityEngine;
using TMPro;

namespace ArrowGame.Optimization
{
    public class FPSCounter : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private bool showInBuild = false;

        [Header("Settings")]
        [SerializeField] private float updateInterval = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color goodColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color badColor = Color.red;
        [SerializeField] private int goodFPS = 55;
        [SerializeField] private int warningFPS = 30;

        private float deltaTime;
        private float timer;

        private void Start()
        {
#if !UNITY_EDITOR
            if (!showInBuild && fpsText != null)
            {
                fpsText.gameObject.SetActive(false);
                enabled = false;
            }
#endif
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            timer += Time.unscaledDeltaTime;

            if (timer >= updateInterval)
            {
                timer = 0f;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (fpsText == null) return;

            int fps = Mathf.RoundToInt(1f / deltaTime);
            fpsText.text = $"FPS: {fps}";

            if (fps >= goodFPS)
                fpsText.color = goodColor;
            else if (fps >= warningFPS)
                fpsText.color = warningColor;
            else
                fpsText.color = badColor;
        }

        public int GetCurrentFPS()
        {
            return Mathf.RoundToInt(1f / deltaTime);
        }
    }
}
