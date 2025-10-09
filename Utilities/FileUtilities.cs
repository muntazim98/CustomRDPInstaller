using System;
using System.IO;

namespace AltraVeraInstaller.Utilities
{
    public class FileUtilities
    {
        public static bool CreateFile(string FilePath)
        {
            try
            {
                if (!File.Exists(FilePath))
                    File.Create(FilePath).Close();
                return true;
            }
            catch { return false; }
        }
        public static bool DeleteFile(string FilePath)
        {
            try
            {
                File.Delete(FilePath);
                return true;
            }
            catch { return false; }
        }
        public static long GetDirectorySize(string folderPath)
        {
            long size = 0;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

                // Add file sizes
                foreach (FileInfo file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    size += file.Length;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
            }
            catch (Exception ex)
            {
            }
            return size;
        }
        public static bool CopyFiles(string source, string dest, bool Overwrite = true)
        {
            try
            {
                File.Copy(source, dest, Overwrite);
                return true;
            }
            catch { return false; }
        }

        
    }
}
