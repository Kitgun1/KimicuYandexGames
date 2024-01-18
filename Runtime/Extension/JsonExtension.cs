using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KimicuYandexGames.Extension
{
    public static class JsonExtension
    {
        internal static Dictionary<string, object> JsonToDictionary(this string json) => JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        internal static string DictionaryToJson(this Dictionary<string, object> dictionary) => JsonConvert.SerializeObject(dictionary);

        public static bool IsValidJson(this string jsonString)
        {
            try
            {
                JsonUtility.FromJson<object>(jsonString);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Error: {exception.Message}");
                return false;
            }
        }
    }
}