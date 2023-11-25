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

        private IEnumerator Start()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return YandexGamesSdk.Initialize(); // Initialize Agava SDK.
#endif
            WebGL.InitializeListener(); // Initialize WebGL.
            yield return YandexData.Initialize(); // Initialize data.
            yield return Leaderboard.Initialize(); // Initialize Leaderboard.
            yield return Billing.Initialize(); // Initialize purchases.
            Advert.AdvertInitialize(); // Initialize advert.
            OnInitialize?.Invoke();
        }
    }
}