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
        public LayerState BrightnessState { get; private set; } = LayerState.Updated;
        public LayerState SaturationState { get; private set; } = LayerState.Updated;
        public LayerState ContrastState { get; private set; } = LayerState.Updated;

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
                BrightnessState = LayerState.Updated;
                brightness = value;
            }
        }

        private float contrast;
        public float Contrast
        {
            get => contrast;
            set
            {
                Console.WriteLine("WOW: " + Contrast);
                LayerState = LayerState.Updated;
                ContrastState = LayerState.Updated;
                contrast = value;
            }
        }

        private float saturation;
        public float Saturation
        {
            get => saturation;
            set
            {
                LayerState = LayerState.Updated;
                SaturationState = LayerState.Updated;
                saturation = value;
            }
        }

        public Bitmap ApplyColor(Bitmap image, LayerMetadata metadata)
        {
            // Apply Brightness
            if (LayerState == LayerState.Unchanged)
                return PreviousRenderedBitmap;

            if (BrightnessState == LayerState.Updated)
                BrightnessMap = CommonUtil.CreateBrightnessMap(metadata.Transform.Width, metadata.Transform.Height, Brightness);

            if (ContrastState == LayerState.Updated)
            {
                Console.WriteLine(Contrast);
                image = CommonUtil.AdjustContrast(image, metadata.Transform.Width, metadata.Transform.Height, 2f);
                // image = CommonUtil.AdjustContrast(image, metadata.Transform.Width, metadata.Transform.Height, 0f);
                // image = CommonUtil.AdjustContrast(image, metadata.Transform.Width, metadata.Transform.Height, 1.2f);
                return image;
            }

            // if (SaturationState == LayerState.Updated)
                // BrightnessMap = CommonUtil.CreateBrightnessMap(metadata.Transform.Width, metadata.Transform.Height, Brightness);

            image = CommonUtil.MergeBitmap(image, BrightnessMap);

            LayerState = LayerState.Unchanged;
            BrightnessState = LayerState.Unchanged;
            ContrastState = LayerState.Unchanged;
            SaturationState = LayerState.Unchanged;

            PreviousRenderedBitmap = (Bitmap) image.Clone();
            return image;
        }

        // Serializable & Constructors
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("Contrast", Contrast);
            info.AddValue("Brightness", Brightness);
            info.AddValue("Saturation", Saturation);
        }

        public ColorLayer(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            Width = GetValue<int>("Width");
            Height = GetValue<int>("Height");
            Contrast = GetValue<float>("Contrast");
            Saturation = GetValue<float>("Saturation");
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
            Contrast = 1;
            Saturation = 1;
        }
    }
}