using System;

namespace KimicuYandexGames
{
    public static class WebProperties
    {
        private static bool _inBackground;
        private static bool _inAdvert;
        private static bool _inPurchaseWindow;
        private static bool _customValue;

        public static bool InBackground
        {
            get => _inBackground;
            internal set
            {
                _inBackground = value;
                TryStopGame();
            }
        }

        public static bool InAdvert
        {
            get => _inAdvert;
            internal set
            {
                _inAdvert = value;
                TryStopGame();
            }
        }

        public static bool InPurchaseWindow
        {
            get => _inPurchaseWindow;
            internal set
            {
                _inPurchaseWindow = value;
                TryStopGame();
            }
        }

        public static bool CustomValue
        {
            get => _customValue;
            internal set
            {
                _customValue = value;
                TryStopGame();
            }
        }

        private static event Action OnStopGame;

        public static void Initialize(Action onStopGame)
        {
            OnStopGame = onStopGame;
            InBackground = Agava.WebUtility.WebApplication.InBackground;
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
        }

        private static void InBackgroundChange(bool inBackground) => InBackground = inBackground;

        private static void TryStopGame()
        {
            if (InBackground || InAdvert || InPurchaseWindow || CustomValue) OnStopGame?.Invoke();
        }
    }
}