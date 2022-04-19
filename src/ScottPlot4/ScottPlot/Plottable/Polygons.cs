using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using ScottPlot.Drawing;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// Polygons are multiple Polygon objects.
    /// This plot type is faster alternative for rendering a large number of polygons
    /// compared to adding a bunch of individual Polygon objects to the plot.
    /// </summary>
    public class Polygons : PropertyNotifier, IPlottable, IHasColor
    {
        // data
        public readonly ObservableCollection<List<(double x, double y)>> Polys;

        // customization
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

        private bool skipOffScreenPolygons = true;
        public bool SkipOffScreenPolygons { get => skipOffScreenPolygons; set { skipOffScreenPolygons = value; OnPropertyChanged(); } }

        private bool renderSmallPolygonsAsSinglePixels = true;
        public bool RenderSmallPolygonsAsSinglePixels { get => renderSmallPolygonsAsSinglePixels; set { renderSmallPolygonsAsSinglePixels = value; OnPropertyChanged(); } }

        public Polygons(List<List<(double x, double y)>> polys)
        {
            Polys = new ObservableCollection<List<(double x, double y)>>();
            Polys.CollectionChanged += Polys_CollectionChanged;
            foreach (var item in polys)
            {
                Polys.Add(item);
            }
        }

        private void Polys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
        }

        public override string ToString()
        {
            string label = string.IsNullOrWhiteSpace(this.Label) ? "" : $" ({this.Label})";
            return $"PlottablePolygons {label} with {PointCount} polygons";
        }

        public void ValidateData(bool deep = false)
        {
            if (deep == false)
                return;

            foreach (var poly in Polys)
            {
                foreach (var point in poly)
                {
                    if (double.IsNaN(point.x) || double.IsNaN(point.y))
                        throw new InvalidOperationException("points cannot contain NaN");

                    if (double.IsInfinity(point.x) || double.IsInfinity(point.y))
                        throw new InvalidOperationException("points cannot contain Infinity");
                }
            }
        }

        public int PointCount { get => Polys.Count; }

        public AxisLimits GetAxisLimits()
        {
            double xMin = Polys[0][0].x;
            double xMax = Polys[0][0].x;
            double yMin = Polys[0][0].y;
            double yMax = Polys[0][0].y;

            foreach (var poly in Polys)
            {
                foreach (var (x, y) in poly)
                {
                    xMin = Math.Min(xMin, x);
                    xMax = Math.Max(xMax, x);
                    yMin = Math.Min(yMin, y);
                    yMax = Math.Max(yMax, y);
                }
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

        private bool IsBiggerThenPixel(List<(double x, double y)> poly, double UnitsPerPixelX, double UnitsPerPixelY)
        {
            double minX = poly[0].x;
            double maxX = poly[0].x;
            double minY = poly[0].y;
            double maxY = poly[0].y;
            double smallerThenPixelX = 0.5 * UnitsPerPixelX;
            double smallerThenPixelY = 0.5 * UnitsPerPixelY;
            for (int i = 1; i < poly.Count; i++)
            {
                if (poly[i].x < minX)
                {
                    minX = poly[i].x;
                    if (maxX - minX > smallerThenPixelX)
                        return true;
                }
                if (poly[i].x > maxX)
                {
                    maxX = poly[i].x;
                    if (maxX - minX > smallerThenPixelX)
                        return true;
                }
                if (poly[i].y < minX)
                {
                    minY = poly[i].y;
                    if (maxY - minY > smallerThenPixelY)
                        return true;
                }
                if (poly[i].y > maxX)
                {
                    maxY = poly[i].y;
                    if (maxY - minY > smallerThenPixelY)
                        return true;
                }
            }
            return (maxX - minX > smallerThenPixelX || maxY - minY > smallerThenPixelY);
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            using (Graphics gfx = GDI.Graphics(bmp, dims, lowQuality))
            using (Brush brush = GDI.Brush(FillColor, HatchColor, HatchStyle))
            using (Pen pen = GDI.Pen(LineColor, LineWidth))
            {
                foreach (List<(double x, double y)> poly in Polys)
                {
                    if (SkipOffScreenPolygons &&
                        poly.Where(pt => pt.x >= dims.XMin && pt.x <= dims.XMax &&
                                         pt.y >= dims.YMin && pt.y <= dims.YMax)
                            .Count() == 0)
                        continue;

                    var polyArray = RenderSmallPolygonsAsSinglePixels && !IsBiggerThenPixel(poly, dims.UnitsPerPxX, dims.UnitsPerPxY) ?
                        new PointF[] { new PointF(dims.GetPixelX(poly[0].x), dims.GetPixelY(poly[0].y)) } :
                        poly.Select(point => new PointF(dims.GetPixelX(point.x), dims.GetPixelY(point.y))).ToArray();

                    if (Fill)
                    {
                        if (polyArray.Length >= 3)
                            gfx.FillPolygon(brush, polyArray);
                        else
                            gfx.FillRectangle(brush, polyArray[0].X, polyArray[0].Y, 1, 1);
                    }

                    if (LineWidth > 0)
                        gfx.DrawPolygon(pen, polyArray);
                }
            }
        }
    }
}
