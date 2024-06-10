namespace Kimicu.YandexGames
{
    public static class Device
    {
        public static bool IsMobile
        {
            get
#if !UNITY_EDITOR && UNITY_WEBGL
                => Agava.WebUtility.Device.IsMobile;
#else
                => true;
#endif
        }
    }
}