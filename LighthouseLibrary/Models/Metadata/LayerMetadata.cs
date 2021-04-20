using LighthouseLibrary.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Xml;

namespace LighthouseLibrary.Models.Metadata
{
    [Serializable]
    public class LayerMetadata : ISerializable
    {
        private RotateFlipType rotationType = RotateFlipType.RotateNoneFlipNone;
        private ObservableCollection<Filter> Filters { get; }
        public LayerState LayerState { get; private set; } = LayerState.Updated;
        public Transform Transform { get; }
        public ColorLayer Color { get; }

        public RotateFlipType RotationType
        {
            get => rotationType;
            set
            {
                LayerState = LayerState.Updated;
                rotationType = value;
            }
        }

        public void ApplyMetadata(ref Bitmap bitmap)
        {
            if (Transform.LayerState == LayerState.Updated)
                bitmap = Transform.ApplyTransform(bitmap);

            if (Color.LayerState == LayerState.Updated)
                bitmap = Color.ApplyColor(bitmap, this);
            
            foreach (var filter in Filters) filter.ApplyFilter(ref bitmap);
            bitmap.RotateFlip(RotationType);

            LayerState = LayerState.Unchanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            LayerState = LayerState.Updated;

        // Serializable & Constructors
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Filters", Filters);
            info.AddValue("RotationType", RotationType);
            info.AddValue("Transform", Transform);
            info.AddValue("Color", Color);
        }

        public LayerMetadata(SerializationInfo info, StreamingContext _)
        {
            Filters = GetValue<ObservableCollection<Filter>>("Filters");
            RotationType = GetValue<RotateFlipType>("RotationType");
            Transform = GetValue<Transform>("Transform");
            Color = GetValue<ColorLayer>("Color");

            Filters.CollectionChanged += OnCollectionChanged;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }

        public LayerMetadata(int width, int height)
        {
            Transform = new Transform(UtilService.GenerateNewId(), width, height);
            Color = new ColorLayer(UtilService.GenerateNewId(), width, height);

            Filters = new ObservableCollection<Filter>();
            Filters.CollectionChanged += OnCollectionChanged;
        }
    }
}