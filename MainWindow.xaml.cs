using AltraVeraInstaller.Utilities;
using AltraVeraHostInstaller.Utilities;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WindowsShortcutFactory;

namespace AltraVeraHostInstaller
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

        private WebClient wc;
        private System.Timers.Timer timeoutTimer;
        private int timeoutInMilliseconds = 30;
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
                    Heading3.Text = "Computing space requirements ...";
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
                    Heading1.Text = "This wizard will guide you through the installation of AltraVera.";
                    Heading2.Visibility = Visibility.Visible;
                    Heading2.Text = "It is recommended that you close all other applications before starting Setup. This will make it possible to update relevant system files without having to reboot your computer.";
                    Heading3.Text = "Click Install Now to continue.";
                    StepCount += 1;
                    break;
                case 3:
                    WelcomeGrid.Visibility = Visibility.Collapsed;
                    FolderSelectionGrid.Visibility = Visibility.Visible;
                    NegativeButton.Content = "Cancel";
                    NegativeButton.Visibility = Visibility.Visible;
                    NegativeButton.Background = new SolidColorBrush(Colors.Black);
                    PositiveButton.Width = 170;
                    PositiveButton.Content = "Install";
                    StepCount += 1;
                    break;
                case 4:
                    WelcomeGrid.Visibility = Visibility.Collapsed;
                    FolderSelectionGrid.Visibility = Visibility.Collapsed;
                    LicensingGrid.Visibility = Visibility.Visible;
                    NegativeButton.Content = "Cancel";
                    NegativeButton.Visibility = Visibility.Visible;
                    NegativeButton.Background = new SolidColorBrush(Colors.Black);
                    PositiveButton.Width = 170;
                    PositiveButton.IsEnabled = false;
                    PositiveButton.Content = "Install";
                    StepCount += 1;
                    break;
                case 5:
                    WelcomeGrid.Visibility = Visibility.Collapsed;
                    FolderSelectionGrid.Visibility = Visibility.Collapsed;
                    LicensingGrid.Visibility = Visibility.Collapsed;
                    NegativeButton.Content = "Cancel";
                    InstallingGridTextBlock1.Text = "Please wait while we are installing AltraVera on your Computer.";
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
                    DefaultPath = Path.Combine(DefaultPath, Constants.ApplicationName);
                    DirectoryUtility.CreateDirectory(DefaultPath, Overwrite: true);
                    InstallAltraVera();
                    StepCount += 1;
                    break;
                case 6:

                    WelcomeGrid.Visibility = Visibility.Collapsed;
                    FolderSelectionGrid.Visibility = Visibility.Collapsed;
                    LicensingGrid.Visibility = Visibility.Collapsed;
                    NegativeButton.Content = "Done";
                    InstallingGrid.Visibility = Visibility.Collapsed;
                    SuccessCompletion.Visibility = Visibility.Visible;
                    NegativeButton.Background = new SolidColorBrush(Color.FromRgb(9, 151, 215));
                    PositiveButton.Width = 170;
                    PositiveButton.IsEnabled = true;
                    PositiveButton.Content = "Launch";
                    NegativeButton.Visibility = Visibility.Visible;
                    PositiveButton.Visibility = Visibility.Visible;
                    NegativeButton.HorizontalAlignment = HorizontalAlignment.Left;
                    StepCount += 1;
                    break;
            }
        }
        private void Browsebtn_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder();
        }
        public bool BrowseFolder()
        {
            try
            {
                var browser = new WPFFolderBrowser.WPFFolderBrowserDialog()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    ShowHiddenItems = true,
                    Title = $"Select Folder To Install {Constants.ApplicationName}",
                };
                var result = browser.ShowDialog(Application.Current.MainWindow);
                if (result != null && result == true)
                    DefaultPath = browser.FileName;
            }
            catch { }
            return !string.IsNullOrEmpty(DefaultPath);
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
            if (PositiveButton.Content.ToString() == "Launch")
            {
                var ApplicationToLaunch = Path.Combine(DefaultPath, Constants.ApplicationName + ".exe");
                if (!string.IsNullOrEmpty(ApplicationToLaunch) && File.Exists(ApplicationToLaunch))
                {
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = ApplicationToLaunch,
                        UseShellExecute = true,
                        WorkingDirectory = DefaultPath,
                        CreateNoWindow = true,
                    };
                    Process.Start(processInfo);
                }
                App.Current.Shutdown();
            }
            else if(PositiveButton.Content.ToString() == "Uninstall")
            {
                if (UninstallStepCount == 1)
                {
                    UninstallingTextBlock.Text = "Uninstalling";
                    RemoveAltraveraTextBlock1.Visibility = Visibility.Visible;
                    RemoveAltraveraTextBlock2.Visibility = Visibility.Visible;
                    RemoveAltraveraTextBlock3.Visibility = Visibility.Visible;
                    RemoveAltraveraTextBlock4.Visibility = Visibility.Visible;
                    pathTextbox.Text = InstalledLocation;
                    pathTextbox.Visibility = Visibility.Visible;
                    UninstallStepCount++;
                }
                else
                {
                    bool isOpen = await CheckByProcess();
                    var IsOkClicked = !isOpen || DialogUtility.ShowMessageBoxModel(msg: Constants.ConfirmationMessageForClosing, UI: this);
                    if (IsOkClicked)
                    {
                        PositiveButton.IsEnabled = false;
                        RemoveAltraveraTextBlock2.Visibility = Visibility.Collapsed;
                        RemoveAltraveraTextBlock3.Visibility = Visibility.Collapsed;
                        RemoveAltraveraTextBlock4.Visibility = Visibility.Collapsed;
                        pathTextbox.Visibility = Visibility.Collapsed;
                        UninstallingProgressText.Visibility = Visibility.Visible;
                        UnInstallingProgressBar.Visibility = Visibility.Visible;
                        await UnInstallByRegistry();
                        while (UnInstallingProgressBar.Value < UnInstallingProgressBar.Maximum)
                        {
                            await Task.Delay(20);
                            UnInstallingProgressBar.Value += 1;
                        }
                        UnInstallingProgressBar.Visibility = Visibility.Collapsed;
                        CompletedImage.Visibility = Visibility.Visible;
                        UninstallingTextBlock.Text = "Uninstallation Completed";
                        RemoveAltraveraTextBlock1.Content = "AltraVera has been Successfully removed from your computer.";
                        
                        UninstallSeparator.Visibility = Visibility.Collapsed;
                        UninstallingProgressText.Visibility = Visibility.Collapsed;
                        PositiveButton.IsEnabled = true;
                        PositiveButton.Content = "Finish";
                        PositiveButton.Visibility = Visibility.Visible;
                        NegativeButton.Visibility = Visibility.Collapsed;


                    }
                }
                    return;
            }
            else if(PositiveButton.Content.ToString() == "Finish")
            {
                Constants.RunFolderDelete(Constants.InstallerFolder);
                Application.Current.Shutdown();
            }
                StepNext();
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
                    wc = new WebClient();
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                    timeoutTimer = new System.Timers.Timer(timeoutInMilliseconds * 1000);
                    timeoutTimer.Elapsed += TimeoutTimer_Elapsed;
                    timeoutTimer.Start();
                    wc.DownloadFileAsync(Constants.uri, FileName);
                }
                catch (Exception)
                {
                }
            });
        }
        private void TimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeoutTimer.Stop();
            wc.CancelAsync();
        }
        private async Task<bool> CheckByProcess()
        {
            try
            {
                return Process.GetProcessesByName(Constants.ApplicationName).Any(x => x.ProcessName == Constants.ApplicationName) || Process.GetProcesses().Any(x => x.ProcessName == Constants.ApplicationName);
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
                timeoutTimer.Stop();
                timeoutTimer.Start();
            });
        }
        private async void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                timeoutTimer?.Stop();
                timeoutTimer?.Dispose();
                wc?.Dispose();
                if (e.Error == null)
                {
                    await Task.Delay(1000);
                    var ZipPath = Path.Combine(DefaultPath, Constants.ZipPath);
                    await Task.Run(() => ZipFile.ExtractToDirectory(ZipPath, DefaultPath));
                    await Task.Delay(3000);
                    FileUtilities.DeleteFile(ZipPath);
                    await CreateRegistry();
                    await CreateShortCut();
                    StepNext();
                }
                else
                {
                    isConnected = IsInternetAvailable();
                    InternetStatus.Text = "Not connected";
                    InternetStatus.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    InstallingGridTextBlock1.Text = "Unable to download, please check your internet connection.";
                    DialogUtility.ShowMessageBoxModel(true, "Unable to download, please check your internet connection.");
                    //btn_Retry.Visibility = Visibility.Visible;
                }
            });
        }
        private async Task CreateRegistry()
        {
            await Task.Run(() =>
            {
                DirectoryUtility.CreateDirectory(Constants.InstallerFolder);
                var dest = Path.Combine(Constants.InstallerFolder, $"{Constants.AssemblyName}.exe");
                FileUtilities.CopyFiles(Constants.GetInstallerExe, dest);
                // var dest = Constants.GetInstallerExe;
                using (RegistryKey parent = (false ? Registry.LocalMachine : Registry.CurrentUser).OpenSubKey(
                             @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
                {
                    try
                    {
                        RegistryKey key = null;

                        try
                        {
                            string guidText = Guid.NewGuid().ToString("B");
                            key = parent.OpenSubKey($"{Constants.ApplicationName}", true) ??
                                  parent.CreateSubKey($"{Constants.ApplicationName}");
                            Assembly asm = GetType().Assembly;
                            Version v = asm.GetName().Version;
                            var appName = Path.Combine(DefaultPath, $"{Constants.ApplicationName}.exe");
                            string exe = "\"" + appName.Replace("/", "\\\\") + "\"";
                            var versionInfo = FileVersionInfo.GetVersionInfo(appName);
                            var folderSizeInBytes = FileUtilities.GetDirectorySize($"{DefaultPath}");
                            var productVersion = GetProductVersion(versionInfo?.ProductVersion?.ToString() ?? v.ToString());
                            key.SetValue("DisplayName", Constants.ApplicationName);
                            key.SetValue("version", productVersion);
                            key.SetValue("Publisher", "Globussoft");
                            key.SetValue("EstimatedSize", (int)(folderSizeInBytes / 1024), RegistryValueKind.DWord);
                            key.SetValue("DisplayIcon", exe);
                            key.SetValue("DisplayVersion", productVersion);
                            //key.SetValue("Contact", "https://socinator.com/contact-us/");
                            key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                            key.SetValue("InstallLocation", $"{DefaultPath}");
                            key.SetValue("UninstallString", dest);
                        }
                        catch (Exception e)
                        {

                        }
                        finally
                        {
                            if (key != null)
                            {
                                key.Close();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
        }
        private async Task CreateShortCut()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var icon = Directory.GetCurrentDirectory()+ $"\\{Constants.IconFileName}";
                        var ApplicationIcon = Path.Combine(DefaultPath, Constants.IconFileName);
                        var iconPath = string.IsNullOrEmpty(ApplicationIcon) || !File.Exists(ApplicationIcon) ? icon : ApplicationIcon;
                        var targetPath = Path.Combine(DefaultPath, $"{Constants.ApplicationName}.exe");
                        var desktopPath = Environment.GetFolderPath(false ? Environment.SpecialFolder.CommonDesktopDirectory : Environment.SpecialFolder.Desktop);
                        var startmenu = Environment.GetFolderPath(false ? Environment.SpecialFolder.CommonStartMenu : Environment.SpecialFolder.StartMenu);
                        SaveShortCut(desktopPath, targetPath, iconPath);
                        SaveShortCut(startmenu, targetPath, iconPath);
                    }
                    catch (Exception e)
                    {

                    }
                });
            }
            catch { }
        }
        private void SaveShortCut(string shortCutPath, string exePath, string iconPath)
        {
            var shortcutName = Path.Combine(shortCutPath, $"{Constants.ApplicationName}.lnk");
            try
            {
                // WindowsShortcutFactory package

                var shortcut1 = new WindowsShortcut
                {
                    Path = exePath,
                    Description = Constants.ShortCutDescription,
                    IconLocation = iconPath,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                };
                shortcut1.Save(shortcutName);
                shortcut1?.Dispose();
            }
            catch { }
        }
        private string GetProductVersion(string version)
        {
            try
            {
                var verArray = version.Split('+');
                return verArray.FirstOrDefault();
            }
            catch { return version; }
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
        public InstalledInfo CheckInstalled(string findByName)
        {
            #region CheckInstalledByRegistry
            string[] info = new string[3];
            var installedInfo = new InstalledInfo();
            try
            {
                var registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                //64 bits computer
                RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
                RegistryKey key = key64.OpenSubKey(registryKey);
                if (key != null)
                {
                    foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                    {
                        if (subkey.GetValue("DisplayName") is string displayName && displayName.Equals(findByName))
                        {
                            installedInfo.DisplayName = displayName;

                            installedInfo.InstalledLocation = subkey.GetValue("InstallLocation").ToString();
                            installedInfo.UninstallString = subkey.GetValue("UninstallString").ToString();

                            installedInfo.Version = GetProductVersion(subkey.GetValue("DisplayVersion").ToString());
                            installedInfo.IsInstalled = true;
                            break;
                        }
                    }
                    key.Close();
                }
                if (!installedInfo.IsInstalled)
                {
                    registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                    key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                    key = key64.OpenSubKey(registryKey);

                    if (key != null)
                    {
                        foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                        {
                            if (subkey.GetValue("DisplayName") is string displayName && displayName.Equals(findByName))
                            {
                                installedInfo.DisplayName = displayName;

                                installedInfo.InstalledLocation = subkey.GetValue("InstallLocation").ToString();
                                installedInfo.UninstallString = subkey.GetValue("UninstallString").ToString();

                                installedInfo.Version = GetProductVersion(subkey.GetValue("DisplayVersion").ToString());
                                installedInfo.IsInstalled = true;
                                break;
                            }
                        }
                        key.Close();
                    }

                }

            }
            catch (Exception)
            {

            }
            return installedInfo;
            #endregion
        }
        private void showUninstallationScreen()
        {
            StartingGrid.Visibility = Visibility.Collapsed;
            LogoGrid.Visibility = Visibility.Visible;
            ContentBorder.Visibility = Visibility.Visible;
            UninstallationGrid.Visibility = Visibility.Visible;
            PositiveButton.Content = "Uninstall";
            NegativeButton.Content = "Cancel";
            NegativeButton.Background = new SolidColorBrush(Colors.Black);
        }

        #region Uninstall By Registry

        private async Task UnInstallByRegistry()
        {
            try
            {
                #region Remove Registry.
                string InstallerRegLoc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                try
                {
                    RegistryKey homeKey = (Registry.CurrentUser).OpenSubKey(InstallerRegLoc, true);
                    RegistryKey appSubKey = homeKey.OpenSubKey(Constants.ApplicationName);
                    if (appSubKey != null)
                        homeKey.DeleteSubKey(Constants.ApplicationName);
                }
                catch { }
                try
                {
                    RegistryKey homeKeyforAllUser = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc, true);
                    RegistryKey appSubKeyforAllUser = homeKeyforAllUser.OpenSubKey(Constants.ApplicationName);
                    if (appSubKeyforAllUser != null)
                        homeKeyforAllUser.DeleteSubKey(Constants.ApplicationName);
                }
                catch { }
                #endregion

                #region Remove ShortCut
                var appName = Constants.ApplicationName;
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                    string startmenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                    //string taskbar = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar";
                    var dlink = Path.Combine(desktopPath, $"{appName}.lnk");
                    var slink = Path.Combine(startmenu, $"{appName}.lnk");
                    //string tlink = System.IO.Path.Combine(taskbar, $"{appName}.lnk");
                    FileUtilities.DeleteFile(dlink);
                    FileUtilities.DeleteFile(slink);
                    //FileUtilities.DeleteFile(tlink);
                }
                catch { }
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string startmenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                    //string taskbar = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar";
                    var dlink = Path.Combine(desktopPath, $"{appName}.lnk");
                    var slink = Path.Combine(startmenu, $"{appName}.lnk");
                    //string tlink = System.IO.Path.Combine(taskbar, $"{appName}.lnk");
                    FileUtilities.DeleteFile(dlink);
                    FileUtilities.DeleteFile(slink);
                    //FileUtilities.DeleteFile(tlink);
                }
                catch { }
                #endregion

                #region Remove Directory
                try
                {
                    await Task.Run( () =>
                    {
                        try
                        {
                            if (Directory.Exists(InstalledLocation))
                            {
                                DeleteFilesAndDirectory(InstalledLocation);
                            }
                            //else
                            //{
                            //    //Delete Files For for all users.
                            //    var allUsers = await GetAllSystemUsers();
                            //    foreach (var user in allUsers)
                            //    {
                            //        var unInstallString = "";
                            //        if (Directory.Exists(unInstallString))
                            //            DeleteFilesAndDirectory(unInstallString);
                            //    }
                            //}
                        }
                        catch (Exception)
                        {
                        }
                    });
                }
                catch { }
                
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
                    if(PositiveButton.Content.ToString() == "Finish")
                        Constants.RunFolderDelete(Constants.InstallerFolder);
                    Application.Current.Shutdown();
                }
            }
            catch { }
        }
    }
}
