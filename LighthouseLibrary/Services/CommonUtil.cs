using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LighthouseLibrary.Services
{
    public static class CommonUtil
    {
        public static Bitmap MergeBitmap(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp2 == null) return bmp1;

            Bitmap result = new Bitmap(bmp1.Width, bmp1.Height);
            using (Graphics g = Graphics.FromImage(result)) {
                g.DrawImage(bmp1, 0, 0, bmp1.Width, bmp1.Height);
                g.DrawImage(bmp2, 0, 0, bmp2.Width, bmp2.Height);
            }

            return result;
        }

        public static Bitmap CreateBrightnessMap(int width, int height, float brightness)
        {
            if (brightness == 1)
            {
                return null;
            }

            brightness -= 1;

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(brightness < 0 ? Color.Black : Color.White);

            return SetImageOpacity(bmp, Math.Abs(brightness));
        }

        public static Bitmap AdjustContrast(Bitmap image, int width, int height, float value)
        {
            return image;
        }

        // public static Bitmap AdjustContrast(Bitmap image, int width, int height, float value)
        // {
        //     return image;
        // }

        /// <summary>
        /// Method for changing the opacity of an image
        /// </summary>
        /// <param name="image">Image to set opacity on</param>
        /// <param name="opacity">Percentage of opacity</param>
        /// <returns></returns>
        public static Bitmap SetImageOpacity(Image image, float opacity)
        {
            try
            {
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                // Create a graphics object from the image
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity
                    matrix.Matrix33 = opacity;

                    //create image attributes
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static Bitmap SetupPixelFormat(Bitmap bitmap)
        {
            Bitmap clone = new Bitmap(bitmap.Width, bitmap.Height,
                PixelFormat.Format32bppPArgb);

            using (Graphics gr = Graphics.FromImage(clone)) {
                gr.DrawImage(bitmap, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return clone;
        }
    }
}