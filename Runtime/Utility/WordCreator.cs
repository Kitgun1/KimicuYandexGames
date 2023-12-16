using System.Collections.Generic;
using System.IO;

namespace Kimicu.YandexGames.Utility
{
    public static class WordCreator
    {
        public static void CreateWordDocument(string templatePath, string outputPath, Dictionary<string, string> replacements)
        {
            File.Copy(templatePath, outputPath, true);

            string docText = File.ReadAllText(outputPath);

            foreach (var replacement in replacements)
            {
                docText = docText.Replace("{" + replacement.Key + "}", replacement.Value);
            }

            File.WriteAllText(outputPath, docText);
        }
    }
}