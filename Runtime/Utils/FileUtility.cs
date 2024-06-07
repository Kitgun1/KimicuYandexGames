using System.IO;
using UnityEngine;

namespace Kimicu.YandexGames.Utils
{
    public static class FileUtility
    {
        private const string FOLDER_NAME = "EditorCloud";
        private const string FILE_EXTENSION = ".txt";
        
        public static void EditOrCreateFile(string name, string content)
        {
            string folderPath = Path.Combine(Application.dataPath, "..", FOLDER_NAME);
            string filePath = Path.Combine(folderPath, name + FILE_EXTENSION);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, content);
            }
            else
            {
                File.WriteAllText(filePath, content);
            }
        }

        public static string ReadFile(string name, string defaultContent = "", bool createInAbsence = false)
        {
            string filePath = Path.Combine(Application.dataPath, "..", FOLDER_NAME, name + FILE_EXTENSION);
        
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                return content;
            }

            if(createInAbsence) EditOrCreateFile(name, defaultContent);
            return defaultContent;
        }
    }
}