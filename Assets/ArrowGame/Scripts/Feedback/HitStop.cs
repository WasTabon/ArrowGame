using UnityEngine;
using System.Collections;

namespace ArrowGame.Feedback
{
    public class HitStop : MonoBehaviour
    {
        public static HitStop Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float coreDuration = 0.08f;
        [SerializeField] private float innerDuration = 0.04f;
        [SerializeField] private float missDuration = 0.12f;

        [Header("Time Scale")]
        [SerializeField] private float freezeTimeScale = 0.02f;

        private Coroutine hitStopCoroutine;
        private bool isHitStopping;

        public bool IsHitStopping => isHitStopping;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void DoHitStop(float duration)
        {
            if (hitStopCoroutine != null)
            {
                StopCoroutine(hitStopCoroutine);
                Time.timeScale = 1f;
            }

            hitStopCoroutine = StartCoroutine(HitStopCoroutine(duration));
        }

        public void DoHitStopForZone(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core:
                    DoHitStop(coreDuration);
                    break;
                case Hit.HitZone.Inner:
                    DoHitStop(innerDuration);
                    break;
                case Hit.HitZone.Miss:
                    DoHitStop(missDuration);
                    break;
            }
        }

        private IEnumerator HitStopCoroutine(float duration)
        {
            isHitStopping = true;
            Time.timeScale = freezeTimeScale;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            isHitStopping = false;
            hitStopCoroutine = null;
        }

        public void Cancel()
        {
            if (hitStopCoroutine != null)
            {
                StopCoroutine(hitStopCoroutine);
                hitStopCoroutine = null;
            }
            Time.timeScale = 1f;
            isHitStopping = false;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
