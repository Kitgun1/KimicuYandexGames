# ⛄ Account
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
## Fields
```csharp
/// Используйте это перед вызовом методов SDK, требующих авторизации.
Account.IsAuthorized; // bool

// ============================================================================= //

/// Разрешение на использование имени и изображения профиля из аккаунта Яндекс..
Account.HasPersonalProfileDataPermission; // bool
```

## Methods
```csharp
/// Запросите разрешение на получение имени учетной записи Яндекса и изображения профиля.
/// ВАЖНО: Имейте в виду, что если пользователь отклоняет запрос – это навсегда. 
/// Окно запроса больше никогда не откроется.
/// ПРИМЕЧАНИЕ: Требуется авторизация. Используйте "IsAuthorized" and "Authorize".
Account.RequestPersonalProfileDataPermission(Action onSuccessCallback = null, Action<string> onErrorCallback = null); // void

// ============================================================================= //

/// Вызывает окно авторизации пользователя.
Account.Authorize(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
    
// ============================================================================= //

Account.StartAuthorizationPolling(int delay, Action successCallback = null, Action errorCallback = null)
```