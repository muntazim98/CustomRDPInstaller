using System;

namespace CustomRDPInstaller.Utilities
{
    public class Constants
    {
        public static string GetDefaultIntallationPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string ApplicationName { get; set; } = "AltraVera";
        public static string ConfirmationMessageForClosing { get; set; } = $"{ApplicationName} is running Do you want to close before uninstalling ?";
        public static double UIOpacityEnable = 1;
        public static double UIOpacityDisable = 0.8;
        public static string ZipPath => $"{ApplicationName}.zip";
        public static Uri uri { get; set; } = new Uri(@"https://storage.googleapis.com/powerbrowser-bulids/Power-dev/power-dev-bulids/Power%20Browser%20Dev%20Installer.exe");
        public static string GetLocalFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string InstallerFolder => $"{GetLocalFolder}\\{ApplicationName}Installer";
        public static string AssemblyName => System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

        public static string ShortCutDescription { get; set; } = "";
        public static string IconFileName { get; internal set; }
        public static string GetInstallerExe => System.Reflection.Assembly.GetEntryAssembly().Location;
    }
}
