# 📢 Advertisement
## Инициализация
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
```csharp
// ...
Initialize();
// ...
```

## Fields
```csharp
/// Используйте его, можно ли вызвать межстраничную рекламу.
Advertisement.AdvertisementIsAvailable; // bool

// ============================================================================= //

/// Используйте его, чтобы проверить, инициализирован ли Advertisement.
Advertisement.IsInitialized; // bool
```

## Methods
[Подробнее про параметры можно посмотреть тут](https://yandex.ru/dev/games/doc/ru/sdk/sdk-adv)
```csharp
/// Вызывает межстраничную рекламу.
/// ПРИМЕЧАНИЕ: Если Advertisement.AdvertisementIsAvailable = false, при вызове метода будет всегда onErrorCallback.
Advertisement.ShowInterstitialAd(Action onOpenCallback = null, Action onCloseCallback = null, 
    Action<string> onErrorCallback = null, Action onOfflineCallback = null); // void

// ============================================================================= //

/// Вызывает рекламу с вознаграждением
Advertisement.ShowVideoAd(Action onOpenCallback = null, Action onRewardedCallback = null, 
    Action onCloseCallback = null, Action<string> onErrorCallback = null); // void

// Пример:
Advertisement.ShowVideoAd(onRewardedCallback: () => Debug.Log("Выдаем награду игроку."));

// ============================================================================= //

/// Используйте для включения или выключения Sticky-баннера
Advertisement.StickySetActive(bool value); // void
```