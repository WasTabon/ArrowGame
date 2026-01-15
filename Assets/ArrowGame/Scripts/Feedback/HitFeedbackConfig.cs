using UnityEngine;

namespace ArrowGame.Feedback
{
    [CreateAssetMenu(fileName = "HitFeedbackConfig", menuName = "ArrowGame/Hit Feedback Config")]
    public class HitFeedbackConfig : ScriptableObject
    {
        [Header("Screen Flash")]
        public Color coreFlashColor = new Color(1f, 1f, 0f, 0.5f);
        public Color innerFlashColor = new Color(0f, 1f, 0f, 0.4f);
        public Color middleFlashColor = new Color(0f, 0.8f, 1f, 0.3f);
        public Color outerFlashColor = new Color(1f, 1f, 1f, 0.2f);
        public Color missFlashColor = new Color(1f, 0f, 0f, 0.3f);
        public float flashDuration = 0.15f;

        [Header("Camera Shake")]
        public float coreShakeStrength = 0.3f;
        public float innerShakeStrength = 0.2f;
        public float middleShakeStrength = 0.1f;
        public float outerShakeStrength = 0.05f;
        public float missShakeStrength = 0.4f;
        public float shakeDuration = 0.2f;

        [Header("Ring Scale Punch")]
        public float scalePunchStrength = 0.3f;
        public float scalePunchDuration = 0.2f;

        [Header("Haptic")]
        public bool hapticEnabled = true;

        public Color GetFlashColor(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core: return coreFlashColor;
                case Hit.HitZone.Inner: return innerFlashColor;
                case Hit.HitZone.Middle: return middleFlashColor;
                case Hit.HitZone.Outer: return outerFlashColor;
                case Hit.HitZone.Miss: return missFlashColor;
                default: return Color.clear;
            }
        }

        public float GetShakeStrength(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core: return coreShakeStrength;
                case Hit.HitZone.Inner: return innerShakeStrength;
                case Hit.HitZone.Middle: return middleShakeStrength;
                case Hit.HitZone.Outer: return outerShakeStrength;
                case Hit.HitZone.Miss: return missShakeStrength;
                default: return 0f;
            }
        }
    }
}
