# 🚩 Flags
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
## Methods
```csharp
/// Используйте для получения всех флагов. 
/// ПРИМЕЧАНИЕ: Для Editor можно также настроить в папке "ProjectFolder"/EditorCloud/flags.txt
Flags.GetFlags(Action<Dictionary<string, string>> onSuccessCallback); // void

// ============================================================================= //

/// Используйте для получения флага по ключу. 
/// ПРИМЕЧАНИЕ: Для Editor можно также настроить в папке "ProjectFolder"/EditorCloud/flags.txt
Flags.GetFlag(string key, string defaultValue = default, Action<string> onSuccessCallback = null); // void
```