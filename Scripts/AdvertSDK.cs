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
        private static bool _interAdvertOff;
        private static bool _rewardAdvertOff;
        private static bool _stickyAdvertOff;
        private static bool _advertAvailable = true;

        private static readonly KiCoroutine Routine = new();

        /// <summary> Delay between ad calls. </summary>
        public static float DelayAd = 30.1f;

        /// <summary> The key by which the shutdown of inter advertising is saved. </summary>
        public static string InterAdvertOffKey = "INTER_ADVERT_OFF";

        /// <summary> The key by which the shutdown of reward advertising is saved. </summary>
        public static string RewardAdvertOffKey = "REWARD_ADVERT_OFF";

        /// <summary> The key by which the shutdown of sticky advertising is saved. </summary>
        public static string StickyAdvertOffKey = "STICKY_ADVERT_OFF";

        /// <summary> Initialize advert for yandex games. </summary>
        public static void AdvertInitialize()
        {
            _interAdvertOff = (bool)YandexData.Load(InterAdvertOffKey, false);
            _rewardAdvertOff = (bool)YandexData.Load(RewardAdvertOffKey, false);
            _stickyAdvertOff = (bool)YandexData.Load(StickyAdvertOffKey, false);

            if (_stickyAdvertOff) StickyAdActive(false);
        }

        /// <summary> Remove InterstitialAd advert in game. </summary>
        public static void AdvertOff(AdvertType advertType = AdvertType.InterstitialAd)
        {
            switch (advertType)
            {
                case AdvertType.InterstitialAd:
                    _interAdvertOff = true;
                    YandexData.Save(InterAdvertOffKey, _interAdvertOff);
                    break;
                case AdvertType.RewardAd:
                    _rewardAdvertOff = true;
                    YandexData.Save(RewardAdvertOffKey, _rewardAdvertOff);
                    break;
                case AdvertType.StickyAd:
                    _stickyAdvertOff = true;
                    YandexData.Save(StickyAdvertOffKey, _stickyAdvertOff);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(advertType), advertType, null);
            }
        }

        /// <summary> Show reward ad. </summary>
        /// <param name="onOpen"> After open ad. </param>
        /// <param name="onRewarded"> After viewing the ad. </param>
        /// <param name="onClose"> After close reward. </param>
        /// <param name="onError"> Called when an error occurs. </param>
        public static void RewardAd(Action onOpen = null, Action onRewarded = null, Action onClose = null,
            Action<string> onError = null)
        {
            if (_rewardAdvertOff)
            {
                onOpen?.Invoke();
                onRewarded?.Invoke();
                onClose?.Invoke();
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
            if (_stickyAdvertOff)
            {
                StickyAd.Hide();
            }
            else
            {
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
            if (_interAdvertOff)
            {
                onOpen?.Invoke();
                onClose?.Invoke(true);
                return;
            }

            if (_advertAvailable == false)
            {
                onError?.Invoke("Advert not available!");
                return;
            }
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
            _advertAvailable = false;
            Routine.StartRoutine(KiCoroutine.Delay(DelayAd, () => _advertAvailable = true), true);
        }
    }
}