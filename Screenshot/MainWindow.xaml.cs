using System;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.Drawing;

namespace Screenshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string savePath;
        private const int SECONDS_IN_MINUTE = 60;
        private WindowState previousWindowState;
        public MainWindow()
        {
            savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            InitializeComponent();
            Directory.CreateDirectory(savePath);
        }
        private void NumericInputOnly(object sender, TextCompositionEventArgs e)
        {
            foreach (var symbol in e.Text)
            {
                if (!(char.IsDigit(symbol)))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
        private void TakeScreenshot(object state)
        {
            var screenLeft = (int)SystemParameters.VirtualScreenLeft;
            var screenTop = (int)SystemParameters.VirtualScreenTop;
            var screenWidth = (int)SystemParameters.VirtualScreenWidth;
            var screenHeight = (int)SystemParameters.VirtualScreenHeight;
            
            using (var bitmap = new Bitmap(screenWidth, screenHeight))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var fileName = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                graphics.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bitmap.Size);
                bitmap.Save(@$"{savePath}\{fileName}");
            }
        }

        private void SaveScreenshots(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Сохранено!");
            if (int.TryParse(inputPeriod.Text, out int period))
            {
                var periodInSeconds = period * SECONDS_IN_MINUTE;
                var random = new Random();
                var dueTime = random.Next(0, periodInSeconds);
                var Timer = new System.Threading.Timer(TakeScreenshot, null, TimeSpan.FromSeconds(dueTime), TimeSpan.FromMinutes(period));
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (WindowState != WindowState.Normal)
            {
                previousWindowState = WindowState;
            }
            else
            {
                Hide();
                taskBar.Visibility = Visibility.Visible;
            } 
            
        }

        private void TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = previousWindowState;
            taskBar.Visibility = Visibility.Hidden;
        }
    }
}
