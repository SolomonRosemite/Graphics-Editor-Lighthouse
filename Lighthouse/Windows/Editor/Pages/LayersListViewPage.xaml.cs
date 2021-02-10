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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lighthouse.Windows.Editor.Pages.LayersListViewPage
{
    /// <summary>
    /// Interaction logic for LayersListViewPage.xaml
    /// </summary>
    public partial class LayersListViewPage : Page
    {
        private readonly Action<object, ListBox> onSelectionChanged;
        private readonly EditorWindow editorWindow;
        private readonly Project project;
        private Point dragStartPoint;

        public LayersListViewPage(
            EditorWindow editorWindow,
            Project project, 
            Action<object, ListBox> onSelectionChanged
            )
        {
            this.editorWindow = editorWindow;
            this.project = project;
            this.onSelectionChanged = onSelectionChanged;

            InitializeComponent();

            RegisterEvents();

            InitLayers();
        }

        private void RegisterEvents()
        {
            listBox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => onSelectionChanged(sender, listBox);

        private void InitLayers()
        {
            if (project.Layers.Count == 0) return;

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

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;
            return FindVisualParent<T>(parentObject);
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
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

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                var source = e.Data.GetData(typeof(Layer)) as Layer;
                var target = item.DataContext as Layer;

                int sourceIndex = listBox.Items.IndexOf(source);
                int targetIndex = listBox.Items.IndexOf(target);

                editorWindow.Move(source, sourceIndex, targetIndex);
            }
        }

        private void OnAddLayerClick(object sender, RoutedEventArgs e) => editorWindow.OnImportImage(sender, e);
    }
}
