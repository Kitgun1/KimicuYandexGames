using System;
using UnityEngine.Scripting;

namespace Agava.YandexGames
{
	[Serializable]
	public class GetGameByIdResponse
	{
		[field: Preserve] public Game game;
		[field: Preserve] public bool isAvailable;
	}
}