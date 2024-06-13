using System;
using PlayerAccount = Agava.YandexGames.PlayerAccount;

namespace Kimicu.YandexGames
{
    public static partial class Account
    {
        private static bool _isAuthorizedInEditor = false;
        private static bool _hasPersonalProfileDataPermissionInEditor = false;
        
        private static event Action AuthorizedInBackground;

        /// <summary> Use this before calling SDK methods that require authorization. </summary>
        public static bool IsAuthorized =>
#if !UNITY_EDITOR && UNITY_WEBGL
            PlayerAccount.IsAuthorized;
#else
            _isAuthorizedInEditor;
#endif
        
        /// <summary> Permission to use name and profile picture from the Yandex account. </summary>
        /// <remarks> Requires authorization. Use <see cref="IsAuthorized"/> and <see cref="Authorize"/>. </remarks>
        public static bool HasPersonalProfileDataPermission =>
#if !UNITY_EDITOR && UNITY_WEBGL
            PlayerAccount.HasPersonalProfileDataPermission;
#else
            _hasPersonalProfileDataPermissionInEditor;
#endif

#region Authorize Event

        /// <summary> The AuthorizedInBackground event occurs when authorization occurs in the background. </summary>
        public static void OnAuthorizedInBackgroundAdd(Action onAuthorizedInBackground)
#if !UNITY_EDITOR && UNITY_WEBGL
            => PlayerAccount.AuthorizedInBackground += onAuthorizedInBackground;
#else
            => AuthorizedInBackground += onAuthorizedInBackground;
#endif

        /// <summary> The AuthorizedInBackground event occurs when authorization occurs in the background. </summary>
        public static void OnAuthorizedInBackgroundRemove(Action onAuthorizedInBackground)
#if !UNITY_EDITOR && UNITY_WEBGL
            => PlayerAccount.AuthorizedInBackground -= onAuthorizedInBackground;
#else
            => AuthorizedInBackground -= onAuthorizedInBackground;
#endif

#endregion

        /// <summary> Request the permission to get Yandex account name and profile picture. </summary>
        /// <remarks>
        /// Be aware, if user rejects the request - it's permanent. The request window will never open again.<br/>
        /// Requires authorization. Use <see cref="IsAuthorized"/> and <see cref="Authorize"/>.
        /// </remarks>
        public static void RequestPersonalProfileDataPermission(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            PlayerAccount.RequestPersonalProfileDataPermission();
#else
            _hasPersonalProfileDataPermissionInEditor = true;
#endif
        }

        /// <summary> ??? </summary>
        public static void StartAuthorizationPolling(int delay, Action successCallback = null, Action errorCallback = null)
#if !UNITY_EDITOR && UNITY_WEBGL
            => PlayerAccount.StartAuthorizationPolling(delay, successCallback, errorCallback);
#else
        {
            _isAuthorizedInEditor = true;
            AuthorizedInBackground?.Invoke();
        }
#endif

        /// <summary> Calls a scary authorization window upon the user. Be very afraid. </summary>
        public static void Authorize(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
#if !UNITY_EDITOR && UNITY_WEBGL
            => PlayerAccount.Authorize(onSuccessCallback, onErrorCallback);
#else
        {
            _isAuthorizedInEditor = true;
            onSuccessCallback?.Invoke();
        }
#endif
    }
}