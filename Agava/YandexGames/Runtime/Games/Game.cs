using System;
using UnityEngine.Scripting;

namespace Agava.YandexGames
{
	[Serializable]
	public class Game
	{
		[field: Preserve] public string appID;
		[field: Preserve] public string title;
		[field: Preserve] public string url;
		[field: Preserve] public string coverURL;
		[field: Preserve] public string iconURL;
	}
}