using System;
using System.Collections;
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
        private static string _json = "{}";
        private static bool _initialized = false;

        private static string _postfix;
        private static string _separator;

        /// <summary> Initialize Yandex Data </summary>
        public static IEnumerator Initialize()
        {
            if (_initialized)
            {
                Debug.LogWarning("YandexData initialization has already been performed.");
            }

            _postfix = KimicuYandexSettings.Instance.Postfix;
            _separator = KimicuYandexSettings.Instance.Separator;
#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerAccount.GetCloudSaveData((data) =>
            {
                _json = data;
                _initialized = true;
            }, Debug.LogWarning);
#else
            InitializePlayerPrefs();
#endif
            yield return new WaitUntil(() => _initialized);
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
            if (!_initialized) InitializePlayerPrefs();
#endif

            var dictionary = _json.ToDictionary();
            string searchKey = $"{key}{_separator}{value.Type.ToString()[..2]}{_separator}{_postfix}";
            if (dictionary.TryGetValue(searchKey, out JToken _)) dictionary[searchKey] = value;
            else dictionary.Add(searchKey, value);

            _json = dictionary.ToJson();
#if UNITY_WEBGL && !UNITY_EDITOR
            if(saveInCloud) PlayerAccount.SetCloudSaveData(_json, onSuccess, onError);
            else onSuccess?.Invoke();
#else
            if (saveInCloud) PlayerPrefs.SetString("json", _json);
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
            var data = _json.ToDictionary();
            string searchKey = $"{key}{_separator}{defaultValue.Type.ToString()[..2]}{_separator}{_postfix}";
            return data.TryGetValue(searchKey, out JToken value) ? value : defaultValue;
        }

        /// <summary> Save current data in cloud. </summary>
        /// <param name="onSuccess"> After successful save data in cloud. </param>
        /// <param name="onError"> After unsuccessful data saving in the cloud. </param>
        public static void SaveToClaud(Action onSuccess = null, Action<string> onError = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerAccount.SetCloudSaveData(_json, onSuccess, onError);
#else
            PlayerPrefs.SetString("json", _json);
            onSuccess?.Invoke();
#endif
        }

        private static void InitializePlayerPrefs()
        {
            _json = PlayerPrefs.GetString("json", "{}");
            _initialized = true;
        }
    }
}