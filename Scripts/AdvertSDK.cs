using System;
using UnityEngine;
using KimicuUtility;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using Agava.YandexGames;
#endif

namespace KiYandexSDK
{
    public static class AdvertSDK
    {
        private static bool _advertOff;
        private static bool _advertAvailable = true;
        
        private static readonly KiCoroutine Routine = new();

        /// <summary> Delay between ad calls. </summary>
        public static float DelayAd = 30.1f;

        /// <summary> The key by which the shutdown of advertising is saved. </summary>
        public static string AdvertOffKey = "ADVERT_OFF";

        /// <summary> Initialize advert for yandex games. </summary>
        public static void AdvertInitialize() => _advertOff = (bool)YandexData.Load(AdvertOffKey, false);

        /// <summary> Remove all advert (Except StickyAd) in game. </summary>
        public static void AdvertOff()
        {
            _advertOff = true;
            YandexData.Save(AdvertOffKey, _advertOff);
        }

        /// <summary> Show reward ad. </summary>
        /// <param name="onOpen"> After open ad. </param>
        /// <param name="onRewarded"> After viewing the ad. </param>
        /// <param name="onClose"> After close reward. </param>
        public static void RewardAd(Action onOpen = null, Action onRewarded = null, Action onClose = null,
            Action<string> onError = null)
        {
            if (_advertOff)
            {
                onOpen?.Invoke();
                onRewarded?.Invoke();
                onClose?.Invoke();
                WebProperty.AdvertOpened = false;
                return;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            VideoAd.Show(()=>
            {
                onOpen?.Invoke();
                WebProperty.AdvertOpened = true;
            }, ()=>
            {
                onRewarded?.Invoke();
            }, ()=>
            {
                onClose?.Invoke();
                WebProperty.AdvertOpened = false;
            },error =>
            {
                onError?.Invoke(error);
                WebProperty.AdvertOpened = false;
            });
#else
            onOpen?.Invoke();
            onRewarded?.Invoke();
            onClose?.Invoke();
            WebProperty.AdvertOpened = false;
#endif
        }

        /// <summary> Active/Inactive Sticky advert. </summary>
        public static void StickyAdActive(bool value)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (value) StickyAd.Show();
            else StickyAd.Hide();
#else
            Debug.Log($"StickyAd is {value}");
#endif
        }

        /// <summary> Show Interstitial ad. </summary>
        /// <param name="onOpen"> After open ad. </param>
        /// <param name="onClose"> After close reward. </param>
        public static void InterstitialAd(Action onOpen = null, Action<bool> onClose = null,
            Action<string> onError = null, Action onOffline = null)
        {
            if (_advertAvailable == false)
            {
                onError?.Invoke("Advert not available!");
                return;
            }

            if (_advertOff)
            {
                onOpen?.Invoke();
                onClose?.Invoke(true);
            }
            else
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                Agava.YandexGames.InterstitialAd.Show(() =>
                {
                    onOpen?.Invoke();
                    WebProperty.AdvertOpened = true;
                }, closeCallback =>
                {
                    onClose?.Invoke(closeCallback);
                    WebProperty.AdvertOpened = false;
                }, (error) =>
                {
                    onError?.Invoke(error);
                    WebProperty.AdvertOpened = false;
                }, () =>
                {
                    onOffline?.Invoke();
                    WebProperty.AdvertOpened = false;
                });
#else
                onClose?.Invoke(true);
                Debug.Log($"InterstitialAd Show");
#endif
            }

            _advertAvailable = false;
            Routine.StartRoutine(KiCoroutine.Delay(DelayAd, () => _advertAvailable = true), true);
        }
    }
}