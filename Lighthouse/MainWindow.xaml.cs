﻿using Lighthouse.Pages;
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Threading;
using Lighthouse.Services;
using Newtonsoft.Json;
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

        private enum DropColorState
        {
            Increase, Decrease
        }

        public MainWindow()
        {
            Console.WriteLine("Cool");
            Console.WriteLine("Cool");
            Console.WriteLine("Cool");
            Console.WriteLine("Cool");
            
            Thread.Sleep(1500);

            InitializeComponent();

            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            //new EditorWindow().Show();
            //this.Hide();
        }

        private void HandleIncomingFile(string filePath)
        {
            Console.WriteLine("Path to file: " + filePath);
            try
            {
                Bitmap image = new Bitmap(filePath);

                new EditorWindow(image).Show();
                this.Hide();
                // Console.WriteLine(data[0]);

                // Image image1 = System.Drawing..FromFile("c:\\FakePhoto1.jpg");
                // if (filePath.EndsWith(".lh"))
                // {
                //     ImportService.LoadProject(filePath);
                // }
                // else
                // {
                //     ImportService.LoadGetImportedImage(filePath);
                // }
            }
            catch (Exception e)
            {
                // Todo: Handle
                Console.WriteLine(e);
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
            // Todo...
            throw new NotImplementedException();
        }

        private void OnImportProject(object sender, RoutedEventArgs e)
        {
            // Todo...
            throw new NotImplementedException();
        }

        private void OnImportImage(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
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
            var value = (DropArea.BorderBrush as SolidColorBrush).Color.A;
            const byte increment = 5;

            if (state == DropColorState.Decrease)
            {
                if (value < 10)
                    state = DropColorState.Increase;

                value -= increment;
            }
            else if (state == DropColorState.Increase)
            {
                if (value > 245)
                    state = DropColorState.Decrease;

                value += increment;
            }

            DropArea.BorderBrush = new SolidColorBrush(Color.FromArgb(value, 255, 255, 255));
        }

        #region WindowEvent

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

        #endregion
    }
}
