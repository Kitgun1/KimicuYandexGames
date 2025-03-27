using System;
using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
using Kimicu.YandexGames.Extension;
using Kimicu.YandexGames.Utils;
using UnityEngine;

namespace Kimicu.YandexGames
{
    public static partial class YandexGamesSdk
    {
        /// <summary> Use it to check whether you're using Build and Run or running in the Editor. Can be called without initializing the SDK, can be called in Editor. </summary>
        public static bool IsRunningOnYandex => Agava.YandexGames.YandexGamesSdk.IsRunningOnYandex;
        /// <summary> Enable it to log SDK callbacks in the console. </summary>
        public static bool CallbackLogging => Agava.YandexGames.YandexGamesSdk.CallbackLogging;
        
        /// <summary> SDK is initialized automatically on load. If either something fails or called way too early, this will return false. </summary>
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
            FileExtensions.LoadObject("environment", new YandexGamesEnvironment());
#endif
        
        public static string Language =>
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.Language;
#else
            Environment.i18n.lang;
#endif

        public static IEnumerator Initialize(Action onSuccessCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return Agava.YandexGames.YandexGamesSdk.Initialize(onSuccessCallback);
#else
            _isInitialized = true;
            onSuccessCallback?.Invoke();
            yield break;
#endif
        }

        public static void GameReady()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.GameReady();
            #else
            Debug.Log($"{nameof(GameReady)} invoke!");
            #endif
        }

        public static void GameStart()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.GameStart();
            #else
            Debug.Log($"{nameof(GameStart)} invoke!");
            #endif
        }

        public static void GameStop()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.YandexGamesSdk.GameStop();
            #else
            Debug.Log($"{nameof(GameStop)} invoke!");
            #endif
        }
    }
}