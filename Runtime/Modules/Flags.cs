using System;
using System.Collections.Generic;
using System.Linq;
using Agava.YandexGames.Utility;
using Kimicu.YandexGames.Utils;
using UnityEngine;
using Utility;

namespace Kimicu.YandexGames
{
    public static class Flags
    {
        public static Dictionary<string, string> DefaultFlagsInEditor = new Dictionary<string, string>();

#if UNITY_EDITOR
        private const string FILE_NAME = "flags";
#endif

        internal static void InitializeInEditor(Dictionary<string, string> defaultFlags = null)
        {
            DefaultFlagsInEditor = defaultFlags ?? new Dictionary<string, string>()
            {
                { "example_key", "example_value" },
                { "example_key2", "example_value2" },
            };
            GetFlags(response => Debug.Log($"flags: {Json.Serialize(response)}"));
        }

        public static void GetFlags(Action<Dictionary<string, string>> onSuccessCallback)
        {
            if (!YandexGamesSdk.IsInitialized) throw new Exception("YandexGamesSdk not initialized!");
#if !UNITY_EDITOR && UNITY_WEBGL
            FlagsUtility.GetFlagsCollection(s => onSuccessCallback?.Invoke((Dictionary<string, string>)s));
#else
            Debug.Log(Json.Serialize(DefaultFlagsInEditor));
            string json = FileUtility.ReadFile(FILE_NAME, Json.Serialize(DefaultFlagsInEditor), true);
            var flags = (Dictionary<string, string>)Json.Deserialize(json);
            onSuccessCallback?.Invoke(flags);
#endif
        }

        public static void GetFlag(string key, string defaultValue = default, Action<string> onSuccessCallback = null)
        {
            if (!YandexGamesSdk.IsInitialized) throw new Exception("YandexGamesSdk not initialized!");
#if !UNITY_EDITOR && UNITY_WEBGL
            FlagsUtility.GetFlagsCollection(s =>
            {
                var dictionary = (Dictionary<string, string>)s;
                onSuccessCallback?.Invoke(dictionary.GetValueOrDefault(key, defaultValue));
            });
#else
            GetFlags(response =>
            {
                if (response.TryGetValue(key, out string value))
                {
                    onSuccessCallback?.Invoke(value);
                }
                else
                {
                    string json = FileUtility.ReadFile(FILE_NAME, Json.Serialize(DefaultFlagsInEditor), true);
                    var flags = (Dictionary<string, string>)Json.Deserialize(json);
                    flags.Add(key, defaultValue);
                    FileUtility.EditOrCreateFile(FILE_NAME, Json.Serialize(flags));
                    onSuccessCallback?.Invoke(defaultValue);
                }
            });
#endif
        }
    }
}