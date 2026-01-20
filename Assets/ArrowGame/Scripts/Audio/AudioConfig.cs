using UnityEngine;

namespace ArrowGame.Audio
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "ArrowGame/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        [Header("Music")]
        public AudioClip menuMusic;
        public AudioClip gameMusic;
        [Range(0f, 1f)] public float musicVolume = 0.5f;
        public float musicFadeDuration = 1f;

        [Header("Hit Sounds")]
        public AudioClip coreHitSound;
        public AudioClip innerHitSound;
        public AudioClip middleHitSound;
        public AudioClip outerHitSound;
        public AudioClip missSound;
        [Range(0f, 1f)] public float hitSoundVolume = 0.8f;

        [Header("UI Sounds")]
        public AudioClip buttonClickSound;
        public AudioClip countdownTickSound;
        public AudioClip countdownGoSound;
        public AudioClip gameOverSound;
        public AudioClip newHighScoreSound;
        [Range(0f, 1f)] public float uiSoundVolume = 0.7f;

        [Header("Streak Sounds")]
        public AudioClip streakUpSound;
        public AudioClip streakBreakSound;
        public AudioClip multiplierUpSound;
        [Range(0f, 1f)] public float streakSoundVolume = 0.6f;

        [Header("Streak Music Layers")]
        public AudioClip[] streakLayers;
        [Range(0f, 1f)] public float layerVolume = 0.4f;
        public float layerFadeDuration = 0.5f;

        public AudioClip GetHitSound(Hit.HitZone zone)
        {
            switch (zone)
            {
                case Hit.HitZone.Core: return coreHitSound;
                case Hit.HitZone.Inner: return innerHitSound;
                case Hit.HitZone.Middle: return middleHitSound;
                case Hit.HitZone.Outer: return outerHitSound;
                case Hit.HitZone.Miss: return missSound;
                default: return null;
            }
        }
    }
}
