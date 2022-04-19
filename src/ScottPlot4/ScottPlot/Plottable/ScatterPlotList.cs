using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// A collection of X/Y coordinates that can be displayed as markers and/or connected lines.
    /// Unlike the regular ScatterPlot, this plot type has Add() methods to easily add data.
    /// </summary>
    public class ScatterPlotList<T> : PropertyNotifier, IPlottable
    {

        private ObservableCollection<T> xs;
        public ObservableCollection<T> Xs
        {
            get => xs;
            set
            {
                if (xs != null)
                    xs.CollectionChanged -= Internal_CollectionChanged;
                xs = value;
                if (xs != null)
                    xs.CollectionChanged += Internal_CollectionChanged;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Count));
            }
        }

        private void Internal_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                OnPropertyChanged(nameof(Count));
        }

        private ObservableCollection<T> ys;
        public ObservableCollection<T> Ys
        {
            get => ys;
            set
            {
                if (ys != null)
                    ys.CollectionChanged -= Internal_CollectionChanged;
                ys = value;
                if (ys != null)
                    ys.CollectionChanged += Internal_CollectionChanged;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Count));
            }
        }

        public int Count => Xs.Count;

        private string label;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }
        private Color color = Color.Black;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        private double lineWidth = 1;
        public double LineWidth { get => lineWidth; set { lineWidth = value; OnPropertyChanged(); } }
        private LineStyle lineStyle = LineStyle.Solid;
        public LineStyle LineStyle { get => lineStyle; set { lineStyle = value; OnPropertyChanged(); } }
        private MarkerShape markerShape = MarkerShape.filledCircle;
        public MarkerShape MarkerShape { get => markerShape; set { markerShape = value; OnPropertyChanged(); } }
        private float markerSize = 3;
        public float MarkerSize { get => markerSize; set { markerSize = value; OnPropertyChanged(); } }

        public ScatterPlotList()
        {
            Xs = new();
            Ys = new();
        }

        public void ValidateData(bool deep = false)
        {
            if (Xs.Count != Ys.Count)
                throw new InvalidOperationException("Xs and Ys must be same length");
        }

        /// <summary>
        /// Clear the list of points
        /// </summary>
        public void Clear()
        {
            Xs.Clear();
            Ys.Clear();
        }

        /// <summary>
        /// Add a single point to the list
        /// </summary>
        public void Add(T x, T y)
        {
            Xs.Add(x);
            Ys.Add(y);
        }

        /// <summary>
        /// Add multiple points to the list
        /// </summary>
        public void AddRange(T[] xs, T[] ys)
        {
            if (xs is null)
                throw new ArgumentException("xs must not be null");
            if (ys is null)
                throw new ArgumentException("ys must not be null");
            if (xs.Length != ys.Length)
                throw new ArgumentException("xs and ys must have the same length");

            for (int i = 0; i < xs.Length; i++)
            {
                Xs.Add(xs[i]);
                Ys.Add(ys[i]);
            }

        }

        public AxisLimits GetAxisLimits()
        {
            if (Count == 0)
                return AxisLimits.NoLimits;

            var xs = Xs.Select(x => Convert.ToDouble(x));
            var ys = Ys.Select(y => Convert.ToDouble(y));

            return new AxisLimits(xs.Min(), xs.Max(), ys.Min(), ys.Max());
        }

        /// <summary>
        /// Return a new array containing pixel locations for each point of the scatter plot
        /// </summary>
        private PointF[] GetPoints(PlotDimensions dims)
        {
            return Enumerable.Range(0, Count)
                .Select(i => Coordinate.FromGeneric(Xs[i], Ys[i]))
                .Select(coord => coord.ToPixel(dims))
                .Select(px => new PointF(px.X, px.Y))
                .ToArray();
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            PointF[] points = GetPoints(dims);
            using var gfx = GDI.Graphics(bmp, dims, lowQuality);
            using var linePen = GDI.Pen(Color, LineWidth, LineStyle, true);

            if (LineStyle != LineStyle.None && LineWidth > 0 && Count > 1)
            {
                gfx.DrawLines(linePen, points);
            }

            if (MarkerShape != MarkerShape.none && MarkerSize > 0 && Count > 0)
            {
                foreach (PointF point in points)
                    MarkerTools.DrawMarker(gfx, point, MarkerShape, MarkerSize, Color);
            }
        }

        public LegendItem[] GetLegendItems()
        {
            var singleLegendItem = new LegendItem(this)
            {
                Label = Label,
                Color = Color,
                LineStyle = LineStyle,
                LineWidth = LineWidth,
                MarkerShape = MarkerShape,
                MarkerSize = MarkerSize
            };
            return new LegendItem[] { singleLegendItem };
        }
    }
}
