using System;

namespace KimicuYandexGames
{
    public static class Shortcut
    {
        public static void CanSuggest(Action<bool> onResultCallback)
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Shortcut.CanSuggest(onResultCallback);
            #endif
        }

        public static void Suggest(Action<bool> onResultCallback = null)
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Shortcut.Suggest(onResultCallback);
            #endif
        }
    }
}