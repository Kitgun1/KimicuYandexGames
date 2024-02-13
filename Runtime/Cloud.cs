using System;
using System.Collections;
using System.Collections.Generic;
using Kimicu.YandexGames.Extension;
using UnityEngine;

namespace Kimicu.YandexGames
{
    public static partial class Cloud
    {
        private static string _json = "{}";
        private static Dictionary<string, object> _jsonDictionary = new Dictionary<string, object>();

        private const string SAVE_NAME = "Save";

        public static bool Initialized { get; private set; } = false;

        #region Initialization

        /// <summary> Initializes the cloud save module. </summary>
        public static IEnumerator Initialize(Action onSuccessCallback = null)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            Agava.YandexGames.PlayerAccount.GetCloudSaveData(OnGetCloudSuccessCallback, OnGetCloudErrorCallback);
            #elif UNITY_EDITOR
            OnGetCloudSuccessCallback(PlayerPrefs.GetString(SAVE_NAME, "{}"));
            #endif
            yield return new WaitUntil(() => Initialized);
            onSuccessCallback?.Invoke();
        }

        private static void OnGetCloudSuccessCallback(string json)
        {
            var dictionary = json.JsonToDictionary();
            if (dictionary.TryGetValue(SAVE_NAME, out object hex))
            {
                _json = StringExtension.HexToString((string)hex);
                _jsonDictionary = _json.JsonToDictionary();
            }
            else
            {
                _json = "{}";
                _jsonDictionary = new Dictionary<string, object>();
            }

            Initialized = true;
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        private static void OnGetCloudErrorCallback(string error) => Debug.LogError($"Error get cloud save data:\n{error}");
        #endif

        #endregion

        /// <summary> Get value from the cloud. </summary>
        public static T GetValue<T>(string key, T defaultValue = default)
        {
            if (!Initialized) throw new Exception($"{nameof(Cloud)}. Not Initialized!");
            if (_jsonDictionary.TryGetValue(key, out object value))
            {
                if (value.GetType() == typeof(T)) return (T)value;
                throw new Exception($"Value is not {defaultValue.GetType()}.");
            }

            Debug.Log($"Value not found, return default value.");
            return defaultValue;
        }

        /// <summary> Sets the value to the cloud and local or only local. </summary>
        public static void SetValue(string key, object value, bool saveToCloud = true, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!Initialized) throw new Exception($"{nameof(Cloud)}. Not Initialized!");

            if (_jsonDictionary.TryGetValue(key, out object _)) _jsonDictionary[key] = value;
            else _jsonDictionary.Add(key, value);

            if (saveToCloud) SaveInCloud(onSuccessCallback, onErrorCallback);
        }

        /// <summary> Saves all local data to the cloud. </summary>
        public static void SaveInCloud(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!Initialized) throw new Exception($"{nameof(Cloud)}. Not Initialized!");
            _json = _jsonDictionary.DictionaryToJson();
            string jsonToYandex = $"{{ \"{SAVE_NAME}\": \"{StringExtension.StringToHex(_json)}\"}}";

            if (!JsonExtension.IsValidJson(jsonToYandex))
            {
                Debug.LogError($"Json is not valid.\nJson:\n{StringExtension.HexToString(_json)}\n\nJsonHex:\n{jsonToYandex}");
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
            Agava.YandexGames.PlayerAccount.SetCloudSaveData(jsonToYandex, onSuccessCallback, onErrorCallback);
            #elif UNITY_EDITOR
            PlayerPrefs.SetString(SAVE_NAME, jsonToYandex);
            onSuccessCallback?.Invoke();
            #endif
        }

        /// <summary> Removes all data from the cloud. </summary>
        public static void ClearCloudData(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!Initialized) throw new Exception($"{nameof(Cloud)}. Not Initialized!");
            #if UNITY_WEBGL && !UNITY_EDITOR
            Agava.YandexGames.PlayerAccount.SetCloudSaveData("{}", onSuccessCallback, onErrorCallback);
            #elif UNITY_EDITOR
            PlayerPrefs.SetString(SAVE_NAME, "{}");
            onSuccessCallback?.Invoke();
            #endif
            _json = "{}";
            _jsonDictionary = new Dictionary<string, object>();
        }
    }
}