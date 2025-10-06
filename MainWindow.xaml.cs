using CustomRDPInstaller.Utilities;
using System;
using System.ComponentModel;
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
            StepCount -= 1;
            StepPrevious();
            if (NegativeButton.Content.ToString() == "Done" || NegativeButton.Content.ToString() == "Cancel")
            {
                string message = NegativeButton.Content.ToString() == "Done" ? "Do you want to close ?" : "Do you want to cancel ?";
                var IsOk = DialogUtility.ShowMessageBoxModel(false, message,false, this);
                if(IsOk)
                    this.Close();
            }
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
