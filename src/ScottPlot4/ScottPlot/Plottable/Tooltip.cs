using ScottPlot.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// A tooltip displays a text bubble pointing to a specific location in X/Y space.
    /// The position of the bubble moves according to the axis limits to best display the text in the data area.
    /// </summary>
    public class Tooltip : PropertyNotifier, IPlottable, IHasColor
    {

        private double x;
        /// <summary>
        /// Tooltip position in coordinate space
        /// </summary>
        public double X { get => x; set { x = value; OnPropertyChanged(); } }

        private double y;
        /// <summary>
        /// Tooltip position in coordinate space
        /// </summary>
        public double Y { get => y; set { y = value; OnPropertyChanged(); } }

        // customization
        private string label = string.Empty;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private float borderWidth = 2;
        public float BorderWidth { get => borderWidth; set { borderWidth = value; OnPropertyChanged(); } }
        private Color borderColor = Color.Black;
        public Color BorderColor { get => borderColor; set { borderColor = value; OnPropertyChanged(); } }

        private Color fillColor = Color.White;
        public Color FillColor { get => fillColor; set { fillColor = value; OnPropertyChanged(); OnPropertyChanged(nameof(Color)); } }

        public Color Color { get => FillColor; set { FillColor = value; OnPropertyChanged(); } }

        private Drawing.Font font;
        public Drawing.Font Font
        {
            get => font;
            set
            {
                if (font != null)
                    font.PropertyChanged -= Internal_PropertyChanged;
                font = value;
                if (font != null)
                    font.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            }
        }
        private void Internal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
        }

        private int arrowSize = 5;
        public int ArrowSize { get => arrowSize; set { arrowSize = value; OnPropertyChanged(); } }

        private int labelPadding = 10;
        public int LabelPadding { get => labelPadding; set { labelPadding = value; OnPropertyChanged(); } }

        public LegendItem[] GetLegendItems() => null;

        public AxisLimits GetAxisLimits() => new AxisLimits(double.NaN, double.NaN, double.NaN, double.NaN);

        public void ValidateData(bool deep = false)
        {
            if (string.IsNullOrEmpty(Label))
                throw new InvalidOperationException("Label may not be empty");

            if (double.IsNaN(X) || double.IsInfinity(X))
                throw new InvalidOperationException("X must be a real number");

            if (double.IsNaN(Y) || double.IsInfinity(Y))
                throw new InvalidOperationException("Y must be a real number");
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (!IsVisible)
                return;

            using (var gfx = GDI.Graphics(bmp, dims, lowQuality, clipToDataArea: true))
            using (var font = GDI.Font(Font))
            using (var fillBrush = GDI.Brush(FillColor))
            using (var fontBrush = GDI.Brush(Font.Color))
            using (var pen = GDI.Pen(BorderColor, BorderWidth))
            {
                SizeF labelSize = gfx.MeasureString(Label, font);

                bool labelIsOnRight = dims.DataWidth - dims.GetPixelX(X) - labelSize.Width > 0;
                int sign = labelIsOnRight ? 1 : -1;

                PointF arrowHeadLocation = new PointF(dims.GetPixelX(X), dims.GetPixelY(Y));

                float contentBoxInsideEdgeX = arrowHeadLocation.X + sign * ArrowSize;
                PointF upperArrowVertex = new PointF(contentBoxInsideEdgeX, arrowHeadLocation.Y - ArrowSize);
                PointF lowerArrowVertex = new PointF(contentBoxInsideEdgeX, arrowHeadLocation.Y + ArrowSize);

                float contentBoxTopEdge = upperArrowVertex.Y - LabelPadding;
                float contentBoxBottomEdge = Math.Max(contentBoxTopEdge + labelSize.Height, lowerArrowVertex.Y) + 2 * LabelPadding;

                PointF[] points =
                {
                    arrowHeadLocation,
                    upperArrowVertex,
                    new PointF(contentBoxInsideEdgeX, upperArrowVertex.Y - LabelPadding),
                    new PointF(contentBoxInsideEdgeX + sign * (labelSize.Width + LabelPadding), upperArrowVertex.Y - LabelPadding),
                    new PointF(contentBoxInsideEdgeX + sign * (labelSize.Width + LabelPadding), contentBoxBottomEdge),
                    new PointF(contentBoxInsideEdgeX, contentBoxBottomEdge),
                    lowerArrowVertex,
                    arrowHeadLocation,
                    // add one more point to prevent render artifacts where thick line ends meet
                    upperArrowVertex,
                };

                byte[] pathPointTypes = Enumerable.Range(0, points.Length).Select(_ => (byte)System.Drawing.Drawing2D.PathPointType.Line).ToArray();

                var path = new System.Drawing.Drawing2D.GraphicsPath(points, pathPointTypes);

                gfx.FillPath(fillBrush, path);
                gfx.DrawPath(pen, path);

                float labelOffsetX = labelIsOnRight ? 0 : -labelSize.Width;
                float labelX = contentBoxInsideEdgeX + labelOffsetX + sign * LabelPadding / 2;
                float labelY = upperArrowVertex.Y;
                gfx.DrawString(Label, font, fontBrush, labelX, labelY);
            }
        }
    }
}
