using Kimicu.YandexGames.Utils;
using Newtonsoft.Json;

namespace Kimicu.YandexGames.Extension
{
    public static class FileExtensions
    {
        public static void SaveObject<T>(this string fileName, T obj)
        {
            FileUtility.EditOrCreateFile(fileName, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        public static T LoadObject<T>(string fileName, T defaultObj, bool createInAbsence = true)
        {
            return JsonConvert.DeserializeObject<T>(FileUtility.ReadFile(fileName, JsonConvert.SerializeObject(defaultObj, Formatting.Indented), createInAbsence));
        }
    }
}