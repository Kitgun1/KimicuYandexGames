using System;
using System.Collections;
using UnityEngine;
using Coroutine = Kimicu.YandexGames.Utils.Coroutine;

namespace Kimicu.YandexGames
{
    using Coroutine = Utils.Coroutine;

    public static partial class Advertisement
    {
        public static bool AdvertisementIsAvailable { get; private set; } = true;

        private static bool _initialized = false;

        private static readonly Coroutine ReloadCoroutine = new Coroutine();

        private const float INTERSTITIAL_AD_COOLDOWN = 70;

        public static void Initialize()
        {
            ReloadCoroutine.StartRoutine(AdvertisementReloadRoutine());
            _initialized = true;
        }

        public static void ShowInterstitialAd(Action onOpenCallback = null, Action onCloseCallback = null, Action<string> onErrorCallback = null, Action onOfflineCallback = null)
        {
            if (!_initialized) throw new Exception($"{nameof(Advertisement)} not initialized!");
            
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.InterstitialAd.Show(onOpenCallback, (wasShown) => onCloseCallback?.Invoke(), onErrorCallback, onOfflineCallback);
            #elif UNITY_EDITOR
            onOpenCallback?.Invoke();
            onCloseCallback?.Invoke();
            #endif
        }

        public static void ShowVideoAd(Action onOpenCallback = null, Action onRewardedCallback = null, Action onCloseCallback = null, Action<string> onErrorCallback = null)
        {
            if (!_initialized) throw new Exception($"{nameof(Advertisement)} not initialized!");
            
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.VideoAd.Show(onOpenCallback, onRewardedCallback, onCloseCallback, onErrorCallback);
            #elif UNITY_EDITOR
            onOpenCallback?.Invoke();
            onRewardedCallback?.Invoke();
            onCloseCallback?.Invoke();
            #endif
        }

        public static void StickySetActive(bool value)
        {
            if (!_initialized) throw new Exception($"{nameof(Advertisement)} not initialized!");
            
            #if !UNITY_EDITOR && UNITY_WEBGL
            if (value) Agava.YandexGames.StickyAd.Show();
            else Agava.YandexGames.StickyAd.Hide();
            #elif UNITY_EDITOR
            #endif
        }

        private static IEnumerator AdvertisementReloadRoutine()
        {
            while (true)
            {
                AdvertisementIsAvailable = true;
                yield return new WaitForSecondsRealtime(INTERSTITIAL_AD_COOLDOWN);
            }
        }
    }
}