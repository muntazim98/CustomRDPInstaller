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
    }
}
