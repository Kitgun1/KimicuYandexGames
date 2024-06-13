# ⚙️ YandexGamesSdk
## Инициализация
```csharp
// ...
yield return Initialize(onSuccessCallback = null);
// ...
```
> onSuccessCallback - вызывается по завершению инициализации

## Fields
```csharp
/// Используйте его, чтобы проверить, используете ли вы сборку и запуск или работаете в редакторе.
/// Можно вызвать без инициализации SDK, можно вызвать в редакторе.
YandexGamesSdk.IsRunningOnYandex; // bool

// ============================================================================= //

/// Включите его для регистрации обратных вызовов SDK в консоли.
YandexGamesSdk.CallbackLogging; // bool

// ============================================================================= //

/// Используйте его, чтобы проверить, инициализирован ли SDK.
YandexGamesSdk.IsInitialized; // bool

// ============================================================================= //

/// Данные (id app, lang, и тд)
YandexGamesSdk.Environment; // YandexGamesEnvironment
```

## Methods
```csharp
/// Вызывает GameReady в начале игры. 
/// ВАЖНО: Вызывайте этот метод в момент готовности игры, когда игрок может куда-то кликать.
YandexGamesSdk.GameReady(); // void
```

## Classes
```csharp
class YandexGamesEnvironment {
    App app; { id }
    Browser browser; { lang }
    Internationalization i18n; { lang; tld }
    string payload;
}
```