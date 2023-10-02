using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KiYandexSDK
{
    public static class WebGL
    {
        private static UnityEvent<bool> _onBackgroundChanged = new();

        /// <summary> Initialize WebGL. </summary>
        public static void Initialize()
        {
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
            WebProperty.AdvertOpenedChange.AddListener(AdvertOpenedChange);
            WebProperty.PurchaseWindowOpenedChange.AddListener(PurchaseWindowOpenedChange);
        }

        private static void InBackgroundChange(bool value)
        {
            WebProperty.InGameView = !value;
            _onBackgroundChanged?.Invoke(value);
            
            AudioManage();
        }

        private static void AdvertOpenedChange(bool value)
        {
            AudioManage();
        }

        private static void PurchaseWindowOpenedChange(bool value)
        {
            AudioManage();
        }

        private static void AudioManage()
        {
            if (!WebProperty.AdvertOpened && WebProperty.InGameView && !WebProperty.PurchaseWindowOpened)
            {
                AudioListener.pause = false;
            }
            else
            {
                AudioListener.pause = true;
            }
        }
    }
}