using System.Runtime.Serialization;
using System.Drawing;
using System;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Transform : ISerializable
    {
        private int xPosition;
        private int yPosition;
        private byte opacity;

        public int Id { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }


        public int XPosition
        {
            get => xPosition;
            set
            {
                LayerState = LayerState.Updated;
                xPosition = value;
            }
        }

        public int YPosition
        {
            get => yPosition;
            set
            {
                LayerState = LayerState.Updated;
                yPosition = value;
            }
        }

        public byte Opacity
        {
            get => opacity;
            set
            {
                LayerState = LayerState.Updated;
                opacity = value;
            }
        }

        public LayerState LayerState { get; private set; } = LayerState.Updated;

        public Bitmap ApplyTransform(Bitmap bitmap)
        {
            LayerState = LayerState.Updated;

            Bitmap result = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(bitmap, 0, 0, Width, Height);

            return result;
        }

        public ResizeResult SetWidth(int width, bool resizeEqually)
        {
            LayerState = LayerState.Updated;

            if (!resizeEqually)
            {
                Width = width;
            }
            else
            {
                var diff = Width - width;

                Height -= diff;
                Width = width;
            }

            if (Width <= 0)
                Width = 1;

            if (Height <= 0)
                Height = 1;

            return new ResizeResult(Width, Height);
        }

        public ResizeResult SetHeight(int height, bool resizeEqually)
        {
            LayerState = LayerState.Updated;

            if (!resizeEqually)
            {
                Height = height;
            }
            else
            {
                var diff = Height - height;

                Width -= diff;
                Height = height;
            }

            if (Width <= 0)
                Width = 1;

            if (Height <= 0)
                Height = 1;

            return new ResizeResult(Width, Height);
        }

        public Transform(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;

            xPosition = 0;
            yPosition = 0;
            Opacity = 100;
        }

        // Serializable & Constructors
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("XPosition", XPosition);
            info.AddValue("YPosition", YPosition);
            info.AddValue("Opacity", Opacity);

        }

        public Transform(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            Width = GetValue<int>("Width");
            Height = GetValue<int>("Height");
            yPosition = GetValue<int>("Opacity");
            xPosition = GetValue<int>("XPosition");
            yPosition = GetValue<int>("YPosition");

            LayerState = LayerState.Updated;


            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }
    }

    public struct ResizeResult
    {
        public ResizeResult(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height { get; private set; }
        public int Width { get; private set; }
    }
}
