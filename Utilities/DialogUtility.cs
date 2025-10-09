using AltraVeraHostInstaller.Views;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AltraVeraHostInstaller.Utilities
{
    public class DialogUtility
    {
        public static bool ShowMessageBoxModel(bool isyes = false, string msg = "", bool isAsync = false, Window UI = null)
        {
            bool IsOkClicked = false;
            CustomMessageBox msgBox = new CustomMessageBox(isyes, msg);

            RectangleGeometry rect = new RectangleGeometry
            {
                Rect = new Rect(0, 0, 300, 140),
                RadiusX = 10,
                RadiusY = 10
            };

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
                //Clip = rect,
                Owner = MainWindow.GetInstance
            };

            msgBox.MyWindow = window;
            window.Content = msgBox;

            BlurEffect blurEffect = null;

            if (UI != null)
            {
                // Apply blur effect to parent window
                blurEffect = new BlurEffect
                {
                    Radius = 8, // Adjust blur intensity (range 0–100)
                    KernelType = KernelType.Gaussian
                };
                UI.Effect = blurEffect;
            }

            window.Closing += (s, e) =>
            {
                IsOkClicked = msgBox.IsOk;
                // Remove blur effect when closing
                if (UI != null)
                    UI.Effect = null;
            };

            window.ShowDialog();
            window.Activate();

            return IsOkClicked;
        }
    }
}
