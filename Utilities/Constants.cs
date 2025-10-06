using System;

namespace CustomRDPInstaller.Utilities
{
    public class Constants
    {
        public static string GetDefaultIntallationPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string ApplicationName { get; set; } = "AltraVera";
    }
}
