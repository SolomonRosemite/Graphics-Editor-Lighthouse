using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LighthouseLibrary.Models;
using LighthouseLibrary.Services;
using Point = System.Windows.Point;

namespace Lighthouse.Windows
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly EditorState state;
        private Project project;

        private Point dragStartPoint;
        private bool isMoveAction;
        private bool isUndoAction;

        public EditorWindow(Project project)
        {
            state = new EditorState();
            this.project = project;

            InitializeComponent();
            RegisterEvents();

            InitLayers();

            Render();
        }

        private void InitLayers()
        {
            if (project.Layers.Count == 0) return;

            LayerNameLabel.Content = project.Layers[0].LayerName;

            listBox.DisplayMemberPath = "LayerName";
            listBox.ItemsSource = project.Layers;

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
            project.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        private void OnRedoClick(object sender, RoutedEventArgs e)
        {
            var res = state.Redo();
            Console.WriteLine(res.Successful);
            if (res.Successful)
            {
                project = res.ProjectState;
                listBox.ItemsSource = project.Layers;
                Render(false);
            }
            else
            {
                // Todo: Handle...
            }
        }

        private void OnUndoClick(object sender, RoutedEventArgs e)
        {
            var res = state.Undo();
            Console.WriteLine(res.Successful);
            if (res.Successful)
            {
                isUndoAction = true;
                project.Layers.Clear();

                project = res.ProjectState;
                listBox.ItemsSource = project.Layers;

                Render(false);
            }
            else
            {
                // Todo: Handle...
            }
        }

        private void TestRotateImage(object sender, RoutedEventArgs e)
        {
            project.Layers[0].RotateImageTest();
            Console.WriteLine(project.Layers[0].LayerName);
            Render();
        }

        private void Render(bool updateState = true)
        {
            if (updateState)
                state.UpdateState(project);

            var res = project.RenderProject();

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
                Layer layer = ImportService.LoadImportedImageToLayer(filename, $"Layer {project.Layers.Count + 1}");
                
                project.Layers.Insert(0, layer);
            }
        }

        private void OnLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (isMoveAction)
            {
                isMoveAction = false;
                return;
            }
            if (isUndoAction)
            {
                isUndoAction = false;
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

        private void OnExportImage(object sender, RoutedEventArgs e) => new ExportWindow(project).Show();

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
            Point point = e.GetPosition(null);
            Vector diff = dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {

                var lbi = FindVisualParent<ListBoxItem>((DependencyObject)e.OriginalSource);
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
