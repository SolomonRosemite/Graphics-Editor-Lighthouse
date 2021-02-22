using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LighthouseLibrary.Models;
using LighthouseLibrary.Models.Metadata;

namespace Lighthouse.Windows.Editor.Pages
{
    /// <summary>
    /// Interaction logic for TransformPage.xaml
    /// </summary>
    public partial class TransformPage : Page
    {
        private readonly Regex regex = new Regex("^[0-9]+$");
        private readonly EditorWindow editorWindow;

        private Layer LastSelectedLayer { get; set; }


        private byte layerOpacity = 100;
        private byte LayerOpacity
        {
            get => layerOpacity;
            set
            {
                LastSelectedLayer.Metadata.Transform.Opacity = value;
                layerOpacity = value;
            }
        }

        private bool skipNextChange;
        private bool isInitialized;
        private bool isChained;

        public TransformPage(EditorWindow editorWindow)
        {
            InitializeComponent();

            this.editorWindow = editorWindow;

            Loaded += OnLoaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            isInitialized = false;
            LastSelectedLayer = editorWindow.LastSelectedLayer;

            // LastSelectedLayer = new Layer(new Bitmap(1, 1), 21, "Test", "Test", new LayerMetadata(1, 1));

            PositionXTextBox.Text = LastSelectedLayer.Metadata.Transform.XPosition.ToString();
            PositionYTextBox.Text = LastSelectedLayer.Metadata.Transform.YPosition.ToString();
            HeightTextBox.Text = LastSelectedLayer.Metadata.Transform.Height.ToString();
            WidthTextBox.Text = LastSelectedLayer.Metadata.Transform.Width.ToString();
            LayerOpacity = LastSelectedLayer.Metadata.Transform.Opacity;

            isInitialized = true;
        }

        private void OnClickRotateLeft(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnClickRotateRight(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnChainClick(object sender, RoutedEventArgs e)
        {
            if (isChained)
            {
                IsChainedImg.Opacity = 0;
                IsNotChainedImg.Opacity = 1;
            }
            else
            {
                IsChainedImg.Opacity = 1;
                IsNotChainedImg.Opacity = 0;
            }

            isChained = !isChained;
        }

        private void OnPositionXChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            int value = int.Parse(item.Text);

            LastSelectedLayer.Metadata.Transform.XPosition = value;

            editorWindow.Render();
        }

        private void OnPositionYChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            int value = int.Parse(item.Text);

            LastSelectedLayer.Metadata.Transform.YPosition = value;

            editorWindow.Render();
        }

        private void OnWidthChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            if (skipNextChange)
            {
                skipNextChange = false;
                return;
            }

            int value = int.Parse(item.Text);

            if (value == 0) return;

            var result = LastSelectedLayer.Metadata.Transform.SetWidth(value, isChained);

            if (isChained)
                skipNextChange = true;

            HeightTextBox.Text = result.Height.ToString();

            editorWindow.Render();
        }

        private void OnHeightChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            if (skipNextChange)
            {
                skipNextChange = false;
                return;
            }

            int value = int.Parse(item.Text);

            if (value == 0) return;

            var result = LastSelectedLayer.Metadata.Transform.SetHeight(value, isChained);
            if (isChained)
                skipNextChange = true;

            WidthTextBox.Text = result.Width.ToString();

            editorWindow.Render();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumbersOnly(e.Text);
        }

        private bool IsNumbersOnly(string text)
        {
            return regex.IsMatch(text) && text.Trim().Length != 0;
        }
    }
}
