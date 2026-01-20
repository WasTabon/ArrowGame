using UnityEngine;
using DG.Tweening;

namespace ArrowGame.Audio
{
    public class AudioSlowMotion : MonoBehaviour
    {
        public static AudioSlowMotion Instance { get; private set; }

        [Header("Low Pass Filter")]
        [SerializeField] private AudioLowPassFilter lowPassFilter;
        [SerializeField] private float normalCutoffFrequency = 22000f;
        [SerializeField] private float slowMoCutoffFrequency = 800f;

        [Header("Pitch")]
        [SerializeField] private bool adjustPitch = true;
        [SerializeField] private float normalPitch = 1f;
        [SerializeField] private float slowMoPitch = 0.7f;

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 0.1f;

        private AudioSource targetAudioSource;
        private Tween filterTween;
        private Tween pitchTween;
        private bool isSlowMo;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SetupFilter();
        }

        private void SetupFilter()
        {
            if (lowPassFilter == null)
            {
                if (AudioManager.Instance != null)
                {
                    targetAudioSource = AudioManager.Instance.GetComponent<AudioSource>();
                    if (targetAudioSource == null)
                    {
                        targetAudioSource = AudioManager.Instance.GetComponentInChildren<AudioSource>();
                    }
                }

                if (targetAudioSource == null)
                {
                    targetAudioSource = FindObjectOfType<AudioSource>();
                }

                if (targetAudioSource != null)
                {
                    lowPassFilter = targetAudioSource.gameObject.GetComponent<AudioLowPassFilter>();
                    if (lowPassFilter == null)
                    {
                        lowPassFilter = targetAudioSource.gameObject.AddComponent<AudioLowPassFilter>();
                    }
                }
            }

            if (lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = normalCutoffFrequency;
            }
        }

        public void EnableSlowMotion()
        {
            if (isSlowMo) return;
            isSlowMo = true;

            filterTween?.Kill();
            pitchTween?.Kill();

            if (lowPassFilter != null)
            {
                filterTween = DOTween.To(
                    () => lowPassFilter.cutoffFrequency,
                    x => lowPassFilter.cutoffFrequency = x,
                    slowMoCutoffFrequency,
                    transitionDuration
                ).SetUpdate(true);
            }

            if (adjustPitch && targetAudioSource != null)
            {
                pitchTween = DOTween.To(
                    () => targetAudioSource.pitch,
                    x => targetAudioSource.pitch = x,
                    slowMoPitch,
                    transitionDuration
                ).SetUpdate(true);
            }
        }

        public void DisableSlowMotion()
        {
            if (!isSlowMo) return;
            isSlowMo = false;

            filterTween?.Kill();
            pitchTween?.Kill();

            if (lowPassFilter != null)
            {
                filterTween = DOTween.To(
                    () => lowPassFilter.cutoffFrequency,
                    x => lowPassFilter.cutoffFrequency = x,
                    normalCutoffFrequency,
                    transitionDuration
                ).SetUpdate(true);
            }

            if (adjustPitch && targetAudioSource != null)
            {
                pitchTween = DOTween.To(
                    () => targetAudioSource.pitch,
                    x => targetAudioSource.pitch = x,
                    normalPitch,
                    transitionDuration
                ).SetUpdate(true);
            }
        }

        public void SetSlowMotion(bool enabled)
        {
            if (enabled)
            {
                EnableSlowMotion();
            }
            else
            {
                DisableSlowMotion();
            }
        }

        public void SetTargetAudioSource(AudioSource source)
        {
            targetAudioSource = source;

            if (lowPassFilter != null && lowPassFilter.gameObject != source.gameObject)
            {
                Destroy(lowPassFilter);
            }

            lowPassFilter = source.gameObject.GetComponent<AudioLowPassFilter>();
            if (lowPassFilter == null)
            {
                lowPassFilter = source.gameObject.AddComponent<AudioLowPassFilter>();
            }

            lowPassFilter.cutoffFrequency = normalCutoffFrequency;
        }

        private void OnDestroy()
        {
            filterTween?.Kill();
            pitchTween?.Kill();

            if (lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = normalCutoffFrequency;
            }

            if (targetAudioSource != null)
            {
                targetAudioSource.pitch = normalPitch;
            }
        }
    }
}
