using ScottPlot.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// Text placed at a location relative to the data area that does not move when the axis limits change
    /// </summary>
    public class Annotation : PropertyNotifier, IPlottable
    {
        private double x;
        /// <summary>
        /// Horizontal location (in pixel units) relative to the data area
        /// </summary>
        public double X { get => x; set { x = value; OnPropertyChanged(); } }

        private double y;
        /// <summary>
        /// Vertical position (in pixel units) relative to the data area
        /// </summary>
        public double Y { get => y; set { y = value; OnPropertyChanged(); } }

        private string label;
        /// <summary>
        /// Text displayed in the annotation
        /// </summary>
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

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

        private bool background = true;
        public bool Background { get => background; set { background = value; OnPropertyChanged(); } }

        private Color backgroundColor = Color.Yellow;
        public Color BackgroundColor { get => backgroundColor; set { backgroundColor = value; OnPropertyChanged(); } }

        private bool shadow = true;
        public bool Shadow { get => shadow; set { shadow = value; OnPropertyChanged(); } }
        private Color shadowColor = Color.FromArgb(25, Color.Black);
        public Color ShadowColor { get => shadowColor; set { shadowColor = value; OnPropertyChanged(); } }

        private bool border = true;
        public bool Border { get => border; set { border = value; OnPropertyChanged(); } }
        private float borderWidth = 1;
        public float BorderWidth { get => borderWidth; set { borderWidth = value; OnPropertyChanged(); } }
        private Color borderColor = Color.Black;
        public Color BorderColor { get => borderColor; set { borderColor = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        public Annotation()
        {
            Font = new();
        }

        public override string ToString() => $"PlottableAnnotation at ({X} px, {Y} px)";
        public LegendItem[] GetLegendItems() => null;
        public AxisLimits GetAxisLimits() => new AxisLimits(double.NaN, double.NaN, double.NaN, double.NaN);

        public void ValidateData(bool deep = false)
        {
            if (double.IsNaN(X) || double.IsInfinity(X))
                throw new InvalidOperationException("xPixel must be a valid number");

            if (double.IsNaN(Y) || double.IsInfinity(Y))
                throw new InvalidOperationException("xPixel must be a valid number");
        }

        // TODO: the negative coordiante thing is silly. Use alignment fields to control this behavior.

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (!IsVisible)
                return;

            using var gfx = GDI.Graphics(bmp, dims, lowQuality, false);
            using var font = GDI.Font(Font);
            using var fontBrush = new SolidBrush(Font.Color);
            using var shadowBrush = new SolidBrush(ShadowColor);
            using var backgroundBrush = new SolidBrush(BackgroundColor);
            using var borderPen = new Pen(BorderColor, BorderWidth);

            SizeF size = GDI.MeasureString(gfx, Label, font);

            double x = (X >= 0) ? X : dims.DataWidth + X - size.Width;
            double y = (Y >= 0) ? Y : dims.DataHeight + Y - size.Height;
            PointF location = new PointF((float)x + dims.DataOffsetX, (float)y + dims.DataOffsetY);

            if (Background && Shadow)
                gfx.FillRectangle(shadowBrush, location.X + 5, location.Y + 5, size.Width, size.Height);

            if (Background)
                gfx.FillRectangle(backgroundBrush, location.X, location.Y, size.Width, size.Height);

            if (Border)
                gfx.DrawRectangle(borderPen, location.X, location.Y, size.Width, size.Height);

            gfx.DrawString(Label, font, fontBrush, location);
        }
    }
}
