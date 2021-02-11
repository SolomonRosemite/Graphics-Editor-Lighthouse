using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Lighthouse.Helpers;
using Lighthouse.Windows;
using Lighthouse.Windows.Editor;
using LighthouseLibrary.Models;
using LighthouseLibrary.Services;
using Color = System.Windows.Media.Color;

namespace Lighthouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DropColorState state = DropColorState.Increase;
        private readonly WindowDoubleClick doubleClick;

        private enum DropColorState
        {
            Increase, Decrease
        }

        public MainWindow()
        {
            // Thread.Sleep(1500);

            doubleClick = new WindowDoubleClick();
            InitializeComponent();

            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
        }

        private void HandleIncomingFile(string filePath)
        {
            Console.WriteLine("Path to file: " + filePath);

            try
            {
                bool isProjectType = filePath.EndsWith(".lh");
                string loadedProjectLocation = !isProjectType ? null : filePath;

                Project project = isProjectType
                    ? ImportService.LoadImportedProject(filePath)
                    : ImportService.LoadImportedImage(filePath);

                new EditorWindow(project, loadedProjectLocation).Show();
                this.Hide();
            }
            catch (Exception e)
            {
				// Todo: Handle if file is not a type image.
                OnDragLeave(null, null);

                Console.WriteLine(e);
                throw;
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            state = DropColorState.Increase;
            dispatcherTimer.Start();
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files == null || files.Length == 0)
                {
                    OnDragLeave(null, null);
                    return;
                }

                HandleIncomingFile(files[0]);
                return;
            }

            OnDragLeave(null, null);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            dispatcherTimer.Stop();
        }

        private void OnClickNewProject(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnImportProject(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnImportImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|All files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                HandleIncomingFile(filename);
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var value = ((SolidColorBrush) DropArea.BorderBrush).Color.A;
            const byte increment = 5;

            switch (state)
            {
                case DropColorState.Decrease:
                {
                    if (value < 10)
                        state = DropColorState.Increase;

                    value -= increment;
                    break;
                }
                case DropColorState.Increase:
                {
                    if (value > 245)
                        state = DropColorState.Decrease;

                    value += increment;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(value, 255, 255, 255));
        }

        #region WindowEvents

        private void WindowClick(object sender, MouseButtonEventArgs e)
        {
            var wasDoubleClick = doubleClick.OnClickClick();

            if (wasDoubleClick)
                OnMaximizedClick(null, null);

            try { DragMove(); } catch { }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
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
            }
        }

        private void OnMinimizedClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        #endregion
    }
}
