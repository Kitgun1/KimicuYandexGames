using System;
using UnityEngine;

namespace KiYandexSDK
{
    public static class WebGL
    {
        private static event Action<bool> OnBackgroundChanged;

        /// <summary>
        /// Инициализация WebGL
        /// </summary>
        /// <param name="onBackgroundChanged"></param>
        public static void Initialize(Action<bool> onBackgroundChanged = null)
        {
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
            OnBackgroundChanged = onBackgroundChanged;
            WebProperty.AdvertOpenedChange.AddListener(AdvertOpenedChange);
        }

        private static void InBackgroundChange(bool value)
        {
            WebProperty.InGameView = !value;
            OnBackgroundChanged?.Invoke(value);
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