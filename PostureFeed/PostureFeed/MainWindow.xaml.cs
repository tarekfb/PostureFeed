using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PostureFeed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LocalWebCamsCollection;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MyWindowViewModel();
            Loaded += MainWindow_Loaded;
            InitHotKeys();
        }

        void InitHotKeys()
        {
            var newCmd = new RoutedCommand();
            _ = newCmd.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newCmd, ResetPos));
        }

        private void ResetPos(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Top = "200";
            Properties.Settings.Default.Left = "200";
        }

        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = new BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Bmp);
                    _ = ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }
                bi.Freeze();
                _ = Dispatcher.BeginInvoke(new ThreadStart(delegate { FrameHolder.Source = bi; }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LocalWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (LocalWebCamsCollection.Count == 0)
            {
                //Image img = Image.FromFile("camera-not-available.png");

                //Graphics graphics = Graphics.FromImage(img);
                //System.Windows.Media.ImageSource imageSource = imageSource.
                //NotAvailable.Source = graphics;
                //NotAvailable.Source = new BitmapImage(new Uri("pack://application:PostureFeed/AssemblyName;component//camera-not-available"));
            }
            LocalWebCam = new VideoCaptureDevice(LocalWebCamsCollection[0].MonikerString);
            LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

            LocalWebCam.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            LocalWebCam.SignalToStop();
            LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame); // as sugested
            LocalWebCam = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
        }

        public class BooleanToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                => (bool)value ? Visibility.Visible : Visibility.Collapsed;

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    }
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ResetPos = new(
                "Resets window positioning to default",
                "Reset positioning",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.R, ModifierKeys.Control)
                }
            );
    }
}
