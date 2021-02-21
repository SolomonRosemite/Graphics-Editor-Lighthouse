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

        public int PreviousWidth { get; private set; }
        public int PreviousHeight { get; private set; }

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

        public void SetWidth(int width)
        {
            LayerState = LayerState.Updated;

            PreviousWidth = Width;
            Width = width;

            if (Width <= 0)
                throw new Exception("Width was zero or less.");
        }

        public void SetHeight(int height)
        {
            Console.WriteLine();
            LayerState = LayerState.Updated;

            PreviousWidth = Width;

            Height = height;

            if (Height <= 0)
                throw new Exception("Height was zero or less.");
        }

        public void SetEqually(int value)
        {
            LayerState = LayerState.Updated;

            PreviousHeight = Height;
            PreviousWidth = Width;

            Height += value;
            Width += value;

            if (Height <= 0 || Width <= 0)
                throw new Exception("Width or Height was zero or less.");
        }

        public Transform(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;

            PreviousWidth = Width;
            PreviousHeight = Height;
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
            info.AddValue("PreviousWidth", PreviousWidth);
            info.AddValue("PreviousHeight", PreviousHeight);
            info.AddValue("XPosition", XPosition);
            info.AddValue("YPosition", YPosition);
            info.AddValue("Opacity", Opacity);

        }

        public Transform(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            Width = GetValue<int>("Width");
            Height = GetValue<int>("Height");
            LayerState = LayerState.Updated;
            PreviousWidth = GetValue<int>("PreviousWidth");
            PreviousHeight = GetValue<int>("PreviousHeight");
            xPosition = GetValue<int>("XPosition");
            yPosition = GetValue<int>("YPosition");
            yPosition = GetValue<int>("Opacity");

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }
    }
}
