using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KiYandexSDK
{
    public static class WebGL
    {
        private static UnityEvent<bool> _onBackgroundChanged = new();

        /// <summary> Initialize WebGL </summary>
        /// <param name="onBackgroundChanged"> After Background Change </param>
        public static void Initialize(UnityEvent<bool> onBackgroundChanged = null)
        {
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
            if (onBackgroundChanged != null) _onBackgroundChanged = onBackgroundChanged;
            WebProperty.AdvertOpenedChange.AddListener(AdvertOpenedChange);
        }

        private static void InBackgroundChange(bool value)
        {
            WebProperty.InGameView = !value;
            _onBackgroundChanged?.Invoke(value);
            if (!WebProperty.AdvertOpened && WebProperty.InGameView)
            {
                AudioListener.pause = false;
            }
            else
            {
                AudioListener.pause = true;
            }
        }

        private static void AdvertOpenedChange(bool value)
        {
            if (!WebProperty.AdvertOpened && WebProperty.InGameView)
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