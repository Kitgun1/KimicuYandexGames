using Kimicu.YandexGames.Extension;

namespace Kimicu.YandexGames
{
    public static class Device
    {
        public static bool IsMobile =>
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.WebUtility.Device.IsMobile;
#else
            FileExtensions.LoadObject("device", true);
#endif
    }
}