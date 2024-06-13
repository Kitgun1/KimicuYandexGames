# 🌐 WebApplication
## Инициализация
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
```csharp
// ...
Initialize(Action<bool> onStopGame = null);
// ...

// Пример:
Initialize(isStopGame => {
    AudioListener.pause = isStopGame;
    AudioListener.volume = isStopGame ? 0 : 1;
    Time.timeScale = isStopGame ? 0 : 1;
});
```
> onStopGame - вызывается при изменении состоянии игры

## Fields
```csharp
/// [READONLY PROPERTY]
/// Состояние вкладки. true - в игре | false - игра свернута
WebApplication.InBackground; // bool 

// ============================================================================= //

/// [READONLY PROPERTY]
/// Состояние рекламы. true - идет реклама | false - рекламы нет
WebApplication.InAdvert; // bool 

// ============================================================================= //

/// [READONLY PROPERTY]
/// Состояние покупки. true - в меню покупки | false - меню покупок закрыто
WebApplication.InPurchaseWindow; // bool 

// ============================================================================= //

/// Кастомное значение. При true игра будет останавливаться
WebApplication.CustomValue; // bool 
```

## Actions
```csharp
WebApplication.InBackgroundChangeState; // Action<bool>
WebApplication.InAdvertChangeState; // Action<bool>
WebApplication.InPurchaseWindowChangeState; // Action<bool>
WebApplication.OnCustomValueChangeState; // Action<bool>
WebApplication.OnStopGame; // Action<bool>
```