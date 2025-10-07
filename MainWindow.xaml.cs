using AltraVeraInstaller.Utilities;
using CustomRDPInstaller.Utilities;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            instance = this;
            DataContext = this;
            this.Loaded += async (s, e) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                StepNext();
                await Task.Delay(TimeSpan.FromSeconds(4));
                StepNext();
            };
        }
        private void StepPrevious()
        {

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
                    Heading3.Text = "Computing space requirements";
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
                    Heading1.Text = "This wizard will guide you through the installation of Virto Sign.";
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
                    NegativeButton.Visibility = Visibility.Visible;
                    InstallingGrid.Visibility = Visibility.Visible;
                    NegativeButton.Background = new SolidColorBrush(Colors.Black);
                    PositiveButton.Width = 170;
                    PositiveButton.IsEnabled = false;
                    PositiveButton.Content = "Install";
                    PositiveButton.Visibility = Visibility.Collapsed;
                    NegativeButton.HorizontalAlignment = HorizontalAlignment.Right;
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
                //Launch the Application here.
                this.Close();
            }
            StepNext();
            if(StepCount == 6)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                StepNext();
            }
        }

        private void MovePrevious(object sender, RoutedEventArgs e)
        {
            if(NegativeButton.Content.ToString() == "Done" )
            {
                    Application.Current.Shutdown();
            }
            else if ( NegativeButton.Content.ToString() == "Cancel")
            {
                string message = NegativeButton.Content.ToString() == "Done" ? "Do you want to close ?" : "Do you want to cancel the installation ?";
                var IsOk = DialogUtility.ShowMessageBoxModel(false, message,false, this);
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
                    StepNext();
                }
            });
        }
        private async Task CreateRegistry()
        {
            await Task.Run(() =>
            {
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
                            var productVersion = GetProductVersion(versionInfo.ProductVersion.ToString());
                            key.SetValue("DisplayName", Constants.ApplicationName);
                            key.SetValue("version", productVersion);
                            key.SetValue("Publisher", "Globussoft");
                            key.SetValue("EstimatedSize", (int)(folderSizeInBytes / 1024), RegistryValueKind.DWord);
                            key.SetValue("DisplayIcon", exe);
                            key.SetValue("DisplayVersion", productVersion);
                            key.SetValue("Contact", "https://socinator.com/contact-us/");
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
                        var iconPath = Path.Combine(DefaultPath, Constants.IconFileName);
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

                //using var shortcut1 = new WindowsShortcut
                //{
                //    Path = exePath,
                //    Description = Constants.ShortCutDescription,
                //    IconLocation = iconPath,
                //    WorkingDirectory = Path.GetDirectoryName(exePath)
                //};
                //shortcut1.Save(shortcutName);
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
    }
}
