using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
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
        }
        public LayerMetadata(SerializationInfo info, StreamingContext _)
        {
            Filters = GetValue<ObservableCollection<Filter>>("Filters");
            RotationType = GetValue<RotateFlipType>("RotationType");
            Filters.CollectionChanged += OnCollectionChanged;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }
        public LayerMetadata()
        {
            Filters = new ObservableCollection<Filter>();
            Filters.CollectionChanged += OnCollectionChanged;
        }
    }
}