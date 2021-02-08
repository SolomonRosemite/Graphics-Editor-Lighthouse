using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lighthouse.Dialogs;
using Lighthouse.Helpers;
using Lighthouse.Windows;
using Lighthouse.Windows.Editor.Pages.LayersListViewPage;
using Lighthouse.Windows.Editor.Structs;
using LighthouseLibrary.Models;
using LighthouseLibrary.Services;
using Point = System.Windows.Point;

namespace Lighthouse.Windows.Editor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly WindowDoubleClick doubleClick;

        private readonly EditorState editorState;
        private Project project;

        private int currentProjectStateId;
        private bool ignoreNextRender;

        private Layer lastSelectedLayer;

        private EditorPages pages;

        public EditorWindow(Project project)
        {
            doubleClick = new WindowDoubleClick();

            editorState = new EditorState();
            this.project = project;

            InitializeComponent();
            RegisterEvents();

            InitializeLayersListViewPage();

            lastSelectedLayer = project.Layers[0];

            Render();
        }

        private void InitializeLayersListViewPage()
        {
            pages.ListViewPage = new LayersListViewPage(this, project, OnLayerListViewSelectionChanged);

            LayerNameLabel.Content = project.Layers[0].LayerName;

            CurrentPage.Content = pages.ListViewPage;
        }

        private void OnLayerListViewSelectionChanged(object _, ListBox listBox)
        {
            try
            {
                if (listBox.SelectedItem == null || !(listBox.SelectedItem is Layer item)) return;

                LayerNameLabel.Content = item.LayerName;
                lastSelectedLayer = item;
            }
            catch { }
        }

        private void RegisterEvents()
        {
            project.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        private void OnRedoClick(object sender, RoutedEventArgs e) => HandleRedoUndoAction(editorState.Redo(currentProjectStateId));

        private void OnUndoClick(object sender, RoutedEventArgs e) => HandleRedoUndoAction(editorState.Undo(currentProjectStateId));

        private void HandleRedoUndoAction(ActionResponse res)
        {
            if (res.Successful)
            {
                project = res.ProjectState;
                currentProjectStateId = res.StateId;

                var fi = project.Layers.GetType().GetEventField("CollectionChanged");
                if (fi == null) return;
                fi.SetValue(project.Layers, null);

                pages.ListViewPage.listBox.ItemsSource = project.Layers;

                project.Layers.CollectionChanged += OnLayerCollectionChanged;

                Render(false);
            }
            else
            {
                Console.WriteLine("Can't go further...");
                // Todo: Maybe show a popup or some...
                // User cant go further back or forth.
                // Depending if the Action was Undo or Redo.
            }
        }

        private void TestRotateImage(object sender, RoutedEventArgs e)
        {
            RotateFlipType type = project.Layers[0].Metadata.RotationType;
            switch (type)
            {
                case RotateFlipType.RotateNoneFlipNone:
                    type = RotateFlipType.Rotate90FlipNone;
                    break;
                case RotateFlipType.Rotate90FlipNone:
                    type = RotateFlipType.Rotate180FlipNone;
                    break;
                case RotateFlipType.Rotate180FlipNone:
                    type = RotateFlipType.Rotate270FlipNone;
                    break;
                case RotateFlipType.Rotate270FlipNone:
                    type = RotateFlipType.RotateNoneFlipNone;
                    break;
            }

            project.Layers[0].RotateImageTest(type);
            Render();
        }

        private void Render(bool updateSnapshot = true)
        {
            if (updateSnapshot)
                this.currentProjectStateId = editorState.AddNewSnapshot(project);

            var res = project.RenderProject();

            var bitmapImage = Helper.ToBitmapSource(res);
            ImageView.Source = bitmapImage;
        }

        private void OnImportImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|All files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                Layer layer = ImportService.LoadImportedImageToLayer(filename, $"Layer {project.Layers.Count + 1}", project);

                project.Layers.Insert(0, layer);
            }
        }

        private void OnLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (ignoreNextRender)
            {
                ignoreNextRender = false;
                return;
            }

            Render();
        }

        private void OnExportImage(object sender, RoutedEventArgs e) => new ExportWindow(project).Show();

        private async void OnEditLayerName(object sender, RoutedEventArgs e)
        {
            Layer layer = lastSelectedLayer;
            var result = await LayerDialog.Open(layer);

            if (!result.Save) return;

            ignoreNextRender = true;

            int index = -1;
            for (int i = 0; i < project.Layers.Count; i++)
            {
                if (project.Layers[i].Id == layer.Id)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                throw new Exception("Index was -1");


            // Remove Item from list
            project.Layers.RemoveAt(index);

            // Update that layer's name
            layer.LayerName = result.LayerName;

            ignoreNextRender = true;

            // Put back the same layer at the same index...
            project.Layers.Insert(index, layer);

            LayerNameLabel.Content = layer.LayerName;

            currentProjectStateId = editorState.AddNewSnapshot(project);
        }

        #region ListViewPage

        public void Move(Layer source, int sourceIndex, int targetIndex)
        {
            ignoreNextRender = true;

            if (sourceIndex < targetIndex)
            {
                project.Layers.Insert(targetIndex + 1, source);
                project.Layers.RemoveAt(sourceIndex);
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (project.Layers.Count + 1 > removeIndex)
                {
                    project.Layers.Insert(targetIndex, source);
                    project.Layers.RemoveAt(removeIndex);
                }
            }
        }

        #endregion

        #region WindowClick Events

        private void WindowClick(object sender, MouseButtonEventArgs e)
        {
            var wasDoubleClick = doubleClick.OnClickClick();

            if (wasDoubleClick)
                OnMaximizedClick(null, null);

            try { DragMove(); } catch { }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            // Todo: Don't exit if there are unsaved changes...
            Application.Current.Shutdown();
        }

        private void OnMaximizedClick(object sender, RoutedEventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMinimizedClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion
    }
}
