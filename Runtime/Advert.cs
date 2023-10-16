using System;
using UnityEngine;
using KimicuUtility;
using Kimicu.YandexGames;

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

        public static bool InterAdvertOff { get; private set; }
        public static bool RewardAdvertOff { get; private set; }
        public static bool StickyAdvertOff { get; private set; }
        public static bool AdvertAvailable { get; private set; } = true;

        public static void AdvertInitialize()
        {
            _delayAd = KimicuYandexSettings.Instance.DelayAdvert;
            _interAdvertOffKey = KimicuYandexSettings.Instance.InterAdvertOffKey;
            _rewardAdvertOffKey = KimicuYandexSettings.Instance.RewardAdvertOffKey;
            _stickyAdvertOffKey = KimicuYandexSettings.Instance.StickyAdvertOffKey;

            InterAdvertOff = (bool)YandexData.Load(_interAdvertOffKey, false);
            RewardAdvertOff = (bool)YandexData.Load(_rewardAdvertOffKey, false);
            StickyAdvertOff = (bool)YandexData.Load(_stickyAdvertOffKey, false);

            if (StickyAdvertOff) StickyAdActive(false);
        }

        /// <summary> Remove advert in game. </summary>
        public static void AdvertOff(AdvertType advertType = AdvertType.InterstitialAd)
        {
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
        }

        /// <summary> Show reward ad. </summary>
        /// <param name="onOpen"> After open ad. </param>
        /// <param name="onRewarded"> After viewing the ad. </param>
        /// <param name="onClose"> After close reward. </param>
        /// <param name="onError"> Called when an error occurs. </param>
        public static void RewardAd(Action onOpen = null, Action onRewarded = null, Action onClose = null,
            Action<string> onError = null)
        {
            if (RewardAdvertOff)
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
            if (InterAdvertOff)
            {
                onOpen?.Invoke();
                onClose?.Invoke(true);
                return;
            }

            if (AdvertAvailable == false)
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
            AdvertAvailable = false;
            Routine.StartRoutine(KiCoroutine.Delay(_delayAd, () => AdvertAvailable = true), true);
        }
    }
}