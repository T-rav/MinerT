using System;
using System.IO;

namespace MinerT.kungfuactiongrip.com
{
    public static class FileHelper
    {
        public static void WriteToFile(string fileName, string contents, bool isAppend = false)
        {
            var filePath = BuildFilePath(fileName);

            if (isAppend)
            {
                File.AppendAllText(filePath, contents);
            }
            else
            {
                File.WriteAllText(filePath, contents);    
            }

        }

        public static string ReadFromFile(string fileName)
        {
            var filePath = BuildFilePath(fileName);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return string.Empty;
        }

        public static bool DoesFileExist(string fileName)
        {
            var path = BuildFilePath(fileName);
            return File.Exists(path);
        }

        public static string BuildFilePath(string fileName)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDir = Path.Combine(appData, "MinerT");
            var filePath = Path.Combine(appDir,fileName);
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }
            return filePath;
        }
    }
}
