using System.Windows;
using System;
using System.Windows.Controls;
using LighthouseLibrary.Models;
using LighthouseLibrary.Services;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using Lighthouse.Helpers;

namespace Lighthouse.Windows
{
    public partial class ExportWindow : Window
    {
        private readonly Project project;

        public ExportWindow(Project project)
        {
            InitializeComponent();
            this.project = project;

            ImageView.Source = Helper.ToBitmapSource(project.RenderProject());
        }

        private void OnExportAsPNG(object sender, RoutedEventArgs e)
        {
            ExportDialog("png", "PNG", ExportType.Png);
        }

        private void OnExportAsJPEG(object sender, RoutedEventArgs e)
        {
            ExportDialog("jpeg", "JPEG", ExportType.Jpeg);
        }

        private void ExportDialog(string defaultExt, string filter, ExportType type)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = project.ProjectName, DefaultExt = $".{defaultExt}", Filter = $"{filter} (.{defaultExt})|*.{defaultExt}"
            };

            var result = dlg.ShowDialog();

            if (result != true) return;

            // Save document
            string filename = dlg.FileName;
            ExportService.ExportImage(project, type, filename);
            this.Hide();
        }
    }
}