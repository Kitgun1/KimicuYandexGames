using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Agava.YandexGames
{
    public static class YandexScreen
    {
        private const string STATUS_ON = "on";
        private const string STATUS_OFF = "off";

        public static bool IsFullscreen { get; private set; }

        public static event Action<bool> FullscreenStatusChanged;

        private static bool _isInitialized;
        
        private static Action _onInitializeSuccessCallback;
        private static Action<string> _onInitializeErrorCallback;
        
        private static Action _onRequestFullscreenSuccessCallback;
        private static Action<string> _onRequestFullscreenErrorCallback;
        
        private static Action _onExitFullscreenSuccessCallback;
        private static Action<string> _onExitFullscreenErrorCallback;

        public static IEnumerator Initialize(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(Initialize)} invoked");
            
            if (_isInitialized)
            {
                onSuccessCallback?.Invoke();
                
                if (YandexGamesSdk.CallbackLogging)
                    Debug.Log($"{nameof(YandexScreen)}.{nameof(Initialize)} invoked. YandexScreen is already initialized.");
                
                yield break;
            }

            _onInitializeSuccessCallback = onSuccessCallback;
            _onInitializeErrorCallback = onErrorCallback;

            FullscreenStatusChanged += OnFullscreenStatusChanged;

#if UNITY_EDITOR
            OnGetFullscreenStatusSuccessCallback(STATUS_ON);
#else
            SubscribeFullscreenStatusChange(OnGetFullscreenStatusSuccessCallback, OnGetFullscreenStatusErrorCallback);
            GetFullscreenStatus(OnGetFullscreenStatusSuccessCallback, OnGetFullscreenStatusErrorCallback);

            while (_isInitialized == false)
                yield return null;
#endif
            
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(Initialize)} invoked. YandexScreen initialized.");
        }

        public static void RequestFullscreenMode(Action onSuccess = null, Action<string> onError = null)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(RequestFullscreenMode)} invoked");
            
            if (YandexGamesSdk.IsInitialized == false)
                throw new InvalidOperationException("YandexGamesSdk is not initialized!");

            if (_isInitialized == false)
                throw new InvalidOperationException("YandexScreen is not initialized!");
            
            if (IsFullscreen)
            {
                if (YandexGamesSdk.CallbackLogging)
                    Debug.Log($"{nameof(YandexScreen)}.{nameof(RequestFullscreenMode)} invoked. Fullscreen mode is already enabled.");
                
                onSuccess?.Invoke();
                return;
            }
            
            _onRequestFullscreenSuccessCallback = onSuccess;
            _onRequestFullscreenErrorCallback = onError;
            
#if UNITY_EDITOR
            onSuccess?.Invoke();
#else      
            RequestFullscreen(OnRequestFullscreenSuccessCallback, OnRequestFullscreenErrorCallback);
#endif
        }

        public static void ExitFullscreenMode(Action onSuccess = null, Action<string> onError = null)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(ExitFullscreenMode)} invoked");
            
            if (YandexGamesSdk.IsInitialized == false)
                throw new InvalidOperationException("YandexGamesSdk is not initialized!");
            
            if (_isInitialized == false)
                throw new InvalidOperationException("YandexScreen is not initialized!");
            
            if (!IsFullscreen)
            {
                if (YandexGamesSdk.CallbackLogging)
                    Debug.Log($"{nameof(YandexScreen)}.{nameof(ExitFullscreenMode)} invoked. Fullscreen mode is already disabled.");
                
                onSuccess?.Invoke();
                return;
            }
            
            _onExitFullscreenSuccessCallback = onSuccess;
            _onExitFullscreenErrorCallback = onError;

#if UNITY_EDITOR
            onSuccess?.Invoke();
#else      
            ExitFullscreen(OnExitFullscreenSuccessCallback, OnExitFullscreenErrorCallback);
#endif
            
        }
        
        [DllImport("__Internal")]
        private static extern void RequestFullscreen(Action onSuccessCallback, Action<string> onErrorCallback);

        [DllImport("__Internal")]
        private static extern void ExitFullscreen(Action onSuccessCallback, Action<string> onErrorCallback);

        [DllImport("__Internal")]
        private static extern void GetFullscreenStatus(Action<string> onSuccessCallback, Action<string> onErrorCallback);

        [DllImport("__Internal")]
        private static extern void SubscribeFullscreenStatusChange(Action<string> onChangeCallback, Action<string> onErrorCallback);
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestFullscreenSuccessCallback()
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnRequestFullscreenSuccessCallback)} invoked");
            
            _onRequestFullscreenSuccessCallback?.Invoke();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestFullscreenErrorCallback(string errorMessage)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnRequestFullscreenErrorCallback)} invoked, {nameof(errorMessage)} = {errorMessage}");

            _onRequestFullscreenErrorCallback?.Invoke(errorMessage);
        }
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnExitFullscreenSuccessCallback()
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnExitFullscreenSuccessCallback)} invoked");

            _onExitFullscreenSuccessCallback?.Invoke();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnExitFullscreenErrorCallback(string errorMessage)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnExitFullscreenErrorCallback)} invoked, {nameof(errorMessage)} = {errorMessage}");

            _onExitFullscreenErrorCallback?.Invoke(errorMessage);
        }
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnGetFullscreenStatusSuccessCallback(string status)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnGetFullscreenStatusSuccessCallback)} invoked, {nameof(status)} = {status}");

            UpdateFullscreenStatus(status);
            
            if (!_isInitialized)
            {
                _isInitialized = true;
                _onInitializeSuccessCallback?.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnGetFullscreenStatusErrorCallback(string errorMessage)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnGetFullscreenStatusErrorCallback)} invoked, {nameof(errorMessage)} = {errorMessage}");

            _onInitializeErrorCallback?.Invoke(errorMessage);
        }
        
        private static void UpdateFullscreenStatus(string status)
        {
            var isFullscreen = status == STATUS_ON;
            
            if (IsFullscreen != isFullscreen)
            {
                IsFullscreen = isFullscreen;
                FullscreenStatusChanged?.Invoke(isFullscreen);
            }
        }

        private static void OnFullscreenStatusChanged(bool isFullscreen)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(YandexScreen)}.{nameof(OnFullscreenStatusChanged)} invoked, {nameof(isFullscreen)} = {isFullscreen}");
        }
    }
}