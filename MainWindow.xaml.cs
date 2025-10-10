using AltraVeraInstaller.Utilities;
using CustomRDPInstaller.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomRDPInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static MainWindow instance;
        private string _defaultPath = Constants.GetDefaultIntallationPath;
        public static int StepCount = 1;
        public static int UninstallStepCount = 1;

        private bool isConnected;
        public static string InstalledLocation {  get; private set; }
        public string DefaultPath
        {
            get => _defaultPath;
            set
            {
                _defaultPath = value;
                OnPropertyChanged(nameof(DefaultPath));
            }
        }

        
        public static MainWindow GetInstance => instance;
        public MainWindow()
        {
            InitializeComponent();
            if (!IsAdministrator())
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Constants.GetInstallerExe,
                    Arguments = string.Empty,
                    UseShellExecute = true,
                    Verb = "runas"
                };


                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception) { }
                finally
                {
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
                return;
            }
            instance = this;
            DataContext = this;
            var isAlreadyInstalled = CheckInstalled(Constants.ApplicationName);
            this.Loaded += async (s, e) =>
            {
                if (isAlreadyInstalled.IsInstalled)
                {
                    InstalledLocation = isAlreadyInstalled.InstalledLocation;
                    showUninstallationScreen();
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    StepNext();
                    await Task.Delay(TimeSpan.FromSeconds(4));
                    StepNext();
                }
            };
        }
        private System.Timers.Timer timeoutTimer;
        private int timeoutInMilliseconds = 30;
        private void StepNext()
        {
            switch (StepCount)
            {
                case 1:
                    StartingGrid.Visibility = Visibility.Collapsed;
                    LogoGrid.Visibility = Visibility.Visible;
                    ContentBorder.Visibility = Visibility.Visible;
                    WelcomeGrid.Visibility = Visibility.Visible;
                    NegativeButton.Visibility = Visibility.Collapsed;
                    PositiveButton.Content = "Install Now";
                    PositiveButton.IsEnabled = false;
                    PositiveButton.Width = 300;
                    Heading1.Text = "Please wait while Setup Wizard prepares to guide you through the installation.";
                    Heading2.Visibility = Visibility.Collapsed;
                    Heading3.Text = "Computing space requirements...";
                    StepCount += 1;
                    break;
                case 2:
                    StartingGrid.Visibility = Visibility.Collapsed;
                    LogoGrid.Visibility = Visibility.Visible;
                    ContentBorder.Visibility = Visibility.Visible;
                    WelcomeGrid.Visibility = Visibility.Visible;
                    NegativeButton.Visibility = Visibility.Collapsed;
                    PositiveButton.Content = "Install Now";
                    PositiveButton.IsEnabled = true;
                    PositiveButton.Width = 300;
                    Heading1.Text = $"This wizard will guide you through the installation of {Constants.AgentName}.";
                    Heading2.Visibility = Visibility.Visible;
                    Heading2.Text = "It is recommended that you close all other applications before starting Setup. This will make it possible to update relevant system files without having to reboot your computer.";
                    Heading3.Text = "Click Install Now to continue.";
                    StepCount += 1;
                    break;
                
                case 3:
                    WelcomeGrid.Visibility = Visibility.Collapsed;
                    FolderSelectionGrid.Visibility = Visibility.Collapsed;
                    LicensingGrid.Visibility = Visibility.Collapsed;
                    NegativeButton.Content = "Cancel";
                    InstallingGridTextBlock1.Text = $"Please wait while we are installing {Constants.AgentName} on your Computer.";
                    NegativeButton.Visibility = Visibility.Visible;
                    InstallingGrid.Visibility = Visibility.Visible;
                    isConnected = IsInternetAvailable();
                    if (isConnected)
                    {
                        InternetStatus.Text = "Connected";
                        InternetStatus.Foreground = new SolidColorBrush(Colors.LimeGreen); ;
                    }
                    else
                    {
                        InternetStatus.Text = "Not connected";
                        InstallingGridTextBlock1.Text = "Unable to download, please check your internet connection.";
                        InternetStatus.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        return;
                    }
                    NegativeButton.Background = new SolidColorBrush(Colors.Black);
                    PositiveButton.Width = 170;
                    PositiveButton.IsEnabled = false;
                    PositiveButton.Content = "Install";
                    PositiveButton.Visibility = Visibility.Collapsed;
                    NegativeButton.HorizontalAlignment = HorizontalAlignment.Right;
                    DefaultPath = Path.Combine(DefaultPath, Constants.ApplicationName+"Service");
                    DirectoryUtility.CreateDirectory(DefaultPath, Overwrite: true);
                    InstallAltraVera();
                    StepCount += 1;
                    break;
                case 4:
                    WelcomeGrid.Visibility = Visibility.Collapsed;
                    FolderSelectionGrid.Visibility = Visibility.Collapsed;
                    LicensingGrid.Visibility = Visibility.Collapsed;
                    NegativeButton.Content = "Close";
                    InstallingGrid.Visibility = Visibility.Collapsed;
                    SuccessCompletion.Visibility = Visibility.Visible;
                    NegativeButton.Background = new SolidColorBrush(Color.FromRgb(9, 151, 215));
                    PositiveButton.Width = 170;
                    PositiveButton.IsEnabled = true;
                    PositiveButton.Content = "Launch";
                    NegativeButton.Visibility = Visibility.Visible;
                    PositiveButton.Visibility = Visibility.Collapsed;
                    NegativeButton.HorizontalAlignment = HorizontalAlignment.Left;
                    StepCount += 1;
                    break;
            }
        }
        private void Browsebtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("8.8.8.8", 1000);
                    return result.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        

        private void DragThisWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch { }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DiskCostBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void MoveNext(object sender, RoutedEventArgs e)
        {
            if (PositiveButton.Content.ToString() == "Launch" || PositiveButton.Content.ToString() == "Close")
            {
                this.Close();
            }
            else if(PositiveButton.Content.ToString() == "Uninstall")
            {
                    bool isOpen = await CheckByProcess();
                    var IsOkClicked = !isOpen || DialogUtility.ShowMessageBoxModel(msg: Constants.ConfirmationMessageForClosing, UI: this);
                    if (IsOkClicked)
                    {
                        RemoveAltraveraTextBlock2.Visibility = Visibility.Collapsed;
                        RemoveAltraveraTextBlock3.Visibility = Visibility.Collapsed;
                        RemoveAltraveraTextBlock4.Visibility = Visibility.Collapsed;
                        pathTextbox.Visibility = Visibility.Collapsed;
                        UninstallingProgressText.Visibility = Visibility.Visible;
                        UnInstallingProgressBar.Visibility = Visibility.Visible;
                        UninstallingTextBlock.Text = $"Uninstalling {Constants.AgentName}, Please wait a moment...";
                        //await Constants.UninstallService(Constants.ServiceName);
                        var installService = Path.Combine(InstalledLocation, Constants.UnInstallServiceName);
                        Constants.UnInstallService(installService);
                        await Task.Delay(5000);
                        while (UnInstallingProgressBar.Value < UnInstallingProgressBar.Maximum)
                        {
                            await Task.Delay(25);
                            UnInstallingProgressBar.Value += 1;
                        }
                        await UnInstallByRegistry();
                        UnInstallingProgressBar.Visibility = Visibility.Collapsed;
                        CompletedImage.Visibility = Visibility.Visible;
                        UninstallingTextBlock.Text = "Uninstallation Completed";
                        RemoveAltraveraTextBlock1.Text = $"{Constants.AgentName} has been Successfully removed from your computer.";
                        UninstallSeparator.Visibility = Visibility.Collapsed;
                        UninstallingProgressText.Visibility = Visibility.Collapsed;
                        PositiveButton.Content = "Close";
                        NegativeButton.Visibility = Visibility.Collapsed;
                        PositiveButton.Visibility = Visibility.Visible;
                        NegativeButton.Opacity = 0.5;
                        NegativeButton.IsEnabled = false;
                    }
                //}
                return;
            }
            StepNext();
        }
        public bool IsServiceRunning(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);
            return sc.Status == ServiceControllerStatus.Running;
        }
        private void MovePrevious(object sender, RoutedEventArgs e)
        {
            if (NegativeButton.Content.ToString() == "Done" || NegativeButton.Content.ToString() == "Close")
            {
                Application.Current.Shutdown();
            }
            else if (NegativeButton.Content.ToString() == "Cancel")
            {
                string message = NegativeButton.Content.ToString() == "Cancel" && PositiveButton.Content.ToString() == "Uninstall" ? "Do you want to cancel the uninstallation ?" : "Do you want to cancel the installation ?";
                var IsOk = DialogUtility.ShowMessageBoxModel(false, message, false, this);
                if (IsOk)
                {
                    Application.Current.Shutdown();
                }

            }
        }
        private void InstallAltraVera()
        {
            Task.Factory.StartNew(async () =>
            {
                var FileName = Path.Combine(DefaultPath, Constants.ZipPath);
                try
                {
                    await Task.Delay(2000);
                    FileUtilities.CreateFile(FileName);
                    WebClient wc = new WebClient();
                    wc.DownloadFileAsync(Constants.uri, FileName);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                }
                catch (Exception)
                {
                }
            });
        }
        private async Task<bool> CheckByProcess()
        {
            try
            {
                return IsServiceRunning(Constants.ServiceName);
            }
            catch
            { return false; }
        }
        private async void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            await App.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    ProgressBar.Value = e.ProgressPercentage;
                }
                catch (Exception)
                {
                    ProgressBar.Value = 0;
                }
            });
        }
        private async void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (e.Error == null)
                {
                    await Task.Delay(1000);
                    var ZipPath = Path.Combine(DefaultPath, Constants.ZipPath);
                    await Task.Run(() => ZipFile.ExtractToDirectory(ZipPath, DefaultPath));
                    await Task.Delay(3000);
                    FileUtilities.DeleteFile(ZipPath);
                    //await CreateRegistry();
                    //await CreateShortCut();
                    var ApplicationToLaunch = Path.Combine(DefaultPath, Constants.ServiceExeName);
                    //await Constants.CreateAndStartServiceAsync(Constants.ServiceName, ApplicationToLaunch);
                    var installService = Path.Combine(DefaultPath, Constants.InstallServiceName);
                    Constants.InstallService(installService);
                    StepNext();
                }
            });
        }
       
       
       
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            PositiveButton.IsEnabled = true;
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            PositiveButton.IsEnabled = false;
        }

        private static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

       
        public InstalledInfo CheckInstalled(string findByName)
        {
            var servicePath = GetServiceExecutablePath(Constants.ServiceName);
            var exists = Constants.ServiceExists(Constants.ServiceName);
            var installedInfo = new InstalledInfo() { DisplayName = Constants.ServiceName, IsInstalled = exists, InstalledLocation = servicePath };
            return installedInfo;
            
        }


        public static string GetServiceExecutablePath(string serviceName)
        {
            string query = $"SELECT * FROM Win32_Service WHERE Name = '{serviceName}'";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (var service in searcher.Get())
                {
                    string path = service["PathName"]?.ToString();
                    if (!string.IsNullOrEmpty(path))
                    {
                        return Path.GetDirectoryName(path?.Replace("\"",""));
                    }
                }
            }
            return null;
        }
        private void showUninstallationScreen()
        {
            StartingGrid.Visibility = Visibility.Collapsed;
            LogoGrid.Visibility = Visibility.Visible;
            ContentBorder.Visibility = Visibility.Visible;
            UninstallationGrid.Visibility = Visibility.Visible;
            PositiveButton.Content = "Uninstall";
            NegativeButton.Content = "Cancel";
            RemoveAltraveraTextBlock1.Visibility = Visibility.Visible;
            NegativeButton.Background = new SolidColorBrush(Colors.Black);
        }

        #region Uninstall By Registry

        private async Task UnInstallByRegistry()
        {
            try
            {
                

                #region Remove Directory
                try
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            if (Directory.Exists(InstalledLocation))
                            {
                                DeleteFilesAndDirectory(InstalledLocation);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
                catch { }
                finally
                {
                    FileUtilities.DeleteFile($"{Constants.InstallerFolder}\\{Constants.ServiceExeName}");
                    DirectoryUtility.DeleteDirectory(InstalledLocation);
                }
                #endregion
            }
            catch { }
        }
        private void DeleteFilesAndDirectory(string uninstallString)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(uninstallString);
                var files = dirInfo.GetFiles();
                foreach (FileInfo file in files)
                    FileUtilities.DeleteFile(file.FullName);
                var dirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                    DirectoryUtility.DeleteDirectory(dir.FullName);
            }
            catch { }
        }
        #endregion

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string message =  "Do you want to close the installer ?";
                var IsOk = DialogUtility.ShowMessageBoxModel(false, message, false, this);
                if (IsOk)
                {
                    Application.Current.Shutdown();
                }
            }
            catch { }
        }
    }
}
