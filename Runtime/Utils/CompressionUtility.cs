using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Kimicu.YandexGames.Utils
{
    public static class CompressionUtility
    {
        public static string LoadFromBase64(string @string)
        {
            if (!string.IsNullOrEmpty(@string) && !string.IsNullOrWhiteSpace(@string))
            {
                var compressedData = Convert.FromBase64String(@string);
                var decompressString = DecompressString(compressedData);

                return decompressString;
            }
            
            return string.Empty;
        }

        public static string SaveToBase64(string @string)
        {
            var compressedData = CompressString(@string);
            var base64String = Convert.ToBase64String(compressedData);

            return base64String;
        }

        public static byte[] CompressString(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");

            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            using (var writer = new StreamWriter(gzipStream, Encoding.UTF8))
            {
                writer.Write(input);
            }
            return outputStream.ToArray();
        }

        public static string DecompressString(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                throw new ArgumentNullException(nameof(compressedData), "Compressed data cannot be null or empty.");

            using var inputStream = new MemoryStream(compressedData);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}