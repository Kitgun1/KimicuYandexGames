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
        
        private static bool _purchaseWindowOpened = false;

        internal static bool PurchaseWindowOpened
        {
            get => _purchaseWindowOpened;
            set
            {
                _purchaseWindowOpened = value;
                PurchaseWindowOpenedChange?.Invoke(value);
            }
        }

        internal static bool InGameView = true;

        internal static readonly UnityEvent<bool> AdvertOpenedChange = new();
        internal static readonly UnityEvent<bool> PurchaseWindowOpenedChange = new();
    }
}