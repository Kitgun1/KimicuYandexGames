using UnityEngine.Events;

namespace Kimicu.YandexGames
{
    internal static class WebProperty
    {
        private static bool _advertOpened = false;

        public static bool AdvertOpened
        {
            get => _advertOpened;
            internal set
            {
                _advertOpened = value;
                AdvertOpenedChange?.Invoke(value);
            }
        }
        
        private static bool _purchaseWindowOpened = false;

        public static bool PurchaseWindowOpened
        {
            get => _purchaseWindowOpened;
            internal set
            {
                _purchaseWindowOpened = value;
                PurchaseWindowOpenedChange?.Invoke(value);
            }
        }

        public static bool InGameView { get; internal set; } = true;

        internal static readonly UnityEvent<bool> AdvertOpenedChange = new();
        internal static readonly UnityEvent<bool> PurchaseWindowOpenedChange = new();
    }
}