using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Agava.YandexGames
{
	public static class YandexGame
	{
		private static Action<GetAllGamesResponse> _onGetAllGamesSuccessCallback;
		private static Action<string> _onGetAllGamesErrorCallback;
		
		private static Action<GetGameByIdResponse> _onGetGameByIdSuccessCallback;
		private static Action<string> _onGetGameByIdErrorCallback;

		#region AllGames

		public static void GetAllGames(
			Action<GetAllGamesResponse> onSuccessCallback = null,
			Action<string> onErrorCallback = null)
		{
			_onGetAllGamesSuccessCallback = onSuccessCallback;
			_onGetAllGamesErrorCallback = onErrorCallback;
			
			YandexGameGetAllGames(OnGetAllGamesSuccessCallback, OnGetAllGamesErrorCallback);
		}

		[DllImport("__Internal")]
		private static extern void YandexGameGetAllGames(
			Action<string> onSuccessCallback,
			Action<string> onErrorCallback
		);

		[MonoPInvokeCallback(typeof(Action<string>))]
		private static void OnGetAllGamesSuccessCallback(string allGamesResponseJson)
		{
			if (YandexGamesSdk.CallbackLogging)
				Debug.Log($"{nameof(YandexGame)}.{nameof(OnGetAllGamesSuccessCallback)} invoked, {nameof(allGamesResponseJson)} = {allGamesResponseJson}");

			GetAllGamesResponse response = JsonUtility.FromJson<GetAllGamesResponse>(allGamesResponseJson);

			_onGetAllGamesSuccessCallback?.Invoke(response);
		}

		[MonoPInvokeCallback(typeof(Action<string>))]
		private static void OnGetAllGamesErrorCallback(string errorMessage)
		{
			if (YandexGamesSdk.CallbackLogging)
				Debug.Log($"{nameof(YandexGame)}.{nameof(OnGetAllGamesErrorCallback)} invoked, {nameof(errorMessage)} = {errorMessage}");

			_onGetAllGamesErrorCallback?.Invoke(errorMessage);
		}

		#endregion

		#region GameById

		public static void GetGameById(
			string appID,
			Action<GetGameByIdResponse> onSuccessCallback = null,
			Action<string> onErrorCallback = null)
		{
			_onGetGameByIdSuccessCallback = onSuccessCallback;
			_onGetGameByIdErrorCallback = onErrorCallback;

			YandexGameGetGameById(appID, OnGetGameByIdSuccessCallback, OnGetGameByIdErrorCallback);
		}

		[DllImport("__Internal")]
		private static extern void YandexGameGetGameById(
			string appID,
			Action<string> onSuccessCallback,
			Action<string> onErrorCallback
		);
		
		[MonoPInvokeCallback(typeof(Action<string>))]
		private static void OnGetGameByIdSuccessCallback(string gameByIdResponseJson)
		{
			if (YandexGamesSdk.CallbackLogging)
				Debug.Log($"{nameof(YandexGame)}.{nameof(OnGetGameByIdSuccessCallback)} invoked, {nameof(gameByIdResponseJson)} = {gameByIdResponseJson}");

			GetGameByIdResponse response = JsonUtility.FromJson<GetGameByIdResponse>(gameByIdResponseJson);

			_onGetGameByIdSuccessCallback?.Invoke(response);
		}

		[MonoPInvokeCallback(typeof(Action<string>))]
		private static void OnGetGameByIdErrorCallback(string errorMessage)
		{
			if (YandexGamesSdk.CallbackLogging)
				Debug.Log($"{nameof(YandexGame)}.{nameof(OnGetGameByIdErrorCallback)} invoked, {nameof(errorMessage)} = {errorMessage}");

			_onGetGameByIdErrorCallback?.Invoke(errorMessage);
		}

		#endregion
	}
}