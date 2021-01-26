using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LighthouseLibrary.Models
{
    public class Layer
    {
        public int Id { get; }
        public string LayerName { get; set; }
        public LayerState LayerState { get; set; }
        public List<Filter> Filters { get; }

        private Bitmap bitmap;
        // private readonly Bitmap originalImage;

        public Layer(Bitmap bitmap, int id, string layerName)
        {
            Id = id;
            this.bitmap = bitmap;
            // originalImage = bitmap;
            LayerName = layerName;
            LayerState = LayerState.Unchanged;

            Filters = new List<Filter>();
        }


        public Bitmap RenderLayer()
        {
            Filters.ForEach(filter => filter.ApplyFilter(ref bitmap));
            return bitmap;
        }
    }
}