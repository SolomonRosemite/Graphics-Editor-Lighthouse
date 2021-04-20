using System;
using System.Runtime.Serialization;
using System.Drawing;
using LighthouseLibrary.Services;

namespace LighthouseLibrary.Models.Metadata
{
    [Serializable]
    public class ColorLayer : ISerializable
    {
        public int Id { get; }

        public int Width { get; }
        public int Height { get; }

        // States
        public LayerState LayerState { get; private set; } = LayerState.Updated;
        public LayerState BrightnessMapState { get; private set; } = LayerState.Updated;

        // Caches
        private Bitmap PreviousRenderedBitmap { get; set; }
        private Bitmap BrightnessMap { get; set; }

        // Values
        private float brightness;
        public float Brightness
        {
            get => brightness;
            set
            {
                LayerState = LayerState.Updated;
                BrightnessMapState = LayerState.Updated;
                brightness = value;
            }
        }

        public Bitmap ApplyColor(Bitmap image, LayerMetadata metadata)
        {
            // Apply Brightness
            if (LayerState == LayerState.Unchanged)
                return PreviousRenderedBitmap;

            if (BrightnessMapState == LayerState.Updated)
                BrightnessMap = CommonUtil.CreateBrightnessMap(metadata.Transform.Width, metadata.Transform.Height, Brightness);

            image = CommonUtil.MergeBitmap(image, BrightnessMap);

            LayerState = LayerState.Unchanged;
            BrightnessMapState = LayerState.Unchanged;

            PreviousRenderedBitmap = (Bitmap) image.Clone();
            return image;
        }

        // Serializable & Constructors
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("Brightness", Brightness);
        }

        public ColorLayer(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            Width = GetValue<int>("Width");
            Height = GetValue<int>("Height");
            Brightness = GetValue<float>("Brightness");

            LayerState = LayerState.Updated;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }

        public ColorLayer(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;

            Brightness = 1;
        }
    }
}