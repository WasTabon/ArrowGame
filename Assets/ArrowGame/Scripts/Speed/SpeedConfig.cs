using UnityEngine;

namespace ArrowGame.Speed
{
    [CreateAssetMenu(fileName = "SpeedConfig", menuName = "ArrowGame/Speed Config")]
    public class SpeedConfig : ScriptableObject
    {
        [Header("Base Speed")]
        public float startSpeed = 10f;
        public float minSpeed = 0f;
        public float maxSpeed = 30f;

        [Header("Speed Changes Per Zone")]
        public float coreSpeedBonus = 2f;
        public float innerSpeedBonus = 1f;
        public float middleSpeedBonus = 0.5f;
        public float outerSpeedPenalty = -1f;
        public float missSpeedPenalty = -3f;

        [Header("Animation")]
        public float speedChangeDuration = 0.3f;

        public float GetSpeedChange(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core: return coreSpeedBonus;
                case Hit.HitZone.Inner: return innerSpeedBonus;
                case Hit.HitZone.Middle: return middleSpeedBonus;
                case Hit.HitZone.Outer: return outerSpeedPenalty;
                case Hit.HitZone.Miss: return missSpeedPenalty;
                default: return 0f;
            }
        }
    }
}
