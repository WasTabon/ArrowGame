using UnityEngine;

namespace ArrowGame.Score
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "ArrowGame/Score Config")]
    public class ScoreConfig : ScriptableObject
    {
        [Header("Base Points Per Zone")]
        public int corePoints = 100;
        public int innerPoints = 50;
        public int middlePoints = 25;
        public int outerPoints = 10;

        [Header("Streak Settings")]
        public int streakForMultiplier2x = 5;
        public int streakForMultiplier3x = 10;
        public int streakForMultiplier4x = 20;
        public int streakForMultiplier5x = 35;

        [Header("Streak Rules")]
        public bool outerBreaksStreak = false;
        public bool outerWeakensStreak = true;
        public int outerStreakPenalty = 2;

        public int GetPointsForZone(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core: return corePoints;
                case Hit.HitZone.Inner: return innerPoints;
                case Hit.HitZone.Middle: return middlePoints;
                case Hit.HitZone.Outer: return outerPoints;
                default: return 0;
            }
        }

        public int GetMultiplierForStreak(int streak)
        {
            if (streak >= streakForMultiplier5x) return 5;
            if (streak >= streakForMultiplier4x) return 4;
            if (streak >= streakForMultiplier3x) return 3;
            if (streak >= streakForMultiplier2x) return 2;
            return 1;
        }

        public float GetStreakIntensity(int streak)
        {
            if (streak >= streakForMultiplier5x) return 1f;
            if (streak >= streakForMultiplier4x) return 0.8f;
            if (streak >= streakForMultiplier3x) return 0.6f;
            if (streak >= streakForMultiplier2x) return 0.4f;
            if (streak > 0) return 0.2f;
            return 0f;
        }
    }
}
