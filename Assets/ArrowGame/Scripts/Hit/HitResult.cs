using UnityEngine;

namespace ArrowGame.Hit
{
    public struct HitResult
    {
        public HitZone Zone;
        public float Distance;
        public Vector3 HitPosition;
        public Ring.RingController Ring;

        public bool IsPerfect => Zone == HitZone.Core;
        public bool IsSuccess => Zone != HitZone.Miss;
    }
}
