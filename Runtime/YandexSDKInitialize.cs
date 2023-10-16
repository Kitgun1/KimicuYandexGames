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
        [Tooltip("OnInitialize invoke in Editor."), SerializeField]
        private float m_InitializeDelay = 0.2f;

        public UnityEvent OnInitialize = new();

        private IEnumerator Start()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return YandexGamesSdk.Initialize(); // Initialize Agava SDK.
            yield return YandexData.Initialize(); // Initialize data.
            yield return Billing.Initialize(); // Initialize purchases.
            Advert.AdvertInitialize();  // Initialize advert.
            WebGL.InitializeListener();  // Initialize WebGL.
            OnInitialize?.Invoke();
#else
            yield return new WaitForSecondsRealtime(m_InitializeDelay);
            OnInitialize?.Invoke();
#endif
        }
    }
}