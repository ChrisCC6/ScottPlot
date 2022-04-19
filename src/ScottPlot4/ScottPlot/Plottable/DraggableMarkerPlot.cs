using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using ScottPlot.Drawing;
using System.ComponentModel;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// This plot type displays a marker at a point that can be dragged with the mouse.
    /// </summary>
    public class DraggableMarkerPlot : MarkerPlot, IDraggable, IHasMarker, IHasColor
    {
        private bool dragEnabled  = true;
        /// <summary>
        /// Indicates whether this marker is draggable in user controls.
        /// </summary>
        public bool DragEnabled { get => dragEnabled; set { dragEnabled = value; OnPropertyChanged(); } }

        private Cursor dragCursor = Cursor.Hand;
        /// <summary>
        /// Cursor to display while hovering over this marker if dragging is enabled.
        /// </summary>
        public Cursor DragCursor { get => dragCursor; set { dragCursor = value; OnPropertyChanged(); } }

        private double dragXLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the marker cannot be dragged more negative than this position
        /// </summary>
        public double DragXLimitMin { get => dragXLimitMin; set { dragXLimitMin = value; OnPropertyChanged(); } }

        private double dragXLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the marker cannot be dragged more positive than this position
        /// </summary>
        public double DragXLimitMax { get => dragXLimitMax; set { dragXLimitMax = value; OnPropertyChanged(); } }

        private double dragYLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the marker cannot be dragged more negative than this position
        /// </summary>
        public double DragYLimitMin { get => dragYLimitMin; set { dragYLimitMin = value; OnPropertyChanged(); } }

        private double dragYLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the marker cannot be dragged more positive than this position
        /// </summary>
        public double DragYLimitMax { get => dragYLimitMax; set { dragYLimitMax = value; OnPropertyChanged(); } }

        /// <summary>
        /// This event is invoked after the marker is dragged
        /// </summary>
        public event EventHandler Dragged = delegate { };

        /// <summary>
        /// Move the marker to a new coordinate in plot space.
        /// </summary>
        /// <param name="coordinateX">new X position</param>
        /// <param name="coordinateY">new Y position</param>
        /// <param name="fixedSize">This argument is ignored</param>
        public void DragTo(double coordinateX, double coordinateY, bool fixedSize)
        {
            if (!DragEnabled)
                return;

            if (coordinateX < DragXLimitMin) coordinateX = DragXLimitMin;
            if (coordinateX > DragXLimitMax) coordinateX = DragXLimitMax;
            if (coordinateX < DragYLimitMin) coordinateY = DragYLimitMin;
            if (coordinateX > DragYLimitMax) coordinateY = DragYLimitMax;
            X = coordinateX;
            Y = coordinateY;
            Dragged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return True if the marker is within a certain number of pixels (snap) to the mouse
        /// </summary>
        /// <param name="coordinateX">mouse position (coordinate space)</param>
        /// <param name="coordinateY">mouse position (coordinate space)</param>
        /// <param name="snapX">snap distance (pixels)</param>
        /// <param name="snapY">snap distance (pixels)</param>
        /// <returns></returns>
        public bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY) => Math.Abs(Y - coordinateY) <= snapY && Math.Abs(X - coordinateX) <= snapX;
    }

    /// <summary>
    /// This plot type displays a marker at a point that can be dragged with the mouse,
    /// but when dragged it "snapps" to specific X/Y coordinates defined by two arrays of values.
    /// </summary>
    public class DraggableMarkerPlotInVector : PropertyNotifier, IDraggable, IPlottable, IHasMarker
    {
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private double[] xs;
        /// <summary>
        /// Horizontal position in coordinate space
        /// </summary>
        public double[] Xs { get => xs; set { xs = value; OnPropertyChanged(); OnPropertyChanged(nameof(PointCount)); } }

        private double[] ys;
        /// <summary>
        /// Vertical position in coordinate space
        /// </summary>
        public double[] Ys { get => ys; set { ys = value; OnPropertyChanged(); } }

        public int PointCount => Xs.Length;

        private int currentIndex = 0;
        public int CurrentIndex { get => currentIndex; set { currentIndex = value; OnPropertyChanged(); } }

        private MarkerShape markerShape  = MarkerShape.filledCircle;
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
            get => markerSize;
            set { markerSize = value; OnPropertyChanged(); }
        }

        private Color color = Color.Black;
        /// <summary>
        /// Color of the marker to display at this point
        /// </summary>
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }


        /// <summary>
        /// Color of the marker to display at this point
        /// </summary>
        public Color MarkerColor { get => Color; set { Color = value; OnPropertyChanged(); } }
        
        private float markerLineWidth = 1;
        /// <summary>
        /// Width of the marker lines in pixel units
        /// </summary>
        public float MarkerLineWidth
        {
            get => markerLineWidth;
            set { markerLineWidth = value; OnPropertyChanged(); }
        }

        private string label { get; set; } = string.Empty;
        /// <summary>
        /// Text to appear in the legend (if populated)
        /// </summary>        
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        private bool dragEnabled = true;
        /// <summary>
        /// Indicates whether this marker on the scatter plot is draggable in user controls.
        /// </summary>
        public bool DragEnabled { get => dragEnabled; set { dragEnabled = value; OnPropertyChanged(); } }

        /// <summary>
        /// Cursor to display while hovering over this marker on the scatter plot if dragging is enabled.
        /// </summary>
        public Cursor DragCursor => Cursor.Crosshair;

        private double dragXLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the marker on the scatter plot cannot be dragged more negative than this position
        /// </summary>
        public double DragXLimitMin { get => dragXLimitMin; set { dragXLimitMin = value; OnPropertyChanged(); } }

        private double dragXLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the marker on the scatter plot cannot be dragged more positive than this position
        /// </summary>
        public double DragXLimitMax { get => dragXLimitMax; set { dragXLimitMax = value; OnPropertyChanged(); } }

        private double dragYLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the marker on the scatter plot cannot be dragged more negative than this position
        /// </summary>
        public double DragYLimitMin { get => dragYLimitMin; set { dragYLimitMin = value; OnPropertyChanged(); } }

        private double dragYLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the marker on the scatter plot cannot be dragged more positive than this position
        /// </summary>
        public double DragYLimitMax { get => dragYLimitMax; set { dragYLimitMax = value; OnPropertyChanged(); } }

        /// <summary>
        /// This event is invoked after the marker on the scatter plot is dragged
        /// </summary>
        public event EventHandler Dragged = delegate { };


        public AxisLimits GetAxisLimits() => new AxisLimits(Xs.Min(), Xs.Max(), Ys.Min(), Ys.Max());

        public void ValidateData(bool deep = false)
        {
            Debug.Assert(CurrentIndex >= 0 && CurrentIndex <= PointCount - 1, $"CurrentIndex property is out of bounds, it should be in the [0 - {PointCount - 1} range");
            Debug.Assert(CurrentIndex <= Xs.Length - 1);
            Validate.AssertHasElements(nameof(Xs), Xs);
            Validate.AssertHasElements(nameof(Ys), Ys);
            Validate.AssertEqualLength("", Xs, Ys);
            if (deep)
            {
                Validate.AssertAllReal(nameof(Xs), Xs);
                Validate.AssertAllReal(nameof(Ys), Ys);
            }
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (!IsVisible)
                return;

            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                PointF point = new PointF(dims.GetPixelX(Xs[CurrentIndex]), dims.GetPixelY(Ys[CurrentIndex]));

                MarkerTools.DrawMarker(gfx, point, MarkerShape, (float)MarkerSize, Color, MarkerLineWidth);
            }
        }

        /// <summary>
        /// Move the line to a new coordinate in plot space.
        /// </summary>
        /// <param name="coordinateX">new X position</param>
        /// <param name="coordinateY">new Y position</param>
        /// <param name="fixedSize">This argument is ignored</param>
        public void DragTo(double coordinateX, double coordinateY, bool fixedSize)
        {
            if (!DragEnabled)
                return;

            if (coordinateX < DragXLimitMin) coordinateX = DragXLimitMin;
            if (coordinateX > DragXLimitMax) coordinateX = DragXLimitMax;
            if (coordinateX < DragYLimitMin) coordinateY = DragYLimitMin;
            if (coordinateX > DragYLimitMax) coordinateY = DragYLimitMax;

            var distancessq = new double[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                distancessq[i] = Math.Pow(Xs[i] - coordinateX, 2) + Math.Pow(Ys[i] - coordinateY, 2);
            }
            for (int i = 0; i < PointCount; i++)
            {
                if (distancessq[i] < distancessq[CurrentIndex])
                {
                    CurrentIndex = i;
                }
            }

            Dragged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return True if the line is within a certain number of pixels (snap) to the mouse
        /// </summary>
        /// <param name="coordinateX">mouse position (coordinate space)</param>
        /// <param name="coordinateY">mouse position (coordinate space)</param>
        /// <param name="snapX">snap distance (pixels)</param>
        /// <param name="snapY">snap distance (pixels)</param>
        /// <returns></returns>
        public bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY) => Math.Abs(Ys[CurrentIndex] - coordinateY) <= snapY && Math.Abs(Xs[CurrentIndex] - coordinateX) <= snapX;

        public LegendItem[] GetLegendItems()
        {
            var singleItem = new LegendItem(this)
            {
                Label = this.Label,
                MarkerShape = this.MarkerShape,
                MarkerSize = this.MarkerSize,
                Color = this.Color,
            };
            return new LegendItem[] { singleItem };
        }
    }
}
