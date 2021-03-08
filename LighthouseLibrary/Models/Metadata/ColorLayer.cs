using System;
using System.Runtime.Serialization;
using System.Drawing;

namespace LighthouseLibrary.Models.Metadata
{
    [Serializable]
    public class ColorLayer : ISerializable
    {
        public int Id { get; }

        public int Width { get; }
        public int Height { get; }

        public LayerState LayerState { get; private set; } = LayerState.Updated;

        private double brightness;
        public double Brightness
        {
            get => brightness;
            set
            {
                LayerState = LayerState.Updated;
                brightness = value;
            }
        }

        public ColorLayer(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;

            Brightness = 1;
        }

        public Bitmap ApplyColor(Bitmap image)
        {
            // Todo: Implement...
            return image;
            // throw new NotImplementedException();
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
            Brightness = GetValue<double>("Brightness");

            LayerState = LayerState.Updated;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }
    }
}