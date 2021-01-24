using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Lighthouse.DataStructures;

namespace Lighthouse.Pages
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly Bitmap bitmap;
        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = memory;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            return image;
        }

        public EditorWindow(Project project)
        {
            InitializeComponent();

            bitmap = project.Layers[0].RenderLayer();

            var bitmapImage = BitmapToImageSource(bitmap);
            ImageView.Source = bitmapImage;
        }

        private void WindowClick(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, Color.Red);
                }
            }

            Console.WriteLine(bitmap.GetPixel(0, 0));
            ImageView.Source = BitmapToImageSource(bitmap);

            // Todo: Maximize or and Minimize Window on Double-click
            try { DragMove(); } catch { }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            // Todo: Don't exit if there are unsaved changes...
            Application.Current.Shutdown();
        }

        private void OnMaximizedClick(object sender, RoutedEventArgs e)
        {
            switch (Application.Current.MainWindow.WindowState)
            {
                case WindowState.Normal:
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMinimizedClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}
