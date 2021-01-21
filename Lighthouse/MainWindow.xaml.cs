using Lighthouse.Pages;
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
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DropColorState state = DropColorState.increase;

        enum DropColorState
        {
            increase, decrease, none
        }

        public MainWindow()
        {
            // Thread.Sleep(1500);

            InitializeComponent();

            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            //new EditorWindow().Show();
            //this.Hide();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            state = DropColorState.increase;
            dispatcherTimer.Start();
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine(e);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            dispatcherTimer.Stop();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var value = (DropArea.BorderBrush as SolidColorBrush).Color.A;
            const byte increment = 5;

            if (state == DropColorState.decrease)
            {
                if (value < 10)
                    state = DropColorState.increase;

                value -= increment;
            } 
            else if (state == DropColorState.increase)
            {
                if (value > 245)
                    state = DropColorState.decrease;

                value += increment;
            }

            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(value, 255, 255, 255));
        }

        private void WindowClick(object sender, MouseButtonEventArgs e)
        {
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
            }
        }

        private void OnMinimizedClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}
