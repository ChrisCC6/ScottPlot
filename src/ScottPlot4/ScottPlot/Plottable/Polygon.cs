using ScottPlot.Drawing;
using ScottPlot.Renderable;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// A polygon is a collection of X/Y points that are all connected to form a closed shape.
    /// Polygons can be optionally filled with a color or a gradient.
    /// </summary>
    public class Polygon : PropertyNotifier, IPlottable, IHasColor
    {
        // data
        private double[] xs;
        public double[] Xs { get => xs; set { xs = value; OnPropertyChanged(); } }
        private double[] ys;
        public double[] Ys { get => ys; set { ys = value; OnPropertyChanged(); } }

        // configuration
        private string label = string.Empty;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        private double lineWidth = 1;
        public double LineWidth
        {
            get => IsHighlighted ? lineWidth * HighlightCoefficient : lineWidth;
            set { lineWidth = value; OnPropertyChanged(); }
        }

        private bool isHighlighted = false;
        public bool IsHighlighted { get => isHighlighted; set { isHighlighted = value; OnPropertyChanged(); } }
        private float highlightCoefficient = 2;
        public float HighlightCoefficient { get => highlightCoefficient; set { highlightCoefficient = value; OnPropertyChanged(); } }

        private Color lineColor = Color.Black;
        public Color LineColor { get => lineColor; set { lineColor = value; OnPropertyChanged(); } }
        
        private bool fill = true;
        public bool Fill { get => fill; set { fill = value; OnPropertyChanged(); } }
        private Color fillColor = Color.Gray;
        public Color FillColor { get => fillColor; set { fillColor = value; OnPropertyChanged(); } }
        public Color Color { get => FillColor; set { FillColor = value; OnPropertyChanged(); } }

        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }

        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private Color hatchColor = Color.Transparent;
        public Color HatchColor { get => hatchColor; set { hatchColor = value; OnPropertyChanged(); } }

        private HatchStyle hatchStyle = HatchStyle.None;
        public HatchStyle HatchStyle { get => hatchStyle; set { hatchStyle = value; OnPropertyChanged(); } }

        public Polygon(double[] xs, double[] ys)
        {
            Xs = xs;
            Ys = ys;
        }

        public override string ToString()
        {
            string label = string.IsNullOrWhiteSpace(this.Label) ? "" : $" ({this.Label})";
            return $"PlottablePolygon{label} with {PointCount} points";
        }

        public int PointCount { get => Xs.Length; }

        public AxisLimits GetAxisLimits()
        {
            double xMin = Xs[0];
            double xMax = Xs[0];
            double yMin = Ys[0];
            double yMax = Ys[0];

            for (int i = 1; i < Xs.Length; i++)
            {
                xMin = Math.Min(xMin, Xs[i]);
                xMax = Math.Max(xMax, Xs[i]);
                yMin = Math.Min(yMin, Ys[i]);
                yMax = Math.Max(yMax, Ys[i]);
            }

            return new AxisLimits(xMin, xMax, yMin, yMax);
        }

        public LegendItem[] GetLegendItems()
        {
            var singleLegendItem = new LegendItem(this)
            {
                Label = this.Label,
                Color = Fill ? FillColor : LineColor,
                LineWidth = Fill ? 10 : LineWidth,
                MarkerShape = MarkerShape.none,
                HatchColor = this.HatchColor,
                HatchStyle = this.HatchStyle
            };
            return new LegendItem[] { singleLegendItem };
        }

        public void ValidateData(bool deep = false)
        {
            Validate.AssertHasElements("xs", Xs);
            Validate.AssertHasElements("ys", Ys);
            Validate.AssertEqualLength("xs and ys", Xs, Ys);

            if (Xs.Length < 3)
                throw new InvalidOperationException("polygons must contain at least 3 points");

            if (deep)
            {
                Validate.AssertAllReal("xs", Xs);
                Validate.AssertAllReal("ys", Ys);
            }
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            PointF[] points = new PointF[Xs.Length];
            for (int i = 0; i < Xs.Length; i++)
                points[i] = new PointF(dims.GetPixelX(Xs[i]), dims.GetPixelY(Ys[i]));

            using (Graphics gfx = GDI.Graphics(bmp, dims, lowQuality))
            using (Brush fillBrush = GDI.Brush(FillColor, HatchColor, HatchStyle))
            using (Pen outlinePen = GDI.Pen(LineColor, (float)LineWidth))
            {
                if (Fill)
                    gfx.FillPolygon(fillBrush, points);

                if (LineWidth > 0)
                    gfx.DrawPolygon(outlinePen, points);
            }
        }
    }
}
