# ☁️ Cloud
## Инициализация
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
```csharp
// ...
yield return Initialize(onSuccessCallback = null);
// ...
```
> onSuccessCallback - вызывается по завершению инициализации

## Fields
```csharp
/// Используйте его, чтобы проверить, инициализирован ли Cloud в SDK.
Cloud.Initialized; // bool
```

## Methods
[Подробнее про параметры flush и другие можно посмотреть тут](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player#ingame-data)
```csharp
/// Получите значение из облака.
Cloud.GetValue<T>(string key, T defaultValue); // T

// Примеры:
int money = Cloud.GetValue<int>("money");
var data = Cloud.GetValue("data", new Dictionary<string, (int, string))>());

// ============================================================================= //

/// Устанавливает значение в облако и локально или только локально.
Cloud.SetValue(string key, object value, bool saveToCloud = false, 
    Action onSuccessCallback = null, Action<string> onErrorCallback = null); // void

// Пример:
Cloud.SetValue("data", new Dictionary<string, (int, string))>());
Cloud.SetValue("money", 2500, true, () => Debug.Log("Все локаальные изменения были отправлены в облако."));

// ============================================================================= //

/// Проверяет, есть ли такое значение в сохранениях.
Cloud.HasKey(string key); // bool

// Пример:
var hasData = Cloud.HasKey("data");

// ============================================================================= //

/// Проверяет, есть ли такое значение в сохранениях.
Cloud.SaveInCloud(Action onSuccessCallback = null, Action<string> onErrorCallback = null, 
    bool flush = false); // void

// Пример:
Cloud.SetValue("money", 2500);
Cloud.SaveInCloud();

// ============================================================================= //

/// Удаляет все данные из облака.
Cloud.ClearCloudData(Action onSuccessCallback = null, Action<string> onErrorCallback = null); // void
```