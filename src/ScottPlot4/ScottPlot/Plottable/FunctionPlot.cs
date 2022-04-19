using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// A function plot displays a curve using a function (Y as a function of X)
    /// </summary>
    public class FunctionPlot : PropertyNotifier, IPlottable, IHasLine, IHasColor
    {
        /// <summary>
        /// The function to translate an X to a Y (or null if undefined)
        /// </summary>
        public Func<double, double?> Function;

        // customizations
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }

        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private bool isHighlighted { get; set; } = false;
        public bool IsHighlighted { get => isHighlighted; set { isHighlighted = value; OnPropertyChanged(); } }

        private float highlightCoefficient { get; set; } = 2;
        public float HighlightCoefficient { get => highlightCoefficient; set { highlightCoefficient = value; OnPropertyChanged(); } }

        private double _lineWidth = 1;
        public double LineWidth
        {
            get => IsHighlighted ? _lineWidth * HighlightCoefficient : _lineWidth;
            set { _lineWidth = value; OnPropertyChanged(); }
        }

        private LineStyle lineStyle = LineStyle.Solid;
        public LineStyle LineStyle { get => lineStyle; set { lineStyle = value; OnPropertyChanged(); } }

        private string label { get; set; } = string.Empty;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        private Color color = Color.Black;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        private Color lineColor = Color.Black;
        public Color LineColor { get => lineColor; set { lineColor = value; OnPropertyChanged(); } }


        public FunctionPlot(Func<double, double?> function)
        {
            Function = function;
        }

        public AxisLimits GetAxisLimits()
        {
            double max = double.NegativeInfinity;
            double min = double.PositiveInfinity;

            foreach (double x in DataGen.Range(-10, 10, .1))
            {
                double? y = Function(x);
                if (y != null)
                {
                    max = Math.Max(max, y.Value);
                    min = Math.Min(min, y.Value);
                }
            }

            // TODO: should X limits be null or NaN?
            return new AxisLimits(-10, 10, min, max);
        }

        public int PointCount { get; private set; }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();

            PointCount = (int)dims.DataWidth;
            for (int columnIndex = 0; columnIndex < dims.DataWidth; columnIndex++)
            {
                double x = columnIndex * dims.UnitsPerPxX + dims.XMin;
                try
                {
                    double? y = Function(x);

                    if (y is null)
                        throw new NoNullAllowedException();

                    if (double.IsNaN(y.Value) || double.IsInfinity(y.Value))
                        throw new ArithmeticException("not a real number");

                    xList.Add(x);
                    yList.Add(y.Value);
                }
                catch (Exception e) //Domain error, such log(-1) or 1/0
                {
                    Debug.WriteLine($"Y({x}) failed because {e}");
                    continue;
                }
            }

            // create a temporary scatter plot and use it for rendering
            double[] xs = xList.ToArray();
            double[] ys = yList.ToArray();
            var scatter = new ScatterPlot(xs, ys)
            {
                Color = Color,
                LineWidth = LineWidth,
                MarkerSize = 0,
                Label = Label,
                MarkerShape = MarkerShape.none,
                LineStyle = LineStyle
            };
            scatter.Render(dims, bmp, lowQuality);
        }

        public void ValidateData(bool deepValidation = false)
        {
            if (Function is null)
                throw new InvalidOperationException("function cannot be null");
        }

        public override string ToString()
        {
            string label = string.IsNullOrWhiteSpace(this.Label) ? "" : $" ({this.Label})";
            return $"PlottableFunction{label} displaying {PointCount} points";
        }

        public LegendItem[] GetLegendItems()
        {
            var singleLegendItem = new LegendItem(this)
            {
                Label = this.Label,
                Color = this.Color,
                LineStyle = this.LineStyle,
                LineWidth = this.LineWidth,
                MarkerShape = MarkerShape.none
            };
            return new LegendItem[] { singleLegendItem };
        }
    }
}
