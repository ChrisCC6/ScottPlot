using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScottPlot.Renderable
{
    public class AxisLine : PropertyNotifier, IRenderable
    {
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private Color color = Color.Black;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        private float width = 1;
        public float Width { get => width; set { width = value; OnPropertyChanged(); } }

        private Edge edge;
        public Edge Edge { get => edge; set { edge = value; OnPropertyChanged(); } }

        private float pixelOffset;
        public float PixelOffset { get => pixelOffset; set { pixelOffset = value; OnPropertyChanged(); } }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (IsVisible == false)
                return;

            using (var gfx = GDI.Graphics(bmp, dims, lowQuality, false))
            using (var pen = GDI.Pen(Color, Width))
            {
                float left = dims.DataOffsetX;
                float right = dims.DataOffsetX + dims.DataWidth;
                float top = dims.DataOffsetY;
                float bottom = dims.DataOffsetY + dims.DataHeight;

                if (Edge == Edge.Bottom)
                    gfx.DrawLine(pen, left, bottom + PixelOffset, right, bottom + PixelOffset);
                else if (Edge == Edge.Left)
                    gfx.DrawLine(pen, left - PixelOffset, bottom, left - PixelOffset, top);
                else if (Edge == Edge.Right)
                    gfx.DrawLine(pen, right + PixelOffset, bottom, right + PixelOffset, top);
                else if (Edge == Edge.Top)
                    gfx.DrawLine(pen, left, top - PixelOffset, right, top - PixelOffset);
                else
                    throw new NotImplementedException();
            }
        }
    }
}
