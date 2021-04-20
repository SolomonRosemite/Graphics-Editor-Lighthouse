using System;
using System.Drawing;
using System.Runtime.Serialization;
using LighthouseLibrary.Models.Metadata;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Layer : ISerializable
    {
        public int Id { get; }
        public string LayerName { get; set; }
        public string FileName { get; }

        public Bitmap Bitmap { get; }
        private Bitmap PreviousRenderedBitmap { get; set; }

        public LayerMetadata Metadata { get; }
        private LayerState LayerState { get; set; }


        public void RotateImageTest(RotateFlipType type) => Metadata.RotationType = type;

        public Bitmap RenderLayer()
        {

            // Note: When adding Metadata to the LayerMetadata make sure to add the LayerState here...
            if (
                LayerState == LayerState.Unchanged 
                && Metadata.LayerState == LayerState.Unchanged
                && Metadata.Transform.LayerState == LayerState.Unchanged
                && Metadata.Color.LayerState == LayerState.Unchanged
               )
                return PreviousRenderedBitmap;

            var image = CloneBitmap();

            Metadata.ApplyMetadata(ref image);

            PreviousRenderedBitmap = image;
            LayerState = LayerState.Unchanged;
            return image;
        }

        private Bitmap CloneBitmap() => (Bitmap) Bitmap.Clone();

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LayerName", LayerName);
            info.AddValue("FileName", FileName);
            info.AddValue("Metadata", Metadata);
            info.AddValue("Id", Id);
        }

        public Layer(SerializationInfo info, StreamingContext _)
        {
            Metadata = GetValue<LayerMetadata>("Metadata");
            LayerName = GetValue<string>("LayerName");
            FileName = GetValue<string>("FileName");
            Id = GetValue<int>("Id");

            LayerState = LayerState.Updated;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }

        public Layer(Bitmap bitmap, int id, string layerName, string fileName, LayerMetadata metadata)
        {
            Id = id;

            Bitmap = bitmap;
            FileName = fileName;
            LayerName = layerName;
            LayerState = LayerState.Updated;

            Metadata = metadata;
        }
    }
}