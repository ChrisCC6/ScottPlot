using ScottPlot.Drawing;
using System.Drawing;

namespace ScottPlot.Renderable
{
    public class FigureBackground :PropertyNotifier,  IRenderable
    {
        private Color color = Color.White;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality: true, false))
            {
                gfx.Clear(Color);
            }
        }
    }
}
