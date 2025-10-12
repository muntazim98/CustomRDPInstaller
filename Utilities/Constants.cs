using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace CustomRDPInstaller.Utilities
{
    public class Constants
    {
       
        public static string GetDefaultIntallationPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string ApplicationName { get; set; } = "AltraVera";
        public static string AgentName = "AltraVera-Agent";
        public static string ConfirmationMessageForClosing { get; set; } = $"{AgentName} is running... Do you want to stop & uninstall ?";
        public static double UIOpacityEnable = 1;
        public static double UIOpacityDisable = 0.8;
        public static string ServiceName => "AltraVeraAgentService";
        public static string ServiceExeName => "AltraVera_agent_service.exe";
        public static string ZipPath => $"{ApplicationName}Service.zip";
        public static Uri uri { get; set; } = new Uri(@"https://www.dropbox.com/scl/fi/iskoq68iqy1o4wg7kqpxm/AltraVeraAgent.zip?rlkey=li9g8t5u4ipyc00v9wm3qfsve&dl=1");
        public static string GetLocalFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string InstallerFolder => $"{GetLocalFolder}\\{ApplicationName}Installer";
        public static string AssemblyName => System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

        public static string ShortCutDescription { get; set; } = "AltraVera";
        public static string IconFileName { get; internal set; } = "AltraVera.ico";
        public static string GetInstallerExe => System.Reflection.Assembly.GetEntryAssembly().Location;

        public static string InstallServiceName = "install_service.bat";

        public static string UnInstallServiceName = "uninstall_service.bat";
         
        
        public static bool ServiceExists(string serviceName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }
        public static void InstallService(string filepath)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = filepath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle= ProcessWindowStyle.Hidden,
                    Verb = "runas"
                };
                Process.Start(processStartInfo);
            }
            catch (Exception)
            {

            }

        }
        public static void UnInstallService(string filepath)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = filepath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas"
                };
                Process.Start(processStartInfo);
            }
            catch (Exception)
            {

            }
        }
    }
}
