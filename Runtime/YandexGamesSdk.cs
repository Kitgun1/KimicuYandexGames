using System;
using System.Collections;
using Agava.YandexGames;
using UnityEngine;

namespace Kimicu.YandexGames
{
    public static partial class YandexGamesSdk
    {
        public static bool IsRunningOnYandex => Agava.YandexGames.YandexGamesSdk.IsRunningOnYandex;
        public static bool CallbackLogging => Agava.YandexGames.YandexGamesSdk.CallbackLogging;
        public static bool IsInitialized => Agava.YandexGames.YandexGamesSdk.IsInitialized;
        public static YandexGamesEnvironment Environment => Agava.YandexGames.YandexGamesSdk.Environment;

        public static IEnumerator Initialize(Action onSuccessCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return Agava.YandexGames.YandexGamesSdk.Initialize(onSuccessCallback);
#else
            onSuccessCallback?.Invoke();
            yield break;
#endif
        }

        public static void GameReady()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.GameReady();
            #else
            Debug.Log($"GameReady invoke!");
            #endif
        }
    }
}