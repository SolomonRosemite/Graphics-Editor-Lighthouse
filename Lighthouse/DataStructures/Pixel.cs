using System;
using System.Drawing;

namespace Lighthouse.DataStructures
{
    public class Pixel
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color ToColor() => Color.FromArgb(A, R, G, B);

        public override string ToString()
        {
            return string.Concat("Pixel: ", "A: " + A, " R: " + R, " G: " + G, " B: " + B);
        }
    }
}