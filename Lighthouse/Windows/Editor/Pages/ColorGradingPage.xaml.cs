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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Lighthouse.Helpers;
using LighthouseLibrary.Models;

namespace Lighthouse.Windows.Editor.Pages
{
    /// <summary>
    /// Interaction logic for ColorGradingPage.xaml
    /// </summary>
    public partial class ColorGradingPage : Page
    {
        private readonly DispatcherTimer dispatcherTimerBrightness;
        private readonly EditorWindow editorWindow;

        private bool initialRun = true;
        private bool InitialRun
        {
            get => initialRun;
            set
            {
                if (initialRun == false)
                    return;

                initialRun = value;
            }
        }

        private float layerBrightness = 1;
        private float LayerBrightness
        {
            get => layerBrightness;
            set
            {
                LastSelectedLayer.Metadata.Color.Brightness = value;
                layerBrightness = value;

                dispatcherTimerBrightness.Stop();
                dispatcherTimerBrightness.Start();
            }
        }

        private Layer LastSelectedLayer { get; set; }

        private bool skipNextChange;
        private bool isInitialized;
        private bool isChained;
        private bool skipNextRender;

        public ColorGradingPage(EditorWindow editorWindow)
        {
            dispatcherTimerBrightness = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(80) };
            dispatcherTimerBrightness.Tick += OnTickBrightness;

            InitializeComponent();

            this.editorWindow = editorWindow;

            Loaded += OnLoaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            isInitialized = false;
            skipNextRender = true;

            LastSelectedLayer = editorWindow.LastSelectedLayer;

            BrightnessTextBox.Text = (LastSelectedLayer.Metadata.Color.Brightness * 100).ToString();

            LayerBrightness = LastSelectedLayer.Metadata.Color.Brightness;
            BrightnessSlider.Value = LayerBrightness * 100;

            if (InitialRun)
                BrightnessSlider.ValueChanged += (o, args) => LayerBrightness = (float) (args.NewValue / 100);

            isInitialized = true;
            InitialRun = false;
        }

        private void OnTickBrightness(object _, EventArgs e)
        {
            if (skipNextRender)
                skipNextRender = false;
            else if (isInitialized)
                editorWindow.Render();

            dispatcherTimerBrightness.Stop();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Helper.IsNumbersOnly(e.Text);
        }
    }
}
