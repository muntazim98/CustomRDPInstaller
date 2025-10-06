using CustomRDPInstaller.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CustomRDPInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private string _defaultPath = Constants.GetDefaultIntallationPath;
        public string DefaultPath
        {
            get => _defaultPath;
            set
            {
                _defaultPath = value;
                OnPropertyChanged(nameof(DefaultPath));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
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

        private void Browsebtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DiskCostBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
