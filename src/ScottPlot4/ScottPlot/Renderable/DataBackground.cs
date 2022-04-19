using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScottPlot.Renderable
{
    public class DataBackground : PropertyNotifier, IRenderable
    {
        private Color color = Color.White;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality: true, false))
            using (var brush = GDI.Brush(Color))
            {
                var dataRect = new RectangleF(x: dims.DataOffsetX, y: dims.DataOffsetY, width: dims.DataWidth, height: dims.DataHeight);
                gfx.FillRectangle(brush, dataRect);
            }
        }
    }
}
