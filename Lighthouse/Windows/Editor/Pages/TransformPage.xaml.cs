﻿using System;
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

namespace Lighthouse.Windows.Editor.Pages
{
    /// <summary>
    /// Interaction logic for TransformPage.xaml
    /// </summary>
    public partial class TransformPage : Page
    {
        private readonly Regex regex = new Regex("^[0-9]+$");
        private readonly EditorWindow editorWindow;
        private Layer lastSelectedLayer;

        private byte layerOpacity = 100;
        private bool isInitialized;
        private bool isChained;

        private byte LayerOpacity
        {
            get => layerOpacity;
            set
            {
                lastSelectedLayer.Metadata.Transform.Opacity = value;
                layerOpacity = value;
            }
        }

        public TransformPage(EditorWindow editorWindow)
        {
            Loaded += OnLoaded;

            InitializeComponent();

            this.editorWindow = editorWindow;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            lastSelectedLayer = editorWindow.LastSelectedLayer;

            PositionXTextBox.Text = lastSelectedLayer.Metadata.Transform.XPosition.ToString();
            PositionYTextBox.Text = lastSelectedLayer.Metadata.Transform.YPosition.ToString();
            HeightTextBox.Text = lastSelectedLayer.Metadata.Transform.Height.ToString();
            WidthTextBox.Text = lastSelectedLayer.Metadata.Transform.Width.ToString();
            Opacity = lastSelectedLayer.Metadata.Transform.Opacity;

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

            lastSelectedLayer.Metadata.Transform.XPosition = value;

            editorWindow.Render();
        }

        private void OnPositionYChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            int value = int.Parse(item.Text);

            lastSelectedLayer.Metadata.Transform.YPosition = value;

            editorWindow.Render();
        }

        private void OnWidthChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            int value = int.Parse(item.Text);

            if (isChained)
            {
                // lastSelectedLayer.Metadata.Transform.SetEqually();
            }
            else
            {
                lastSelectedLayer.Metadata.Transform.SetWidth(value);
            }

            editorWindow.Render();
        }

        private void OnHeightChange(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized || !(sender is TextBox item)) return;

            if (item.Text.Trim().Length == 0) return;

            int value = int.Parse(item.Text);

            if (isChained)
            {
                // lastSelectedLayer.Metadata.Transform.SetEqually();
            }
            else
            {
                lastSelectedLayer.Metadata.Transform.SetHeight(value);
            }

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
