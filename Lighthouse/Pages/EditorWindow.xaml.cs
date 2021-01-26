using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using LighthouseLibrary.Models;
using LighthouseLibrary.Services;

namespace Lighthouse.Pages
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly ObservableCollection<Item> items = new ObservableCollection<Item>();

        private System.Windows.Point dragStartPoint;
        private readonly Bitmap bitmap;

        private Project Project;

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = memory;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            return image;
        }

        public EditorWindow(Project project)
        {
            Project = project;

            InitializeComponent();
            RegisterEvents();

            InitLayers(Project.Layers);

            bitmap = Project.Layers[0].RenderLayer();

            var bitmapImage = BitmapToImageSource(bitmap);
            ImageView.Source = bitmapImage;
        }

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

        private void InitLayers(List<Layer> layers)
        {
            if (layers.Count == 0) return;

            layers.ForEach(layer => items.Add(new Item(layer.LayerName, layer.Id)));
            LayerNameLabel.Content = layers[0].LayerName;

            listBox.DisplayMemberPath = "Name";
            listBox.ItemsSource = items;

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
            items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Todo...
            //switch (e.Action)
            //{
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
            //        break;
            //}
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Console.WriteLine(listBox.SelectedItem);

                if (listBox.SelectedItem == null || !(listBox.SelectedItem is Item item)) return;

                LayerNameLabel.Content = item.Name;
            }
            catch { }
        }

        #region

        public class Item
        {
            public string Name { get; set; }
            public int Id { get; }
            public Item(string name, int id)
            {
                Name = name;
                Id = id;
            }
        }

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
                var lb = sender as ListBox;
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
            if (sender is ListBoxItem)
            {
                var source = e.Data.GetData(typeof(Item)) as Item;
                var target = ((ListBoxItem)(sender)).DataContext as Item;

                int sourceIndex = listBox.Items.IndexOf(source);
                int targetIndex = listBox.Items.IndexOf(target);

                Move(source, sourceIndex, targetIndex);
            }
        }

        private void Move(Item source, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                items.Insert(targetIndex + 1, source);
                items.RemoveAt(sourceIndex);
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (items.Count + 1 > removeIndex)
                {
                    items.Insert(targetIndex, source);
                    items.RemoveAt(removeIndex);
                }
            }
        }

        #endregion
    }
}
