using CustomRDPInstaller.Views;
using System.Windows;
using System.Windows.Media;

namespace CustomRDPInstaller.Utilities
{
    public class DialogUtility
    {
        public static bool ShowMessageBoxModel(bool isyes = false, string msg = "", bool isAsync = false, Window UI = null)
        {
            var IsOkClicked = false;
            CustomMessageBox msgBox = new CustomMessageBox(isyes, msg);
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0, 0, 300, 140);
            rect.RadiusX = 10;
            rect.RadiusY = 10;
            Window window = new Window
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                BorderThickness = new Thickness(0),
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                Height = 140,
                Width = 300,
                Background = Brushes.Transparent,
                Clip = rect
            };
            window.Owner = MainWindow.GetInstance;
            msgBox.MyWindow = window;
            window.Content = msgBox;
            if (UI != null)
                UI.Opacity = Constants.UIOpacityDisable;
            window.Closing += (s, e) =>
            {
                IsOkClicked = msgBox.IsOk;
                if (UI != null)
                    UI.Opacity = Constants.UIOpacityEnable;
            };
            window.ShowDialog();
            window.Activate();
            return IsOkClicked;
        }
    }
}
