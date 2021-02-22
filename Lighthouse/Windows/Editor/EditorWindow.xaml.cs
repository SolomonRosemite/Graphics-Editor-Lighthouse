using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Lighthouse.Dialogs;
using Lighthouse.Helpers;
using Lighthouse.Windows;
using Lighthouse.Windows.Editor.Pages;
using Lighthouse.Windows.Editor.Pages.LayersListViewPage;
using Lighthouse.Windows.Editor.Structs;
using LighthouseLibrary.Models;
using LighthouseLibrary.Services;

namespace Lighthouse.Windows.Editor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly EditorState editorState;
        private Project project;

        private int currentProjectStateId;
        private bool ignoreNextRender;

        private readonly string loadedProjectLocation;

        public Layer LastSelectedLayer
        {
            get => lastSelectedLayer;
            private set
            {
                if (IsInitialized)
                    LayerNameLabel.Content = value.LayerName;

                lastSelectedLayer = value;
            }
        }

        private EditorPages pages;
        private Layer lastSelectedLayer;

        public EditorWindow(Project project, string loadedProjectLocation)
        {
            this.loadedProjectLocation = loadedProjectLocation;
            LastSelectedLayer = project.Layers[0];
            editorState = new EditorState();
            this.project = project;

            InitializeComponent();
            InitializePages();
            RegisterEvents();
            InitializeApp();

            PlayAnimation(2000);

            Render();
        }

        private void InitializePages()
        {
            pages.ColorGradingPage = new ColorGradingPage();
            pages.BackgroundPage = new BackgroundPage();
            pages.FiltersPage = new FiltersPage();
            pages.ProjectSettings = new ProjectSettings();
            pages.TransformPage = new TransformPage(this);
            InitializeLayersListViewPage();

            void InitializeLayersListViewPage()
            {
                pages.ListViewPage = new LayersListViewPage(this, project, OnLayerListViewSelectionChanged);
                CurrentPage.Content = pages.ListViewPage;
            }
        }

        private void InitializeApp()
        {
            this.LayerNameLabel.Content = LastSelectedLayer.LayerName;
            this.Title = $"Lighthouse - {project.ProjectName}";
        }

        private void OnLayerListViewSelectionChanged(object _, ListBox listBox)
        {
            try
            {
                if (listBox.SelectedItem == null || !(listBox.SelectedItem is Layer item)) return;

                LastSelectedLayer = item;
            }
            catch { }
        }

        private void RegisterEvents()
        {
            project.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        private void OnRedoClick(object sender, RoutedEventArgs e) => HandleRedoUndoAction(editorState.Redo(currentProjectStateId));

        private void OnUndoClick(object sender, RoutedEventArgs e) => HandleRedoUndoAction(editorState.Undo(currentProjectStateId));

        public void HandleRedoUndoAction(ActionResponse res)
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

                var result = project.Layers.FirstOrDefault(l => l.Id == LastSelectedLayer.Id)
                             ?? project.Layers[^1];

                LastSelectedLayer = result;

                pages.TransformPage.OnLoaded(null, null);

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

        public void Render(bool updateSnapshot = true)
        {
            if (updateSnapshot)
                this.currentProjectStateId = editorState.AddNewSnapshot(project);

            var res = project.RenderProject();

            var bitmapImage = Helper.ToBitmapSource(res);
            ImageView.Source = bitmapImage;
        }

        public void OnImportImage(object sender, RoutedEventArgs e)
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
                Layer layer = ImportService.LoadImportedImageToLayer(filename, $"Layer{project.Layers.Count + 1}", project);

                project.Layers.Insert(0, layer);

                LastSelectedLayer = layer;

                pages.TransformPage.OnLoaded(null, null);
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

        private void OnSaveProject(object _, RoutedEventArgs e)
        {
            // Todo: Save the Project...
            // If the Project name contains "unnamed" OpenFileDialog to find a place to save it.
            // Else just save at where it was loaded from.
            if (loadedProjectLocation == null)
            {
                // This is a newly created Project.
                return;
            }

            // This is not a new Project... thus we save at the same Location (same file).
            // Todo: Implement...
        }

        private async void OnEditLayerName(object sender, RoutedEventArgs e)
        {
            Layer layer = LastSelectedLayer;
            var result = await LayerDialog.Open(layer);

            if (!result.Save || result.LayerName.Length > 50 || result.LayerName.Trim().Length == 0) return;

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
                throw new Exception("Couldn't find Layer to match Id with.");

            // Remove Item from list
            project.Layers.RemoveAt(index);

            // Update that layer's name
            layer.LayerName = result.LayerName.Trim();

            ignoreNextRender = true;

            // Put back the same layer at the same index...
            project.Layers.Insert(index, layer);

            LastSelectedLayer = layer;

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

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            // Todo: Don't exit if there are unsaved changes...
            Application.Current.Shutdown();
        }

        private void OnColorGradingClick(object sender, RoutedEventArgs e)
        {
            Fade(pages.ColorGradingPage);
        }

        private void OnFiltersClick(object sender, RoutedEventArgs e)
        {
            Fade(pages.FiltersPage);
        }

        private void OnLayersClick(object sender, RoutedEventArgs e)
        {
            Fade(pages.ListViewPage);
        }

        private void OnTransformClick(object sender, RoutedEventArgs e)
        {
            Fade(pages.TransformPage);
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            Fade(pages.ProjectSettings);
        }

        private void OnBackgroundClick(object sender, RoutedEventArgs e)
        {
            Fade(pages.BackgroundPage);
        }

        private void Fade(Page page)
        {
            if (Equals(CurrentPage.Content, page)) return;
            PlayAnimation();

            CurrentPage.Content = page;
        }

        private void PlayAnimation(int milliseconds = 1000)
        {
            AnimationRectangle.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(milliseconds))));
        }
    }
}
