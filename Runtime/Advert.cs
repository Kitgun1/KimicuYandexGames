using System;
using System.Collections;
using UnityEngine;
using KimicuUtility;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using Agava.YandexGames;
#endif

namespace Kimicu.YandexGames
{
    public static class Advert
    {
        private static float _delayAd;
        private static string _interAdvertOffKey;
        private static string _rewardAdvertOffKey;
        private static string _stickyAdvertOffKey;

        private static readonly KiCoroutine Routine = new();
        private static IEnumerator _enumerator = null;

        public static bool InterAdvertOff { get; private set; }
        public static bool RewardAdvertOff { get; private set; }
        public static bool StickyAdvertOff { get; private set; }
        public static bool AdvertAvailable { get; private set; } = true;

        private static readonly bool DebugEnabled = KimicuYandexSettings.Instance.AdvertDebugEnabled;

        public static void AdvertInitialize()
        {
            if (DebugEnabled) Debug.Log($"Advert Initialize Starts");
            _delayAd = KimicuYandexSettings.Instance.DelayAdvert;
            _interAdvertOffKey = KimicuYandexSettings.Instance.InterAdvertOffKey;
            _rewardAdvertOffKey = KimicuYandexSettings.Instance.RewardAdvertOffKey;
            _stickyAdvertOffKey = KimicuYandexSettings.Instance.StickyAdvertOffKey;

            InterAdvertOff = (bool)YandexData.Load(_interAdvertOffKey, false);
            RewardAdvertOff = (bool)YandexData.Load(_rewardAdvertOffKey, false);
            StickyAdvertOff = (bool)YandexData.Load(_stickyAdvertOffKey, false);

            if (DebugEnabled)
                Debug.Log($"Advert Initialized:\n" +
                          $"   Inter Advert Off: {InterAdvertOff}" +
                          $"   Reward Advert Off: {RewardAdvertOff}" +
                          $"   Sticky Advert Off: {StickyAdvertOff}");

            if (StickyAdvertOff) StickyAdActive(false);
        }

        /// <summary> Remove advert in game. </summary>
        public static void AdvertOff(AdvertType advertType = AdvertType.InterstitialAd)
        {
            if (DebugEnabled) Debug.Log($"Advert Off: {advertType}");
            switch (advertType)
            {
                case AdvertType.InterstitialAd:
                    InterAdvertOff = true;
                    YandexData.Save(_interAdvertOffKey, InterAdvertOff);
                    break;
                case AdvertType.RewardAd:
                    RewardAdvertOff = true;
                    YandexData.Save(_rewardAdvertOffKey, RewardAdvertOff);
                    break;
                case AdvertType.StickyAd:
                    StickyAdvertOff = true;
                    YandexData.Save(_stickyAdvertOffKey, StickyAdvertOff);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(advertType), advertType, null);
            }

            if (DebugEnabled) Debug.Log($"Advert Off success");
        }

        /// <summary> Show reward ad. </summary>
        /// <param name="onOpen"> After open ad. </param>
        /// <param name="onRewarded"> After viewing the ad. </param>
        /// <param name="onClose"> After close reward. </param>
        /// <param name="onError"> Called when an error occurs. </param>
        public static void RewardAd(Action onOpen = null, Action onRewarded = null, Action onClose = null,
            Action<string> onError = null)
        {
            if (DebugEnabled) Debug.Log($"RewardAd Show");
            if (RewardAdvertOff)
            {
                if (DebugEnabled) Debug.Log($"Reward Advert off. Call: [onOpen, onRewarded, onClose]");
                onOpen?.Invoke();
                onRewarded?.Invoke();
                onClose?.Invoke();
            }
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (DebugEnabled) Debug.Log($"Reward Advert in Yandex Games.");
            VideoAd.Show(()=>
            {
                if (DebugEnabled) Debug.Log($"[Reward Advert]. Call: [onOpen]");
                onOpen?.Invoke();
                WebProperty.AdvertOpened = true;
            }, ()=>
            {
                if (DebugEnabled) Debug.Log($"[Reward Advert]. Call: [onRewarded]");
                onRewarded?.Invoke();
            }, ()=>
            {
                if (DebugEnabled) Debug.Log($"[Reward Advert]. Call: [onClose]");
                onClose?.Invoke();
                WebProperty.AdvertOpened = false;
            },error =>
            {
                if (DebugEnabled) Debug.Log($"[Reward Advert]. Call: [onError]\nError message: {error}");
                onError?.Invoke(error);
                WebProperty.AdvertOpened = false;
            });
            #else
            if (DebugEnabled) Debug.Log($"Reward Advert in Editor. Call: [onOpen, onRewarded, onClose]");
            onOpen?.Invoke();
            onRewarded?.Invoke();
            onClose?.Invoke();
            WebProperty.AdvertOpened = false;
            #endif
        }

        /// <summary> Active/Inactive Sticky advert. </summary>
        public static void StickyAdActive(bool value)
        {
            if (DebugEnabled) Debug.Log($"StickyAd Active");
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (StickyAdvertOff)
            {
                if (DebugEnabled) Debug.Log($"Sticky Advert off.");
                StickyAd.Hide();
            }
            else
            {
                if (DebugEnabled && value) Debug.Log($"Sticky Advert Activate");
                else if(DebugEnabled && !value) Debug.Log($"Sticky Advert Deactivate");
                if (value) StickyAd.Show();
                else StickyAd.Hide();
            }
            #endif
        }

        /// <summary> Show Interstitial ad. </summary>
        /// <param name="onOpen"> After open ad. </param>
        /// <param name="onClose"> After close reward. </param>
        /// <param name="onError"> Called when an error occurs. </param>
        /// <param name="onOffline"> Called when the network connection is lost (switching to offline mode). </param>
        public static void InterstitialAd(Action onOpen = null, Action<bool> onClose = null,
            Action<string> onError = null, Action onOffline = null)
        {
            if (DebugEnabled) Debug.Log($"Inter Show");
            if (InterAdvertOff)
            {
                if (DebugEnabled) Debug.Log($"Inter Advert off. Call: [onOpen, onClose]");
                onOpen?.Invoke();
                onClose?.Invoke(true);
                return;
            }

            if (AdvertAvailable == false)
            {
                if (DebugEnabled) Debug.Log($"Inter Advert not available. Call: [onError]");
                onError?.Invoke("Advert not available!");
                return;
            }
            #if UNITY_WEBGL && !UNITY_EDITOR
            Agava.YandexGames.InterstitialAd.Show(() =>
            {
                if (DebugEnabled) Debug.Log($"[Inter Advert]. Call: [onOpen]");
                onOpen?.Invoke();
                WebProperty.AdvertOpened = true;
            }, closeCallback =>
            {
                if (DebugEnabled) Debug.Log($"[Inter Advert]. Call: [onClose]");
                onClose?.Invoke(closeCallback);
                WebProperty.AdvertOpened = false;
            }, (error) =>
            {
                if (DebugEnabled) Debug.Log($"[Inter Advert]. Call: [onError]\nError message: {error}");
                onError?.Invoke(error);
                WebProperty.AdvertOpened = false;
            }, () =>
            {
                if (DebugEnabled) Debug.Log($"[Inter Advert]. Call: [onOffline]");
                onOffline?.Invoke();
                WebProperty.AdvertOpened = false;
            });
            #else
            if (DebugEnabled) Debug.Log($"Inter Advert in Editor. Call: [onOpen, onClose]");
            onOpen?.Invoke();
            onClose?.Invoke(true);
            #endif
            AdvertAvailable = false;
            if (_enumerator != null) return;
            _enumerator = ReloadInterAdvert();
            Routine.StartRoutine(_enumerator);
        }

        private static IEnumerator ReloadInterAdvert()
        {
            if (DebugEnabled) Debug.Log($"Inter Advert. Start Reload Advert");
            yield return new WaitForSecondsRealtime(_delayAd);
            if (DebugEnabled) Debug.Log($"Inter Advert. End Reload Advert");
            AdvertAvailable = true;
            _enumerator = null;
        }
    }
}