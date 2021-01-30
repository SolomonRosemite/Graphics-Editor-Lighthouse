using LighthouseLibrary.Services;
using LighthouseLibrary.Models;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lighthouse.Windows;
using System.Windows;
using System;

namespace Lighthouse.Pages
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private System.Windows.Point dragStartPoint;

        private readonly Project Project;

        private bool isMoveAction = false;

        public EditorWindow(Project project)
        {
            Project = project;

            InitializeComponent();
            RegisterEvents();

            InitLayers();

            Render();
        }

        private void InitLayers()
        {
            if (Project.Layers.Count == 0) return;

            LayerNameLabel.Content = Project.Layers[0].LayerName;

            listBox.DisplayMemberPath = "LayerName";
            listBox.ItemsSource = Project.Layers;

            listBox.PreviewMouseMove += ListBox_PreviewMouseMove;

            var style = new Style(typeof(ListBoxItem));
            style.Setters.Add(new Setter(AllowDropProperty, true));
            style.Setters.Add(new EventSetter(
                PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(ListBoxItem_PreviewMouseLeftButtonDown)
            ));
            style.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(ListBoxItem_Drop)));
            listBox.ItemContainerStyle = style;
        }

        private void RegisterEvents()
        {
            listBox.SelectionChanged += ListBox_SelectionChanged;
            Project.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        private void Render()
        {
            var res = Project.RenderProject();

            var bitmapImage = Helper.BitmapToImageSource(res);
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
                Layer layer = ImportService.LoadImportedImageToLayer(filename, $"Layer {Project.Layers.Count + 1}");
                
                Project.Layers.Insert(0, layer);
            }
        }

        private void OnLayerCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (isMoveAction)
            {
                isMoveAction = false;
                return;
            }

            Render();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBox.SelectedItem == null || !(listBox.SelectedItem is Layer item)) return;

                LayerNameLabel.Content = item.LayerName;
            }
            catch { }
        }

        private void OnExportImage(object sender, RoutedEventArgs e) => new ExportWindow(Project).Show();

        #region

        private T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;
            return FindVisualParent<T>(parentObject);
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(null);
            Vector diff = dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {

                var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                if (lbi != null)
                {
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                var source = e.Data.GetData(typeof(Layer)) as Layer;
                var target = item.DataContext as Layer;

                int sourceIndex = listBox.Items.IndexOf(source);
                int targetIndex = listBox.Items.IndexOf(target);

                Move(source, sourceIndex, targetIndex);
            }
        }

        private void Move(Layer source, int sourceIndex, int targetIndex)
        {
            isMoveAction = true;
            
            if (sourceIndex < targetIndex)
            {
                Project.Layers.Insert(targetIndex + 1, source);
                Project.Layers.RemoveAt(sourceIndex);
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (Project.Layers.Count + 1 > removeIndex)
                {
                    Project.Layers.Insert(targetIndex, source);
                    Project.Layers.RemoveAt(removeIndex);
                }
            }
        }

        #endregion

        #region

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMinimizedClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        #endregion
    }
}
