using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using FastBitmapLib;

namespace LighthouseBenchmark.Models
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class ColorEditBenchmarks
    {
        private static Bitmap bitmapFilterMedium;
        private static Bitmap bitmapFilterLarge;
        private static Bitmap bitmapFilter;

        private static Bitmap medium;
        private static Bitmap large;

        // 20% Brightness increment...
        private const int Increment = 51;

        [GlobalSetup]
        public void GlobalSetup()
        {
            const string path = "./../../../../../../../Assets/";

            medium = new Bitmap(path + "medium.jpg");
            large = new Bitmap(path + "large.jpg");

            if (Image.GetPixelFormatSize(medium.PixelFormat) != 32)
                medium = SetupPixelFormat(medium);

            if (Image.GetPixelFormatSize(large.PixelFormat) != 32)
                large = SetupPixelFormat(large);

            bitmapFilterMedium = PrepareBitmapFilter(medium);
            bitmapFilterLarge = PrepareBitmapFilter(large);
        }

        [Benchmark]
        public void FastBitmapMedium()
        {
            int r = 0;
            int g = 0;
            int b = 0;

            using (var fastBitmap = medium.FastLock())
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);

                        if (color.R + Increment > 255)
                            r = 255;
                        else
                            r = color.R + Increment;

                        if (color.G + Increment > 255)
                            g = 255;
                        else
                            g = color.G + Increment;

                        if (color.B + Increment > 255)
                            b = 255;
                        else
                            b = color.B + Increment;

                        fastBitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                    }
                }
            }
        }

        [Benchmark]
        public void FastBitmapLarge()
        {
            int r = 0;
            int g = 0;
            int b = 0;

            using (var fastBitmap = large.FastLock())
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);

                        if (color.R + Increment > 255)
                            r = 255;
                        else
                            r = color.R + Increment;

                        if (color.G + Increment > 255)
                            g = 255;
                        else
                            g = color.G + Increment;

                        if (color.B + Increment > 255)
                            b = 255;
                        else
                            b = color.B + Increment;

                        fastBitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                    }
                }
            }
        }

        [Benchmark]
        public void FilterBitmapMedium()
        {
            bitmapFilter = new Bitmap(medium.Width, medium.Height);

            int r = 0;
            int g = 0;
            int b = 0;

            using (var fastBitmap = medium.FastLock())
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);

                        if (color.R + Increment > 255)
                            r = 255;
                        else
                            r = color.R + Increment;

                        if (color.G + Increment > 255)
                            g = 255;
                        else
                            g = color.G + Increment;

                        if (color.B + Increment > 255)
                            b = 255;
                        else
                            b = color.B + Increment;

                        fastBitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                    }
                }
            }

            Bitmap result = new Bitmap(medium.Width, medium.Height);
            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.DrawImage(bitmapFilter, 0, 0, bitmapFilter.Width, bitmapFilter.Height);
                gr.DrawImage(medium, 0, 0, medium.Width, medium.Height);
            }
        }

        [Benchmark]
        public void FilterBitmapLarge()
        {
            bitmapFilter = new Bitmap(large.Width, large.Height);

            int r = 0;
            int g = 0;
            int b = 0;

            using (var fastBitmap = large.FastLock())
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);

                        if (color.R + Increment > 255)
                            r = 255;
                        else
                            r = color.R + Increment;

                        if (color.G + Increment > 255)
                            g = 255;
                        else
                            g = color.G + Increment;

                        if (color.B + Increment > 255)
                            b = 255;
                        else
                            b = color.B + Increment;

                        fastBitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                    }
                }
            }

            Bitmap result = new Bitmap(large.Width, large.Height);
            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.DrawImage(bitmapFilter, 0, 0, bitmapFilter.Width, bitmapFilter.Height);
                gr.DrawImage(large, 0, 0, large.Width, large.Height);
            }
        }

        [Benchmark]
        public void FilterBitmapInitializedMedium()
        {
            Bitmap result = new Bitmap(medium.Width, medium.Height);
            using Graphics gr = Graphics.FromImage(result);
            gr.DrawImage(bitmapFilterMedium, 0, 0, bitmapFilterMedium.Width, bitmapFilterMedium.Height);
            gr.DrawImage(medium, 0, 0, medium.Width, medium.Height);
        }

        [Benchmark]
        public void FilterBitmapInitializedLarge()
        {
            Bitmap result = new Bitmap(large.Width, large.Height);
            using Graphics gr = Graphics.FromImage(result);
            gr.DrawImage(bitmapFilterLarge, 0, 0, bitmapFilterLarge.Width, bitmapFilterLarge.Height);
            gr.DrawImage(large, 0, 0, large.Width, large.Height);
        }

        private Bitmap PrepareBitmapFilter(Bitmap bitmap)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            using (var fastBitmap = bitmap.FastLock())
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);

                        if (color.R + Increment > 255)
                            r = 255;
                        else
                            r = color.R + Increment;

                        if (color.G + Increment > 255)
                            g = 255;
                        else
                            g = color.G + Increment;

                        if (color.B + Increment > 255)
                            b = 255;
                        else
                            b = color.B + Increment;

                        fastBitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                    }
                }
            }

            return bitmap;
        }

        private Bitmap SetupPixelFormat(Bitmap bitmap)
        {
            Bitmap clone = new Bitmap(bitmap.Width, bitmap.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            using (Graphics gr = Graphics.FromImage(clone)) {
                gr.DrawImage(bitmap, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return clone;
        }
    }
}