using System;
using Kimicu.YandexGames;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kimicu.YandexGames
{
    public static class WebGL
    {
        private static readonly UnityEvent<bool> OnBackgroundChanged = new();
        private static readonly UnityEvent<bool> OnChangeGameState = new();

        private static bool _audioChange = false;

        /// <summary> Initialize Listener WebGL. </summary>
        public static void InitializeListener()
        {
            _audioChange = KimicuYandexSettings.Instance.SoundChange;
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
            WebProperty.AdvertOpenedChange.AddListener(AdvertOpenedChange);
            WebProperty.PurchaseWindowOpenedChange.AddListener(PurchaseWindowOpenedChange);
        }

        private static void InBackgroundChange(bool value)
        {
            WebProperty.InGameView = value;
            OnBackgroundChanged?.Invoke(value);

            GameStateCheck();
        }

        private static void AdvertOpenedChange(bool value)
        {
            GameStateCheck();
        }

        private static void PurchaseWindowOpenedChange(bool value)
        {
            GameStateCheck();
        }

        private static void GameStateCheck()
        {
            if (!WebProperty.AdvertOpened && WebProperty.InGameView && !WebProperty.PurchaseWindowOpened)
            {
                if (!_audioChange) AudioListener.pause = false;
                OnChangeGameState?.Invoke(true);
            }
            else
            {
                if (!_audioChange) AudioListener.pause = true;
                OnChangeGameState?.Invoke(false);
            }
        }

        /// <summary> Add a listener to game state changes. </summary>
        /// <remarks> Out of play - false <para> In game - true </para> </remarks>
        public static void AddListener(UnityAction<bool> onChangeGameState = null)
        {
            OnChangeGameState.AddListener(onChangeGameState);
        }

        /// <summary> Remove a listener to game state changes. </summary>
        public static void RemoveListener(UnityAction<bool> onChangeGameState = null)
        {
            OnChangeGameState.RemoveListener(onChangeGameState);
        }

        /// <summary> Remove all a listener to game state changes. </summary>
        public static void RemoveAllListener()
        {
            OnChangeGameState.RemoveAllListeners();
        }
    }
}