using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Layer : ISerializable
    {
        public int Id { get; }
        public string LayerName { get; set; }

        private ObservableCollection<Filter> Filters { get; }
        private Bitmap PreviousRenderedBitmap { get; set; }
        private LayerState LayerState { get; set; }
        public Bitmap Bitmap { get; }

        public Layer(Bitmap bitmap, int id, string layerName)
        {
            Id = id;

            Bitmap = bitmap;
            LayerName = layerName;
            LayerState = LayerState.Updated;

            Filters = new ObservableCollection<Filter>();
            Filters.CollectionChanged += OnCollectionChanged;
        }

        public void RotateImageTest()
        {
            Bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            LayerState = LayerState.Updated;
        }

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

        public Layer(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            LayerName = GetValue<string>("LayerName");
            Filters = GetValue<ObservableCollection<Filter>>("Filters");

            PreviousRenderedBitmap = GetValue<Bitmap>("PreviousRenderedBitmap");
            LayerState = GetValue<LayerState>("LayerState");
            Bitmap = GetValue<Bitmap>("Bitmap");

            T GetValue<T>(string name)
            {
                return (T)info.GetValue(name, typeof(T));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("LayerName", LayerName);
            info.AddValue("Filters", Filters);
            info.AddValue("PreviousRenderedBitmap", PreviousRenderedBitmap);
            info.AddValue("LayerState", LayerState);
            info.AddValue("Bitmap", Bitmap);
        }
    }
}