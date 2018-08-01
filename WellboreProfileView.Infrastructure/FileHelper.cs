using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WellboreProfileView.Infrastructure
{
    public static class FileHelper
    {
        public static string GetTempReportFileName(string extension)
        {
            string outputFileName = Path.ChangeExtension(Path.GetRandomFileName(), extension);

            return GetTempDirNotUsedFileName(outputFileName);
        }

        public static void DeleteFile(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            File.Delete(fileName);
        }

        private static string GetTempDirNotUsedFileName(string fileName)
        {
            string tempPath = Path.GetTempPath();
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            return GetNotUsedFileName(tempPath, fileName);
        }

        private static string GetNotUsedFileName(string path, string fileName)
        {
            string notUsedFileName = String.Format(@"{0}{1}", path, ClearIllegalCharacters(fileName));
            if (!File.Exists(notUsedFileName))
            {
                try
                {
                    File.Delete(notUsedFileName);
                }
                catch
                {
                    notUsedFileName = GetNotUsedFileName(notUsedFileName);
                }
            }
            return notUsedFileName;
        }


        private static string GetNotUsedFileName(string fullFileName, bool dontTryDeleteFile = false)
        {
            int i = 0;
            string notUsedFileName = fullFileName;
            while (File.Exists(notUsedFileName))
            {
                try
                {
                    if (i == 0 && dontTryDeleteFile)
                        throw new Exception();

                    File.Delete(notUsedFileName);
                }
                catch
                {
                    notUsedFileName = String.Format(@"{3}\{0}({1}){2}", ClearIllegalCharacters(Path.GetFileNameWithoutExtension(notUsedFileName)), ++i, Path.GetExtension(notUsedFileName), Path.GetDirectoryName(notUsedFileName));
                }
            }
            return notUsedFileName;
        }

        private static string ClearIllegalCharacters(string filePath)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex regex = new Regex(String.Format("[{0}]", Regex.Escape(regexSearch)));
            return regex.Replace(filePath, ".");
        }
    }
}