using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AltraVeraHostInstaller.Utilities
{
    public class Constants
    {
        //public static string GetDefaultIntallationPathX86 { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string GetDefaultIntallationPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string ApplicationName { get; set; } = "AltraVera";
        public static string ConfirmationMessageForClosing { get; set; } = $"{ApplicationName} is running ... Do you want to close before uninstalling ?";

        public static double UIOpacityEnable = 1;

        public static double UIOpacityDisable = 0.8;
        public static string ZipPath => $"{ApplicationName}.zip";
        public static Uri uri { get; set; } = new Uri(@"https://www.dropbox.com/scl/fi/6v0y6sd6s9opffjub11jp/AltraVeraHost.zip?rlkey=bvtzqtlmhzlwq1nwkbf0q2jac&dl=1");
        public static string GetLocalFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string InstallerFolder => $"{GetLocalFolder}\\{ApplicationName}Installer";
        public static string AssemblyName => Assembly.GetEntryAssembly().GetName().Name;

        public static string ShortCutDescription { get; set; } = "AltraVera";
        public static string IconFileName { get; internal set; } = "AltraVera.ico";
        public static string GetInstallerExe => Assembly.GetEntryAssembly().Location;

        public static void RunFolderDelete(string folderToDelete)
        {
            try
            {
                // Path to temp file
                string tempPath = Path.Combine(Path.GetTempPath(), "FolderDelete.bat");

                // Extract the embedded batch file
                using (Stream stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("AltraVeraHostInstaller.FolderDelete.bat")) 
                using (FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }

                // Prepare to launch silently
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = tempPath,
                    Arguments = $"\"{folderToDelete}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                
            }
        }
        
       
        
        
    }
}
