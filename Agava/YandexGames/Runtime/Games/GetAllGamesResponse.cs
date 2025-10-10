using System;
using UnityEngine.Scripting;

namespace Agava.YandexGames
{
	[Serializable]
	public class GetAllGamesResponse
	{
		[field: Preserve] public Game[] games;
		[field: Preserve] public string developerURL;
	}
}