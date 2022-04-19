using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot.Plottable
{
    public class ErrorBar : PropertyNotifier, IPlottable, IHasLine, IHasMarker, IHasColor
    {
        private double[] xs;
        public double[] Xs { get => xs; set { xs = value; OnPropertyChanged(); } }
        private double[] ys;
        public double[] Ys { get => ys; set { ys = value; OnPropertyChanged(); } }
        private double[] xErrorsPositive;
        public double[] XErrorsPositive { get => xErrorsPositive; set { xErrorsPositive = value; OnPropertyChanged(); } }
        private double[] xErrorsNegative;
        public double[] XErrorsNegative { get => xErrorsNegative; set { xErrorsNegative = value; OnPropertyChanged(); } }
        private double[] yErrorsPositive;
        public double[] YErrorsPositive { get => yErrorsPositive; set { yErrorsPositive = value; OnPropertyChanged(); } }
        private double[] yErrorsNegative;
        public double[] YErrorsNegative { get => yErrorsNegative; set { yErrorsNegative = value; OnPropertyChanged(); } }
        private int capSize = 3;
        public int CapSize { get => capSize; set { capSize = value; OnPropertyChanged(); } }


        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private double lineWidth = 1;
        public double LineWidth { get => lineWidth; set { lineWidth = value; OnPropertyChanged(); } }
        private Color color = Color.Gray;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }
        public Color LineColor { get => Color; set { Color = value; OnPropertyChanged(); } }
        private LineStyle lineStyle = LineStyle.Solid;
        public LineStyle LineStyle { get => lineStyle; set { lineStyle = value; OnPropertyChanged(); } }
        private MarkerShape markerShape = MarkerShape.filledCircle;
        public MarkerShape MarkerShape { get => markerShape; set { markerShape = value; OnPropertyChanged(); } }
        private float markerLineWidth = 1;
        public float MarkerLineWidth { get => markerLineWidth; set { markerLineWidth = value; OnPropertyChanged(); } }
        private float markerSize = 0;
        public float MarkerSize { get => markerSize; set { markerSize = value; OnPropertyChanged(); } }
        public Color MarkerColor { get => Color; set { Color = value; OnPropertyChanged(); } }

        public ErrorBar(double[] xs, double[] ys, double[] xErrorsPositive, double[] xErrorsNegative, double[] yErrorsPositive, double[] yErrorsNegative)
        {
            Xs = xs;
            Ys = ys;
            XErrorsPositive = xErrorsPositive;
            XErrorsNegative = xErrorsNegative;
            YErrorsPositive = yErrorsPositive;
            YErrorsNegative = yErrorsNegative;
        }

        public AxisLimits GetAxisLimits()
        {
            double xMin = double.PositiveInfinity;
            double xMax = double.NegativeInfinity;
            double yMin = double.PositiveInfinity;
            double yMax = double.NegativeInfinity;

            for (int i = 0; i < Xs.Length; i++)
            {
                xMin = Math.Min(xMin, Xs[i] - (XErrorsNegative?[i] ?? 0));
                xMax = Math.Max(xMax, Xs[i] + (XErrorsPositive?[i] ?? 0));
                yMin = Math.Min(yMin, Ys[i] - (YErrorsNegative?[i] ?? 0));
                yMax = Math.Max(yMax, Ys[i] + (YErrorsPositive?[i] ?? 0));
            }

            return new AxisLimits(xMin, xMax, yMin, yMax);
        }

        public LegendItem[] GetLegendItems()
        {
            return new LegendItem[0];
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            using Graphics gfx = GDI.Graphics(bmp, dims, lowQuality, clipToDataArea: true);
            using Pen pen = GDI.Pen(Color, LineWidth, LineStyle, true);

            if (XErrorsPositive is not null && XErrorsNegative is not null)
            {
                DrawErrorBars(dims, gfx, pen, XErrorsPositive, XErrorsNegative, true);
            }

            if (YErrorsPositive is not null && YErrorsNegative is not null)
            {
                DrawErrorBars(dims, gfx, pen, YErrorsPositive, YErrorsNegative, false);
            }

            if (MarkerSize > 0 && MarkerShape != MarkerShape.none)
            {
                DrawMarkers(dims, gfx);
            }
        }

        private void DrawErrorBars(PlotDimensions dims, Graphics gfx, Pen pen, double[] errorPositive, double[] errorNegative, bool onXAxis)
        {
            for (int i = 0; i < Xs.Length; i++)
            {
                // Pixel centre = dims.GetPixel(new Coordinate(Xs[i], Ys[i]));

                if (onXAxis)
                {
                    Pixel left = dims.GetPixel(new Coordinate(Xs[i] - errorNegative[i], Ys[i]));
                    Pixel right = dims.GetPixel(new Coordinate(Xs[i] + errorPositive[i], Ys[i]));

                    gfx.DrawLine(pen, left.X, left.Y, right.X, right.Y);
                    gfx.DrawLine(pen, left.X, left.Y - CapSize, left.X, left.Y + CapSize);
                    gfx.DrawLine(pen, right.X, right.Y - CapSize, right.X, right.Y + CapSize);
                }
                else
                {
                    Pixel top = dims.GetPixel(new Coordinate(Xs[i], Ys[i] - errorNegative[i]));
                    Pixel bottom = dims.GetPixel(new Coordinate(Xs[i], Ys[i] + errorPositive[i]));

                    gfx.DrawLine(pen, top.X, top.Y, bottom.X, bottom.Y);
                    gfx.DrawLine(pen, top.X - CapSize, top.Y, top.X + CapSize, top.Y);
                    gfx.DrawLine(pen, bottom.X - CapSize, bottom.Y, bottom.X + CapSize, bottom.Y);
                }
            }
        }

        private void DrawMarkers(PlotDimensions dims, Graphics gfx)
        {
            PointF[] pixels = new PointF[Xs.Length];
            for (int i = 0; i < Xs.Length; i++)
            {
                float xPixel = dims.GetPixelX(Xs[i]);
                float yPixel = dims.GetPixelY(Ys[i]);
                pixels[i] = new(xPixel, yPixel);
            }
            MarkerTools.DrawMarkers(gfx, pixels, MarkerShape, MarkerSize, Color, MarkerLineWidth);
        }

        public void ValidateData(bool deep = false)
        {
            Validate.AssertHasElements(nameof(Xs), Xs);
            Validate.AssertHasElements(nameof(Ys), Ys);
            Validate.AssertEqualLength($"{nameof(Xs)}, {nameof(Ys)}", Xs, Ys);

            if (XErrorsPositive is not null || XErrorsNegative is not null)
            {
                Validate.AssertHasElements(nameof(XErrorsPositive), XErrorsPositive);
                Validate.AssertHasElements(nameof(XErrorsNegative), XErrorsNegative);
                Validate.AssertEqualLength($"{Xs} {nameof(XErrorsPositive)}, {nameof(XErrorsNegative)}", Xs, XErrorsPositive, XErrorsNegative);
            }

            if (YErrorsPositive is not null || YErrorsNegative is not null)
            {
                Validate.AssertHasElements(nameof(YErrorsPositive), YErrorsPositive);
                Validate.AssertHasElements(nameof(YErrorsNegative), YErrorsNegative);
                Validate.AssertEqualLength($"{Xs} {nameof(YErrorsPositive)}, {nameof(YErrorsNegative)}", Xs, YErrorsPositive, YErrorsNegative);
            }

            if (deep)
            {
                Validate.AssertAllReal(nameof(Xs), Xs);
                Validate.AssertAllReal(nameof(Ys), Ys);

                if (XErrorsPositive is not null && XErrorsNegative is not null)
                {
                    Validate.AssertAllReal(nameof(XErrorsPositive), XErrorsPositive);
                    Validate.AssertAllReal(nameof(XErrorsNegative), XErrorsNegative);
                }

                if (YErrorsPositive is not null && YErrorsNegative is not null)
                {
                    Validate.AssertAllReal(nameof(YErrorsPositive), YErrorsPositive);
                    Validate.AssertAllReal(nameof(YErrorsNegative), YErrorsNegative);
                }

                // TODO: Should we validate that errors are all positive?
            }
        }
    }
}
