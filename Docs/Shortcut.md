# 🫧 Shortcut
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
## Methods
```csharp
/// Используйте это для проверки возможности вызова "Suggest". 
Shortcut.CanSuggest(Action<bool> onResultCallback); // void

// ============================================================================= //

/// Используйте это для добавления иконки на рабочий стол. 
Shortcut.Suggest(Action<bool> onResultCallback = null); // void
```
