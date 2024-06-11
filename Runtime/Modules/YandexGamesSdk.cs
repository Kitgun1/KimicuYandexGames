using System;
using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
using UnityEngine;

namespace Kimicu.YandexGames
{
    public static partial class YandexGamesSdk
    {
        public static bool IsRunningOnYandex => Agava.YandexGames.YandexGamesSdk.IsRunningOnYandex;
        public static bool CallbackLogging => Agava.YandexGames.YandexGamesSdk.CallbackLogging;
        public static bool IsInitialized =>
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.IsInitialized;
#else
            _isInitialized;
#endif

        private static bool _isInitialized;

        public static YandexGamesEnvironment Environment =>
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.Environment;
#else
            new()
            {
                app = new YandexGamesEnvironment.App { id = "editor" },
                browser = new YandexGamesEnvironment.Browser { lang = "ru" },
                payload = "editor",
                i18n = new YandexGamesEnvironment.Internationalization { lang = "ru", tld = "https://yandex.ru/games" }
            };
#endif

        public static IEnumerator Initialize(Action onSuccessCallback = null, Dictionary<string, string> defaultFlags = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return Agava.YandexGames.YandexGamesSdk.Initialize(onSuccessCallback);
#else
            _isInitialized = true;
            Flags.InitializeInEditor(defaultFlags);
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