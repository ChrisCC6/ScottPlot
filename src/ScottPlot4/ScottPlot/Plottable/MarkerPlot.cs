using System.ComponentModel;
using System.Drawing;

namespace ScottPlot.Plottable
{
    public class MarkerPlot : PropertyNotifier, IPlottable, IHasMarker, IHasColor
    {
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }

        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private double x;
        /// <summary>
        /// Horizontal position in coordinate space
        /// </summary>
        public double X { get => x; set { x = value; OnPropertyChanged(); } }

        private double y;
        /// <summary>
        /// Vertical position in coordinate space
        /// </summary>
        public double Y { get => y; set { y = value; OnPropertyChanged(); } }

        private MarkerShape markerShape = MarkerShape.filledCircle;
        /// <summary>
        /// Marker to draw at this point
        /// </summary>
        public MarkerShape MarkerShape { get => markerShape; set { markerShape = value; OnPropertyChanged(); } }

        private float markerSize = 10;
        /// <summary>
        /// Size of the marker in pixel units
        /// </summary>
        public float MarkerSize
        {
            get => IsHighlighted ? markerSize * HighlightCoefficient : markerSize;
            set { markerSize = value; OnPropertyChanged(); }
        }

        private bool isHighlighted { get; set; } = false;
        public bool IsHighlighted { get => isHighlighted; set { isHighlighted = value; OnPropertyChanged(); } }

        private float highlightCoefficient { get; set; } = 2;
        public float HighlightCoefficient { get => highlightCoefficient; set { highlightCoefficient = value; OnPropertyChanged(); } }

        private float markerLineWidth = 1;
        /// <summary>
        /// Thickness of the marker lines in pixel units
        /// </summary>
        public float MarkerLineWidth
        {
            get => IsHighlighted ? (float)markerLineWidth * HighlightCoefficient : markerLineWidth;
            set { markerLineWidth = value; OnPropertyChanged(); }
        }

        private Color color;
        /// <summary>
        /// Color of the marker to display at this point
        /// </summary>
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        public Color MarkerColor { get => Color; set { Color = value; OnPropertyChanged(); } }

        private string label = string.Empty;
        /// <summary>
        /// Text to appear in the legend (if populated)
        /// </summary>
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        /// <summary>
        /// Text to appear on the graph at the point
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Font settings for rendering <see cref="Text"/>.
        /// Alignment and orientation relative to the marker can be configured here.
        /// </summary>
        public Drawing.Font TextFont = new();

        public AxisLimits GetAxisLimits() => new(X, X, Y, Y);

        public LegendItem[] GetLegendItems()
        {
            LegendItem item = new(this)
            {
                Label = this.Label,
                MarkerShape = this.MarkerShape,
                MarkerSize = this.MarkerSize,
                Color = this.Color
            };

            return new LegendItem[] { item };
        }

        public void ValidateData(bool deep = false)
        {
            Validate.AssertIsReal(nameof(X), X);
            Validate.AssertIsReal(nameof(Y), Y);
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (!IsVisible)
                return;

            PointF point = new(dims.GetPixelX(X), dims.GetPixelY(Y));

            using Graphics gfx = Drawing.GDI.Graphics(bmp, dims, lowQuality);
            MarkerTools.DrawMarker(gfx, point, MarkerShape, (float)MarkerSize, Color, MarkerLineWidth);

            if (!string.IsNullOrEmpty(Text))
            {
                SizeF stringSize = Drawing.GDI.MeasureString(gfx, Text, TextFont.Name, TextFont.Size, TextFont.Bold);
                gfx.TranslateTransform(point.X, point.Y);
                gfx.RotateTransform(TextFont.Rotation);

                (float dX, float dY) = Drawing.GDI.TranslateString(Text, TextFont);
                gfx.TranslateTransform(-dX, -dY);

                using var font = Drawing.GDI.Font(TextFont);
                using var fontBrush = new SolidBrush(TextFont.Color);
                gfx.DrawString(Text, font, fontBrush, new PointF(0, 0));

                Drawing.GDI.ResetTransformPreservingScale(gfx, dims);
            }
        }
    }
}
