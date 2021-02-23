using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using FastBitmapLib;

namespace LighthouseBenchmark.Models
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class BitmapBenchmarks
    {
        private static Bitmap simple2000X2000Bitmap;
        private static Bitmap small;
        private static Bitmap medium;
        private static Bitmap large;
        private static Bitmap maximum;

        private static DirectBitmap simple2000X2000BitmapDirectBitmap;
        private static DirectBitmap smallDirectBitmap;
        private static DirectBitmap mediumDirectBitmap;
        private static DirectBitmap largeDirectBitmap;
        private static DirectBitmap maximumDirectBitmap;

        [GlobalSetup]
        public void GlobalSetup()
        {
            const string path = "./../../../../../../../Assets/";

            simple2000X2000Bitmap = new Bitmap(2000, 2000);

            small = new Bitmap(path + "Small.jpg");
            medium = new Bitmap(path + "medium.jpg");
            large = new Bitmap(path + "large.jpg");
            maximum = new Bitmap(path + "maximum.jpg");

            if (Image.GetPixelFormatSize(small.PixelFormat) != 32)
                small = SetupPixelFormat(small);

            if (Image.GetPixelFormatSize(medium.PixelFormat) != 32)
                medium = SetupPixelFormat(medium);

            if (Image.GetPixelFormatSize(large.PixelFormat) != 32)
                large = SetupPixelFormat(large);

            if (Image.GetPixelFormatSize(maximum.PixelFormat) != 32)
                maximum = SetupPixelFormat(maximum);

            simple2000X2000BitmapDirectBitmap = DirectBitmap.FromBitmap(simple2000X2000Bitmap);
            smallDirectBitmap = DirectBitmap.FromBitmap(small);
            mediumDirectBitmap = DirectBitmap.FromBitmap(medium);
            largeDirectBitmap = DirectBitmap.FromBitmap(large);
            maximumDirectBitmap = DirectBitmap.FromBitmap(maximum);
        }

        #region FastBitmap

        [Benchmark]
        public void Simple2000X2000FastBitmap() => RunFastBitmap(simple2000X2000Bitmap);

        [Benchmark]
        public void SmallFastBitmap() => RunFastBitmap(small);

        [Benchmark]
        public void MediumFastBitmap() =>  RunFastBitmap(medium);

        [Benchmark]
        public void LargeFastBitmap() =>  RunFastBitmap(large);

        [Benchmark]
        public void MaximumFastBitmap() => RunFastBitmap(maximum);

        private void RunFastBitmap(Bitmap bitmap)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            using(var fastBitmap = bitmap.FastLock())
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);

                        r = color.R > 252 ? 255 : color.R + 3;
                        g= color.G > 252 ? 255 : color.G + 3;
                        b = color.B > 252 ? 255 : color.B + 3;

                        fastBitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                    }
                }
            }
        }

        #endregion

        #region DirectBitmap

        [Benchmark]
        public void Simple2000X2000DirectBitmap() => RunDirectBitmap(simple2000X2000BitmapDirectBitmap);

        [Benchmark]
        public void SmallDirectBitmap() => RunDirectBitmap(smallDirectBitmap);

        [Benchmark]
        public void MediumDirectBitmap() => RunDirectBitmap(mediumDirectBitmap);

        [Benchmark]
        public void LargeDirectBitmap() =>  RunDirectBitmap(largeDirectBitmap);

        [Benchmark]
        public void MaximumDirectBitmap() => RunDirectBitmap(maximumDirectBitmap);

        private void RunDirectBitmap(DirectBitmap bitmap)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var color = bitmap.GetPixel(x, y);

                    r = color.R > 252 ? 255 : color.R + 3;
                    g= color.G > 252 ? 255 : color.G + 3;
                    b = color.B > 252 ? 255 : color.B + 3;

                    bitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                }
            }
        }

        #endregion

        #region Normal

        [Benchmark]
        public void Simple2000X2000SBitmap() => RunBitmap(simple2000X2000Bitmap);

        [Benchmark]
        public void SmallBitmap() => RunBitmap(small);

        [Benchmark]
        public void MediumBitmap() => RunBitmap(medium);

        [Benchmark]
        public void LargeBitmap() =>  RunBitmap(large);

        private void RunBitmap(Bitmap bitmap)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var color = bitmap.GetPixel(x, y);

                    r = color.R > 252 ? 255 : color.R + 3;
                    g= color.G > 252 ? 255 : color.G + 3;
                    b = color.B > 252 ? 255 : color.B + 3;

                    bitmap.SetPixel(x, y, Color.FromArgb(color.A, r, g, b));
                }
            }
        }

        #endregion

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