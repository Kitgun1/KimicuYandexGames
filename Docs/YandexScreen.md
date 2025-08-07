# ⚙️ YandexScreen
## Инициализация
```csharp
/// Инициализация. Метод обязан быть вызван перед использованием
public static IEnumerator Initialize(Action onSuccessCallback = null, Action<string> onErrorCallback = null);
```
- onSuccessCallback - возвращается по завершении инициализации
- onErrorCallback - возвращается при ошибке инициализации с описанием ошибки 

## Properties
```csharp
/// Возвращает true, если приложение запущено в полноэкранном режиме
public static bool IsFullscreen { get; private set; }
```

## Methods
```csharp
/// Вызывает запрос полноэкранного режима со стороны Yandex Games
/// ПРИМЕЧАНИЕ: Если полнокранный режим уже включен, при вызове метода, запрос не будет отправлен. onSuccess будет вызван сразу
/// ВАЖНО: Данный метод ввызывает запрос на полноэкранный режим. Также полноэкранный режим может включится с небольшой задержкой, поэтому не стоит что-то делать сразу по коллбеку onSuccess 
public static void RequestFullscreenMode(Action onSuccess = null, Action<string> onError = null)();

/// Вызывает запрос выхода из полноэкранного режима со стороны Yandex Games
/// ПРИМЕЧАНИЕ: Если полнокранный режим уже выключен, при вызове метода, запрос не будет отправлен. onSuccess будет вызван сразу
/// ВАЖНО: Данный метод ввызывает запрос на выход из полноэкранного режима. Также полноэкранный режим может выключится с небольшой задержкой, поэтому не стоит что-то делать сразу по коллбеку onSuccess
public static void ExitFullscreenMode(Action onSuccess = null, Action<string> onError = null);
```

## Events
```csharp
/// Возвращает актуальный статус полноэкранного режима. Вызывается каждый раз при изменении
public static event Action<bool> FullscreenStatusChanged;

```