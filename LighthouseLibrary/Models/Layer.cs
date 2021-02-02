using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
        public string FileName { get; }

        public Layer(Bitmap bitmap, int id, string layerName, string fileName)
        {
            Id = id;

            Bitmap = bitmap;
            FileName = fileName;
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
            Filters = GetValue<ObservableCollection<Filter>>("Filters");
            LayerName = GetValue<string>("LayerName");
            FileName = GetValue<string>("FileName");
            Id = GetValue<int>("Id");

            LayerState = LayerState.Updated;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LayerName", LayerName);
            info.AddValue("FileName", FileName);
            info.AddValue("Filters", Filters);
            info.AddValue("Id", Id);
        }
    }
}