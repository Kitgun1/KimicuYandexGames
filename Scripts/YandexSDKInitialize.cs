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
            yield return YandexGamesSdk.Initialize(); // Инициализация Agava SDK
            yield return YandexData.Initialize(); // Инициализация сохранений
            yield return Billing.Initialize(); // Инициализация покупок
            AdvertSDK.AdvertInitialize();  // Инициализация рекламы
            WebGL.Initialize();  // Инициализация WebGL
            OnInitialize?.Invoke();
#else
            yield return new WaitForSecondsRealtime(_initializeDelay);
            OnInitialize?.Invoke();
#endif
        }
    }
}