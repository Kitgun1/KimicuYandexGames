using UnityEngine.Events;

namespace KiYandexSDK
{
    internal static class WebProperty
    {
        private static bool _advertOpened = false;

        internal static bool AdvertOpened
        {
            get => _advertOpened;
            set
            {
                _advertOpened = value;
                AdvertOpenedChange?.Invoke(value);
            }
        }

        internal static bool InGameView = true;

        internal static UnityEvent<bool> AdvertOpenedChange;
    }
}