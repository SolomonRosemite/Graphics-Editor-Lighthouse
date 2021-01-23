using System;
using System.Drawing;
using System.Windows.Controls;
using LanguageExt;

namespace Lighthouse.DataStructures
{
    public class Layer
    {
        public int Id { get; }
        public string LayerName { get; set; }
        public Pixel[] Pixels { get; private set; }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public LayerType LayerType { get; }
        public LayerState LayerState { get; set; }

        public Layer(Bitmap bitmap, int id, string layerName, LayerType layerType)
        {
            Id = id;
            LayerName = layerName;
            Height = bitmap.Height;
            Width = bitmap.Width;
            LayerType = layerType;
            LayerState = LayerState.Unchanged;

            var pixels = new Pixel[Width * Height];

            uint index = 0;
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    pixels[index++] = new Pixel { A = c.A, R = c.R,G = c.G, B = c.B };
                }

            Pixels = pixels;
        }

        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            uint index = 0;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    bitmap.SetPixel(x, y, Pixels[index++].ToColor());

            return bitmap;
        }

        public Option<Pixel> GetPixel(int x, int y)
        {
            try
            {
                Pixel pixel = Pixels[Pixels.Length / Width * y + x];

                return pixel == null ? Option<Pixel>.None : Option<Pixel>.Some(pixel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Option<Pixel>.None;
            }
        }

        private bool SetPixel(int x, int y, Pixel pixel)
        {
            if (pixel == null)
                throw new Exception("The Pixel passed in was null");

            try
            {
                int index = Pixels.Length / Width * y + x;
                Pixel res = Pixels[index];

                if (res == null)
                    return false;

                Pixels[index] = pixel;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}