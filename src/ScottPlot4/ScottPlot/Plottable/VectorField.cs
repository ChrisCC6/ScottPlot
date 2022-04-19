using ScottPlot.Drawing;
using ScottPlot.Statistics;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// The VectorField displays arrows representing a 2D array of 2D vectors
    /// </summary>
    public class VectorField : PropertyNotifier, IPlottable
    {
        private double[] xs;
        public double[] Xs { get => xs; private set { xs = value; OnPropertyChanged(); } }
        private double[] ys;
        public double[] Ys { get => ys; private set { ys = value; OnPropertyChanged(); } }

        private Vector2[,] vectors;
        public Vector2[,] Vectors { get => vectors; private set { vectors = value; OnPropertyChanged(); OnPropertyChanged(nameof(PointCount)); } }
        private  Color[] vectorColors;
        public Color[] VectorColors { get => vectorColors; private set { vectorColors = value; OnPropertyChanged(); } }


        private string label = string.Empty;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }

        private readonly Renderable.ArrowStyle ArrowStyle = new();

        /// <summary>
        /// Describes which part of the vector line will be placed at the data coordinates.
        /// </summary>
        public ArrowAnchor Anchor { get => ArrowStyle.Anchor; set { ArrowStyle.Anchor = value; OnPropertyChanged(); } }

        /// <summary>
        /// If enabled arrowheads will be drawn as lines scaled to each vector's magnitude.
        /// </summary>
        public bool ScaledArrowheads { get => ArrowStyle.ScaledArrowheads; set { ArrowStyle.ScaledArrowheads = value; OnPropertyChanged(); } }

        /// <summary>
        /// When using scaled arrowheads this defines the width of the arrow relative to the vector line's length.
        /// </summary>
        public double ScaledArrowheadWidth { get => ArrowStyle.ScaledArrowheadWidth; set { ArrowStyle.ScaledArrowheadWidth = value; OnPropertyChanged(); } }

        /// <summary>
        /// When using scaled arrowheads this defines length of the arrowhead relative to the vector line's length.
        /// </summary>
        public double ScaledArrowheadLength { get => ArrowStyle.ScaledArrowheadLength; set { ArrowStyle.ScaledArrowheadLength = value; OnPropertyChanged(); } }

        /// <summary>
        /// Marker drawn at each coordinate
        /// </summary>
        public MarkerShape MarkerShape { get => ArrowStyle.MarkerShape; set { ArrowStyle.MarkerShape = value; OnPropertyChanged(); } }

        /// <summary>
        /// Size of markers to be drawn at each coordinate
        /// </summary>
        public float MarkerSize { get => ArrowStyle.MarkerSize; set { ArrowStyle.MarkerSize = value; OnPropertyChanged(); } }

        public VectorField(Vector2[,] vectors, double[] xs, double[] ys, Colormap colormap, double scaleFactor, Color defaultColor)
        {
            double minMagnitudeSquared = vectors[0, 0].LengthSquared();
            double maxMagnitudeSquared = vectors[0, 0].LengthSquared();
            for (int i = 0; i < xs.Length; i++)
            {
                for (int j = 0; j < ys.Length; j++)
                {
                    if (vectors[i, j].LengthSquared() > maxMagnitudeSquared)
                        maxMagnitudeSquared = vectors[i, j].LengthSquared();
                    else if (vectors[i, j].LengthSquared() < minMagnitudeSquared)
                        minMagnitudeSquared = vectors[i, j].LengthSquared();
                }
            }
            double minMagnitude = Math.Sqrt(minMagnitudeSquared);
            double maxMagnitude = Math.Sqrt(maxMagnitudeSquared);

            double[,] intensities = new double[xs.Length, ys.Length];
            for (int i = 0; i < xs.Length; i++)
            {
                for (int j = 0; j < ys.Length; j++)
                {
                    if (colormap != null)
                        intensities[i, j] = (vectors[i, j].Length() - minMagnitude) / (maxMagnitude - minMagnitude);
                    vectors[i, j] = Vector2.Multiply(vectors[i, j], (float)(scaleFactor / (maxMagnitude * 1.2)));
                }
            }

            double[] flattenedIntensities = intensities.Cast<double>().ToArray();
            VectorColors = colormap is null ?
                Enumerable.Range(0, flattenedIntensities.Length).Select(x => defaultColor).ToArray() :
                Colormap.GetColors(flattenedIntensities, colormap);

            this.Vectors = vectors;
            this.Xs = xs;
            this.Ys = ys;
        }

        public void ValidateData(bool deep = false) { /* validation occurs in constructor */ }

        public LegendItem[] GetLegendItems()
        {
            var singleLegendItem = new LegendItem(this)
            {
                Label = Label,
                Color = VectorColors[0],
                LineWidth = 10,
                MarkerShape = MarkerShape.none
            };
            return new LegendItem[] { singleLegendItem };
        }

        public AxisLimits GetAxisLimits() => new AxisLimits(Xs.Min() - 1, Xs.Max() + 1, Ys.Min() - 1, Ys.Max() + 1);

        public int PointCount { get => Vectors.Length; }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (IsVisible == false)
                return;

            using Graphics gfx = GDI.Graphics(bmp, dims, lowQuality);

            ArrowStyle.Render(dims, gfx, Xs, Ys, Vectors, VectorColors);
        }

        public override string ToString()
        {
            string label = string.IsNullOrWhiteSpace(this.Label) ? "" : $" ({this.Label})";
            return $"PlottableVectorField{label} with {PointCount} vectors";
        }
    }
}
