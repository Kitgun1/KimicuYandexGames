using System;
using System.Collections.Generic;
using Utility;

namespace Kimicu.YandexGames
{
    public static class Flags
    {
        public static void GetFlags(Action<Dictionary<string, string>> onSuccessCallback)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            FlagsUtility.GetFlagsCollection(s => onSuccessCallback?.Invoke((Dictionary<string, string>)s));
#else
            onSuccessCallback?.Invoke(new Dictionary<string, string>());
#endif
        }

        public static void GetFlag(string key, string defaultValue = default, Action<string> onSuccessCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            FlagsUtility.GetFlagsCollection(s =>
            {
                var dictionary = (Dictionary<string, string>)s;
                onSuccessCallback?.Invoke(dictionary.GetValueOrDefault(key, defaultValue));
            });
#else
            onSuccessCallback?.Invoke(defaultValue);
#endif   
        }
    }
}