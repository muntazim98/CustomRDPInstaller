using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace CustomRDPInstaller.Utilities
{
    public class Constants
    {
        public static string GetDefaultIntallationPathX86 { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string GetDefaultIntallationPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string ApplicationName { get; set; } = "AltraVera";

       

        public static string ConfirmationMessageForClosing { get; set; } = $"{ServiceName} is running... Do you want to stop & uninstall ?";
        public static double UIOpacityEnable = 1;
        public static double UIOpacityDisable = 0.8;
        public static string ServiceName => "AltraVeraAgentService";
        public static string ServiceExeName => "AltraVera_agent_service.exe";
        public static string ZipPath => $"{ApplicationName}Service.zip";
        public static Uri uri { get; set; } = new Uri(@"https://www.dropbox.com/scl/fi/4t9xzulxa3ltn5tgsj2z8/AltraVeraAgentService.zip?rlkey=ls6mjingtn3lllnmbi7w557m1&dl=1");
        public static string GetLocalFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string InstallerFolder => $"{GetLocalFolder}\\{ApplicationName}Installer";
        public static string AssemblyName => System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

        public static string ShortCutDescription { get; set; } = "AltraVera";
        public static string IconFileName { get; internal set; } = "AltraVera.ico";
        public static string GetInstallerExe => System.Reflection.Assembly.GetEntryAssembly().Location;

        public static async Task<bool> CreateAndStartServiceAsync(string serviceName, string exePath)
        {
            // Check if the service already exists
            if (!ServiceExists(serviceName))
            {
                // Create the service
                var createResult = await RunProcessAsync("sc", $"create \"{serviceName}\" binPath= \"{exePath}\" start= auto");

                if (!string.IsNullOrWhiteSpace(createResult.stderr) || createResult.exitCode != 0)
                {
                    return false;
                }
            }

            // Start the service
            var startResult = await RunProcessAsync("sc", $"start \"{serviceName}\"");

            if (!string.IsNullOrWhiteSpace(startResult.stderr) || startResult.exitCode != 0)
            {
                return false;
            }

            return true;
        }
        public static bool ServiceExists(string serviceName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }
        private static async Task<(string stdout, string stderr, int exitCode)> RunProcessAsync(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = psi })
            {
                process.Start();

                var stdoutTask = process.StandardOutput.ReadToEndAsync();
                var stderrTask = process.StandardError.ReadToEndAsync();

                await Task.WhenAll(stdoutTask, stderrTask);
                process.WaitForExit();


                return (stdoutTask.Result, stderrTask.Result, process.ExitCode);
            }
        }
        public static async Task<bool> UninstallService(string serviceName)
        {
            if (ServiceExists(serviceName))
            {
                var service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                    StopService(serviceName);
                await DeleteService(serviceName);
            }
            return !ServiceExists(serviceName);
        }
        private static bool StopService(string serviceName)
        {
            var service = new ServiceController(serviceName);
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
            return service.Status == ServiceControllerStatus.Stopped;
        }
        private static async Task DeleteService(string serviceName)
        {
            try
            {
                await RunProcessAsync("sc", $"delete \"{serviceName}\"");
            }
            catch { }
        }
    }
}
