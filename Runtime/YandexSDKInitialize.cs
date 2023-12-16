using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_WEBGL && !UNITY_EDITOR
using Agava.YandexGames;
#endif

namespace Kimicu.YandexGames
{
    public sealed class YandexSDKInitialize : MonoBehaviour
    {
        public UnityEvent OnInitialize = new();

        private KimicuYandexSettings Settings => KimicuYandexSettings.Instance;

        private IEnumerator Start()
        {
            if(Settings == null) Debug.LogWarning($"Settings not loaded!");
            #if !UNITY_EDITOR && UNITY_WEBGL
            if(Settings.InitializeDebugEnabled) Debug.Log($"YandexGamesSdk Start Initialize. {Time.time}");
            yield return YandexGamesSdk.Initialize(); // Initialize Agava SDK.
            if(Settings.InitializeDebugEnabled) Debug.Log($"YandexGamesSdk End Initialize. {Time.time}");
            #endif

            if (Settings.InitializeDebugEnabled) Debug.Log($"WebGL Start Initialize. {Time.time}");
            WebGL.InitializeListener(); // Initialize WebGL.
            if (Settings.InitializeDebugEnabled) Debug.Log($"WebGL End Initialize. {Time.time}");

            if (Settings.InitializeDebugEnabled) Debug.Log($"Cloud Data Start Initialize. {Time.time}");
            yield return YandexData.Initialize(); // Initialize data.
            if (Settings.InitializeDebugEnabled) Debug.Log($"Cloud Data End Initialize. {Time.time}");

            if (Settings.LeaderboardActive)
            {
                if (Settings.InitializeDebugEnabled) Debug.Log($"Leaderboard Start Initialize. {Time.time}");
                yield return Leaderboard.Initialize(); // Initialize Leaderboard.
                if (Settings.InitializeDebugEnabled) Debug.Log($"Leaderboard End Initialize. {Time.time}");
            }

            if (Settings.InitializeDebugEnabled) Debug.Log($"Billing Start Initialize. {Time.time}");
            yield return Billing.Initialize(); // Initialize purchases.
            if (Settings.InitializeDebugEnabled) Debug.Log($"Billing End Initialize. {Time.time}");

            if (Settings.InitializeDebugEnabled) Debug.Log($"Advert Start Initialize. {Time.time}");
            Advert.AdvertInitialize(); // Initialize advert.
            if (Settings.InitializeDebugEnabled) Debug.Log($"Advert End Initialize. {Time.time}");

            OnInitialize?.Invoke();
        }
    }
}