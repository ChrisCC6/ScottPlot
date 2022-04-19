using System;
using System.ComponentModel;
using System.Drawing;
using ScottPlot.Drawing;
using ScottPlot.Styles;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// An L-shaped scalebar rendered in the corner of the data area
    /// </summary>
    public class ScaleBar : PropertyNotifier, IPlottable, IStylable, IHasColor
    {
        private double width;
        /// <summary>
        /// Width of the scalebar in cooridinate units
        /// </summary>
        public double Width { get => width; set { width = value; OnPropertyChanged(); } }

        private double height;
        /// <summary>
        /// Height of the scalebar in cooridinate units
        /// </summary>
        public double Height { get => height; set { height = value; OnPropertyChanged(); } }

        private float padding = 10;
        /// <summary>
        /// Distance in pixels from the edge of the data area
        /// </summary>
        public float Padding { get => padding; set { padding = value; OnPropertyChanged(); } }

        private string horizontalLabel;
        public string HorizontalLabel { get => horizontalLabel; set { horizontalLabel = value; OnPropertyChanged(); } }

        private string verticalLabel;
        public string VerticalLabel { get => verticalLabel; set { verticalLabel = value; OnPropertyChanged(); } }

        private float lineWidth = 2;
        public float LineWidth { get => lineWidth; set { lineWidth = value; OnPropertyChanged(); } }

        private Color lineColor = Color.Black;
        public Color LineColor { get => lineColor; set { lineColor = value; OnPropertyChanged(); } }

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


        public float FontSize { get => Font.Size; set { Font.Size = value; OnPropertyChanged(); OnPropertyChanged(nameof(Font)); } }
        public Color FontColor { get => Font.Color; set { Font.Color = value; OnPropertyChanged(); OnPropertyChanged(nameof(Font)); } }
        public bool FontBold { get => Font.Bold; set { Font.Bold = value; OnPropertyChanged(); OnPropertyChanged(nameof(Font)); } }

        public Color Color { get => LineColor; set { LineColor = value; FontColor = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        public ScaleBar() { Font = new(); }

        public override string ToString() => $"PlottableScaleBar ({HorizontalLabel}={Width}, {VerticalLabel}={Height})";
        public AxisLimits GetAxisLimits() => new AxisLimits(double.NaN, double.NaN, double.NaN, double.NaN);
        public LegendItem[] GetLegendItems() => null;

        public void SetStyle(Color? tickMarkColor, Color? tickFontColor)
        {
            LineColor = tickMarkColor ?? LineColor;
            FontColor = tickFontColor ?? Font.Color;
        }

        public void ValidateData(bool deep = false)
        {
            if (double.IsNaN(Width) || double.IsNaN(Height))
                throw new InvalidOperationException("Width and Height cannot be NaN");
            if (double.IsInfinity(Width) || double.IsInfinity(Height))
                throw new InvalidOperationException("Width and Height cannot be Infinity");
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            using (var font = GDI.Font(Font))
            using (var fontBrush = new SolidBrush(Font.Color))
            using (var linePen = new Pen(LineColor, LineWidth))
            using (var sfNorth = new StringFormat() { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Center })
            using (var sfWest = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
            {
                // determine where the corner of the scalebar will be
                float widthPx = (float)(Width * dims.PxPerUnitX);
                float heightPx = (float)(Height * dims.PxPerUnitY);
                PointF cornerPoint = new PointF(dims.GetPixelX(dims.XMax) - Padding, dims.GetPixelY(dims.YMin) - Padding);

                // move the corner point away from the edge to accommodate label size
                var xLabelSize = GDI.MeasureString(gfx, HorizontalLabel, font);
                var yLabelSize = GDI.MeasureString(gfx, VerticalLabel, font);
                cornerPoint.X -= yLabelSize.Width * 1.2f;
                cornerPoint.Y -= yLabelSize.Height;

                // determine all other points relative to the corner point
                PointF horizPoint = new PointF(cornerPoint.X - widthPx, cornerPoint.Y);
                PointF vertPoint = new PointF(cornerPoint.X, cornerPoint.Y - heightPx);
                PointF horizMidPoint = new PointF((cornerPoint.X + horizPoint.X) / 2, cornerPoint.Y);
                PointF vertMidPoint = new PointF(cornerPoint.X, (cornerPoint.Y + vertPoint.Y) / 2);

                // draw the scalebar
                gfx.DrawLines(linePen, new PointF[] { horizPoint, cornerPoint, vertPoint });
                gfx.DrawString(HorizontalLabel, font, fontBrush, horizMidPoint.X, cornerPoint.Y, sfNorth);
                gfx.DrawString(VerticalLabel, font, fontBrush, cornerPoint.X, vertMidPoint.Y, sfWest);
            }
        }
    }
}
