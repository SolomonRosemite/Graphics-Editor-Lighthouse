using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lighthouse.Helpers
{
    public static class Helper
    {
        private static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }
        private static readonly Regex regex = new Regex(@"^\d+$");

        public static BitmapSource ToBitmapSource(Bitmap source)
        {
            BitmapSource bitSrc = null;
            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        public static bool IsNumbersOnly(string text, bool allowMinus = false)
        {
            string s = text.Trim();

            if (allowMinus && s.StartsWith('-'))
            {
                string content = s.Substring(1).Trim();

                if (content.Length == 0) return true;

                return regex.IsMatch(content) && content.Trim().Length != 0;
            }

            return regex.IsMatch(text) && text.Trim().Length != 0;
        }
    }
}