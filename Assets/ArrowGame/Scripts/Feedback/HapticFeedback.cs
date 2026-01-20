using UnityEngine;

namespace ArrowGame.Feedback
{
    public static class HapticFeedback
    {
        private static bool IsEnabled
        {
            get
            {
                if (Settings.SettingsManager.Instance != null)
                {
                    return Settings.SettingsManager.Instance.VibrationEnabled;
                }
                return true;
            }
        }

        public static void Vibrate()
        {
            if (!IsEnabled) return;

#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }

        public static void LightImpact()
        {
            if (!IsEnabled) return;

#if UNITY_IOS
            IOSHaptic(1);
#elif UNITY_ANDROID
            AndroidVibrate(20);
#endif
        }

        public static void MediumImpact()
        {
            if (!IsEnabled) return;

#if UNITY_IOS
            IOSHaptic(2);
#elif UNITY_ANDROID
            AndroidVibrate(40);
#endif
        }

        public static void HeavyImpact()
        {
            if (!IsEnabled) return;

#if UNITY_IOS
            IOSHaptic(3);
#elif UNITY_ANDROID
            AndroidVibrate(60);
#endif
        }

        public static void Success()
        {
            if (!IsEnabled) return;

#if UNITY_IOS
            IOSHaptic(4);
#elif UNITY_ANDROID
            AndroidVibrate(50);
#endif
        }

        public static void Warning()
        {
            if (!IsEnabled) return;

#if UNITY_IOS
            IOSHaptic(5);
#elif UNITY_ANDROID
            AndroidVibrate(80);
#endif
        }

        public static void Error()
        {
            if (!IsEnabled) return;

#if UNITY_IOS
            IOSHaptic(6);
#elif UNITY_ANDROID
            AndroidVibrate(100);
#endif
        }

        private static void IOSHaptic(int type)
        {
#if UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

        private static void AndroidVibrate(long milliseconds)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                    
                    if (vibrator != null)
                    {
                        vibrator.Call("vibrate", milliseconds);
                    }
                }
            }
            catch (System.Exception)
            {
                Handheld.Vibrate();
            }
#endif
        }
    }
}
