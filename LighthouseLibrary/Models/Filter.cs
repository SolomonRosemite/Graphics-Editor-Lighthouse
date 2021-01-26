using System.Drawing;

namespace LighthouseLibrary.Models
{
    public class Filter
    {
        public int Id { get; }

        public Filter(int id)
        {
            Id = id;
        }

        public void ApplyFilter(ref Bitmap bitmap)
        {
            // Todo: Apply Filter to bitmap.
        }
    }
}