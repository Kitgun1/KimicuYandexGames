using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_WEBGL && !UNITY_EDITOR
using JetBrains.Annotations;
using UnityEngine;
using Agava.YandexGames;
#endif
using KimicuUtility;
using Newtonsoft.Json.Linq;
using UnityEngine;
using PlayerPrefs = UnityEngine.PlayerPrefs;

namespace Kimicu.YandexGames
{
    public static class YandexData
    {
        public static string Json { get; private set; } = "{}";
        private static bool _initialized = false;

        private static string _postfix;
        private static string _separator;

        private static readonly bool DebugEnabled = KimicuYandexSettings.Instance.YandexDataDebugEnabled;

        /// <summary> Initialize Yandex Data </summary>
        public static IEnumerator Initialize()
        {
            if (_initialized) Debug.LogWarning("YandexData initialization has already been performed.");

            _postfix = KimicuYandexSettings.Instance.Postfix;
            _separator = KimicuYandexSettings.Instance.Separator;
            #if UNITY_WEBGL && !UNITY_EDITOR
            PlayerAccount.GetCloudSaveData((data) =>
            {
                if(DebugEnabled) Debug.Log($"Get cloud save data:\n{data}");
                Json = data;
                _initialized = true;
            }, Debug.LogWarning);
            #else
            if (DebugEnabled) Debug.Log($"Initialize player prefs in Unity Editor.");
            InitializePlayerPrefs();
            #endif
            yield return new WaitUntil(() => _initialized);
            if (DebugEnabled) Debug.Log($"Initialize success.");
        }

        /// <summary> Save data by key and value type </summary>
        /// <param name="saveInCloud"> Whether to save to the cloud. </param>
        /// <param name="onSuccess"> After successful save data in cloud. </param>
        /// <param name="onError"> After unsuccessful data saving in the cloud. </param>
        /// <remarks> If "saveInCloud" is set to false, then the "onSuccess" event will not be triggered. </remarks>
        public static void Save(string key, JToken value, bool saveInCloud = true, Action onSuccess = null,
            Action<string> onError = null)
        {
            if (!Application.isEditor && !_initialized)
            {
                throw new Exception($"{nameof(YandexData)} not initialized!");
            }

            #if UNITY_EDITOR && !UNITY_WEBGL
            if (!_initialized)
            {
                if (DebugEnabled) Debug.Log($"Yandex Data is not initialized. Initialization player prefs..");
                InitializePlayerPrefs();
            }
            #endif

            if (DebugEnabled) Debug.Log($"Json convert to dictionary. Json: {Json}");
            var dictionary = Json.ToDictionary();
            string searchKey = $"{key}{_separator}{value.Type.ToString()[..2]}{_separator}{_postfix}";
            if (DebugEnabled) Debug.Log($"Searching key. Key: {searchKey}");
            if (dictionary.TryGetValue(searchKey, out JToken searchingValue)) dictionary[searchKey] = value;
            else dictionary.Add(searchKey, value);

            if (DebugEnabled) Debug.Log($"Key found. Value: {searchingValue}");
            if (DebugEnabled) DebugLogDictionary(dictionary);

            Json = dictionary.ToJson();
            if (DebugEnabled) Debug.Log($"Json: {Json}");
            #if UNITY_WEBGL && !UNITY_EDITOR
            if(saveInCloud) PlayerAccount.SetCloudSaveData(Json, onSuccess, onError);
            else onSuccess?.Invoke();
            #else
            if (saveInCloud) PlayerPrefs.SetString("json", Json);
            onSuccess?.Invoke();
            #endif
        }

        /// <summary> Returns a value by key "key" and by type "defaultValue". </summary>
        /// <param name="key"> The value by which the search will take place. </param>
        /// <param name="defaultValue">
        /// <para> The standard value, if the right one is not found. </para>
        /// The type by which the search will take place.
        /// </param>
        public static JToken Load(string key, JToken defaultValue)
        {
            if (DebugEnabled) Debug.Log($"Load Value by key. Key: {key}\n\nJson {Json}");
            var data = Json.ToDictionary();
            string searchKey = $"{key}{_separator}{defaultValue.Type.ToString()[..2]}{_separator}{_postfix}";
            return data.TryGetValue(searchKey, out JToken value) ? value : defaultValue;
        }

        public static void DeleteAllData(Action onSuccessDeleted = null, Action<string> onErrorDeleted = null)
        {
            if (DebugEnabled) Debug.Log($"Delete All Data start!");
            Agava.YandexGames.PlayerAccount.SetCloudSaveData("{}", () =>
            {
                if (DebugEnabled) Debug.Log($"Delete All Data success!");
                Json = "{}";
                onSuccessDeleted?.Invoke();
            }, (error) =>
            {
                if (DebugEnabled) Debug.Log($"Delete All Data error!\n Message: {error}");
                onErrorDeleted?.Invoke(error);
            });
        }

        /// <summary> Save current data in cloud. </summary>
        /// <param name="onSuccess"> After successful save data in cloud. </param>
        /// <param name="onError"> After unsuccessful data saving in the cloud. </param>
        public static void SaveToClaud(Action onSuccess = null, Action<string> onError = null)
        {
            if (DebugEnabled) Debug.Log($"Save to cloud..\n\n Json: {Json}");
            #if UNITY_WEBGL && !UNITY_EDITOR
            PlayerAccount.SetCloudSaveData(Json, onSuccess, onError);
            #else
            PlayerPrefs.SetString("json", Json);
            onSuccess?.Invoke();
            #endif
        }

        private static void InitializePlayerPrefs()
        {
            Json = PlayerPrefs.GetString("json", "{}");
            _initialized = true;
        }

        private static void DebugLogDictionary(Dictionary<string, JToken> dictionary)
        {
            var dictionaryArray = dictionary.ToArray();
            string dictionaryString = "Dictionary:\n{";
            for (int i = 0; i < dictionary.Count; i++)
            {
                dictionaryString += $"    {dictionaryArray[i].Key}:{dictionaryArray[i].Value}";
                if (dictionary.Count != i + 1) dictionaryString += ",\n";
                else dictionaryString += "\n";
            }

            dictionaryString += "\n}";
            Debug.Log($"{dictionaryString}");
        }
    }
}