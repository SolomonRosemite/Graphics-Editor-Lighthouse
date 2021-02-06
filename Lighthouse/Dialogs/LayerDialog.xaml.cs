using LighthouseLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lighthouse.Dialogs
{
    /// <summary>
    /// Interaction logic for LayerDialog.xaml
    /// </summary>
    public partial class LayerDialog : Window
    {
        private static readonly LayerSettings layerSettings = new LayerSettings();
        private static bool complete = false;

        public static async Task<LayerSettings> Open(Layer layer)
        {
            layerSettings.LayerName = (string)layer.LayerName.Clone();

            var dialog = new LayerDialog();
            dialog.Show();

            while(!complete)
                await Task.Delay(100);

            complete = false;

            return layerSettings;
        }

        private LayerDialog()
        {
            InitializeComponent();
            DataContext = layerSettings;
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            layerSettings.Save = true;
            complete = true;
            Close();
        }

        private void OnDiscardClick(object sender, RoutedEventArgs e)
        {
            layerSettings.Save = false;
            complete = true;
            Close();
        }

        public class LayerSettings
        {
            public bool Save { get; set; }
            public string LayerName { get; set; }
        }
    }
}
