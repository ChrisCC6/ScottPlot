using System;
using System.Drawing;

namespace ScottPlot.Plottable
{
    [Obsolete("This plot type is deprecated: Use a regular scatter plot and call GetPointNearest(). See examples in documentation.", true)]
    public class ScatterPlotHighlight : ScatterPlot, IPlottable, IHasPoints, IHasHighlightablePoints
    {
        private MarkerShape highlightedShape = MarkerShape.openCircle;
        public MarkerShape HighlightedShape { get => highlightedShape; set { highlightedShape = value; OnPropertyChanged(); } }

        private float highlightedMarkerSize = 10;
        public float HighlightedMarkerSize { get => highlightedMarkerSize; set { highlightedMarkerSize = value; OnPropertyChanged(); } }

        private Color highlightedColor = Color.Red;
        public Color HighlightedColor { get => highlightedColor; set { highlightedColor = value; OnPropertyChanged(); } }

        private bool[] isHighlighted;
        protected new bool[] IsHighlighted { get => isHighlighted; set { isHighlighted = value; OnPropertyChanged(); } }

        public ScatterPlotHighlight(double[] xs, double[] ys, double[] xErr = null, double[] yErr = null) :
                                    base(xs, ys, xErr, yErr) => HighlightClear();

        public new void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false) => throw new NotImplementedException();
        public void HighlightClear() => throw new NotImplementedException();
        public (double x, double y, int index) HighlightPoint(int index) => throw new NotImplementedException();
        public (double x, double y, int index) HighlightPointNearestX(double x) => throw new NotImplementedException();
        public (double x, double y, int index) HighlightPointNearestY(double y) => throw new NotImplementedException();
        public (double x, double y, int index) HighlightPointNearest(double x, double y) => throw new NotImplementedException();
    }
}
