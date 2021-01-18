using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lighthouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Thread.Sleep(1500);
            InitializeComponent();
        }

        private void WindowClick(object sender, MouseButtonEventArgs e)
        {
            // Todo: Maximize or and Minimize Window on Double-click

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
    }
}
