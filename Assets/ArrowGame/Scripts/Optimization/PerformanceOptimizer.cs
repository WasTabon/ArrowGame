using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ArrowGame.Optimization
{
    public class PerformanceOptimizer : MonoBehaviour
    {
        public static PerformanceOptimizer Instance { get; private set; }

        [Header("Target")]
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private int minAcceptableFPS = 45;

        [Header("Monitoring")]
        [SerializeField] private float checkInterval = 3f;
        [SerializeField] private int sampleCount = 10;

        [Header("References")]
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;

        [Header("Quality Levels")]
        [SerializeField] private bool autoAdjustQuality = true;

        private float[] fpsSamples;
        private int sampleIndex;
        private float timer;
        private int currentQualityLevel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Application.targetFrameRate = targetFPS;
            fpsSamples = new float[sampleCount];
            currentQualityLevel = QualitySettings.GetQualityLevel();
        }

        private void Update()
        {
            if (!autoAdjustQuality) return;

            fpsSamples[sampleIndex] = 1f / Time.unscaledDeltaTime;
            sampleIndex = (sampleIndex + 1) % sampleCount;

            timer += Time.unscaledDeltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                EvaluatePerformance();
            }
        }

        private void EvaluatePerformance()
        {
            float avgFPS = GetAverageFPS();

            if (avgFPS < minAcceptableFPS)
            {
                DecreaseQuality();
            }
            else if (avgFPS > targetFPS + 10 && currentQualityLevel < QualitySettings.names.Length - 1)
            {
                IncreaseQuality();
            }
        }

        private float GetAverageFPS()
        {
            float sum = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                sum += fpsSamples[i];
            }
            return sum / sampleCount;
        }

        private void DecreaseQuality()
        {
            if (currentQualityLevel > 0)
            {
                currentQualityLevel--;
                QualitySettings.SetQualityLevel(currentQualityLevel);
                Debug.Log($"[Performance] Decreased quality to: {QualitySettings.names[currentQualityLevel]}");
            }
            else
            {
                ApplyEmergencyOptimizations();
            }
        }

        private void IncreaseQuality()
        {
            if (currentQualityLevel < QualitySettings.names.Length - 1)
            {
                currentQualityLevel++;
                QualitySettings.SetQualityLevel(currentQualityLevel);
                Debug.Log($"[Performance] Increased quality to: {QualitySettings.names[currentQualityLevel]}");
            }
        }

        private void ApplyEmergencyOptimizations()
        {
            if (postProcessVolume != null)
            {
                postProcessVolume.enabled = false;
            }

            if (urpAsset != null)
            {
                urpAsset.renderScale = 0.75f;
            }

            Debug.Log("[Performance] Applied emergency optimizations");
        }

        public void SetPostProcessing(bool enabled)
        {
            if (postProcessVolume != null)
            {
                postProcessVolume.enabled = enabled;
            }
        }

        public void SetRenderScale(float scale)
        {
            if (urpAsset != null)
            {
                urpAsset.renderScale = Mathf.Clamp(scale, 0.5f, 1f);
            }
        }
    }
}
