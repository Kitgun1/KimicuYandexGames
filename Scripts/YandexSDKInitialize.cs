using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_WEBGL && !UNITY_EDITOR
using Agava.YandexGames;
#endif

namespace KiYandexSDK
{
    public sealed class YandexSDKInitialize : MonoBehaviour
    {
        [Tooltip("Initialize invoke in Editor."), SerializeField]
        private float _initializeDelay = 0.2f;

        public UnityEvent OnInitialize = new();

        private IEnumerator Start()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return YandexGamesSdk.Initialize(); // Initialize Agava SDK.
            yield return YandexData.Initialize(); // Initialize data.
            yield return Billing.Initialize(); // Initialize purchases.
            AdvertSDK.AdvertInitialize();  // Initialize advert.
            WebGL.Initialize();  // Initialize WebGL.
            OnInitialize?.Invoke();
#else
            yield return new WaitForSecondsRealtime(_initializeDelay);
            OnInitialize?.Invoke();
#endif
        }
    }
}