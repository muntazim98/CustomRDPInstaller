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
    }
}
