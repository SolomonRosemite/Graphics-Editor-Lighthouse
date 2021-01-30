using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;

namespace LighthouseLibrary.Models
{
    public class Layer
    {
        public int Id { get; }
        public string LayerName { get; set; }

        private ObservableCollection<Filter> Filters { get; }
        private Bitmap PreviousRenderedBitmap { get; set; }
        private LayerState LayerState { get; set; }
        private Bitmap Bitmap { get; }

        public Layer(Bitmap bitmap, int id, string layerName)
        {
            Id = id;

            Bitmap = bitmap;
            LayerName = layerName;
            LayerState = LayerState.Updated;

            Filters = new ObservableCollection<Filter>();
            Filters.CollectionChanged += OnCollectionChanged;
        }

        //  ReSharper restore Unity.ExpensiveCode
        public Bitmap RenderLayer()
        {
            if (LayerState == LayerState.Unchanged)
                return PreviousRenderedBitmap;

            var image = CloneBitmap();

            foreach (var filter in Filters) filter.ApplyFilter(ref image);

            PreviousRenderedBitmap = image;
            LayerState = LayerState.Unchanged;
            return image;
        }

        private Bitmap CloneBitmap() => (Bitmap) Bitmap.Clone();

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            LayerState = LayerState.Updated;
    }
}