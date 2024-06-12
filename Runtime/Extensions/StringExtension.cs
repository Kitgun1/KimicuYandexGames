using System;

namespace Kimicu.YandexGames.Extension
{
    public static class StringExtension
    {
        public static string StringToHex(this string input)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static string HexToString(this string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}