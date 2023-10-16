using UnityEngine.EventSystems;

namespace Kimicu.YandexGames
{
    /// <summary>
    /// On mobile Google Chrome, fixes unresponsive UI controls after going to home screen and back to the browser.
    /// </summary>
    /// <remarks>This happens due to OnApplicationFocus callback not working properly.</remarks>
    public class WebEventSystem : EventSystem
    {
        protected override void OnApplicationFocus(bool hasFocus) => base.OnApplicationFocus(true);
    } 
}