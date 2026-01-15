using UnityEngine;

namespace ArrowGame.Hit
{
    [CreateAssetMenu(fileName = "HitZoneConfig", menuName = "ArrowGame/Hit Zone Config")]
    public class HitZoneConfig : ScriptableObject
    {
        [Header("Zone Radiuses (from center)")]
        [Tooltip("Max distance for Core zone")]
        public float coreRadius = 0.3f;
        
        [Tooltip("Max distance for Inner zone")]
        public float innerRadius = 0.7f;
        
        [Tooltip("Max distance for Middle zone")]
        public float middleRadius = 1.2f;
        
        [Tooltip("Max distance for Outer zone")]
        public float outerRadius = 1.8f;

        public HitZone GetZoneByDistance(float distance)
        {
            if (distance <= coreRadius) return HitZone.Core;
            if (distance <= innerRadius) return HitZone.Inner;
            if (distance <= middleRadius) return HitZone.Middle;
            if (distance <= outerRadius) return HitZone.Outer;
            return HitZone.Miss;
        }
    }
}
