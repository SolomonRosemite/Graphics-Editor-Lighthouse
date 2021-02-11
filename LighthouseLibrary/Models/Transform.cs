using System.Runtime.Serialization;
using System.Drawing;
using System;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Transform : ISerializable
    {
        public int Id { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public LayerState LayerState { get; private set; } = LayerState.Updated;

        public Bitmap ApplyTransform(Bitmap bitmap)
        {
            LayerState = LayerState.Updated;

            Bitmap result = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(bitmap, 0, 0, Width, Height);

            return result;
        }

        public void ResizeOnlyWidth(int width)
        {
            LayerState = LayerState.Updated;

            Width = width;

            //int originalWidth = 1 / Percent * ; 
            //Width = (int)(originalWidth * Percent);
        }

        public void ResizeOnlyHeight(int height)
        {
            LayerState = LayerState.Updated;

            Height = height;
        }

        public void ResizeEqually(int value)
        {
            // Todo: This somehow doesn't work yet.
            // Fix me...
            LayerState = LayerState.Updated;

            Width += value;
            Height += value;
        }

        public Transform(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;
        }

        // Serializable & Constructors
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
        }

        public Transform(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            Width = GetValue<int>("Width");
            Height = GetValue<int>("Height");
            LayerState = LayerState.Updated;

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }
    }
}
