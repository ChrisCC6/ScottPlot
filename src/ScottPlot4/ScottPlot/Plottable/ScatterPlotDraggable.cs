using System;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// The scatter plot renders X/Y pairs as points and/or connected lines.
    /// Scatter plots can be extremely slow for large datasets, so use Signal plots in these situations.
    /// </summary>
    public class ScatterPlotDraggable : ScatterPlot, IDraggable
    {
        // Remark: obsolete as base class setter changed from private to protected attribute
        //private double[] xs;
        //public new double[] Xs { get => xs; private set { xs = value; OnPropertyChanged(); } }
        //private double[] ys;
        //public new double[] Ys { get => ys; private set { ys = value; OnPropertyChanged(); OnPropertyChanged(nameof(PointCount)); } }

        private int currentIndex = 0;
        public int CurrentIndex { get => currentIndex; set { currentIndex = value; OnPropertyChanged(); } }

        private bool dragEnabled = false;
        /// <summary>
        /// Indicates whether scatter points are draggable in user controls.
        /// </summary>
        public bool DragEnabled { get => dragEnabled; set { dragEnabled = value; OnPropertyChanged(); } }

        private bool dragEnabledX = true;
        /// <summary>
        /// Indicates whether scatter points are horizontally draggable in user controls.
        /// </summary>
        public bool DragEnabledX { get => dragEnabledX; set { dragEnabledX = value; OnPropertyChanged(); } }

        private bool dragEnabledY = true;
        /// <summary>
        /// Indicates whether scatter points are vertically draggable in user controls.
        /// </summary>
        public bool DragEnabledY { get => dragEnabledY; set { dragEnabledY = value; OnPropertyChanged(); } }

        private ScottPlot.Cursor dragCursor = ScottPlot.Cursor.Crosshair;
        /// <summary>
        /// Cursor to display while hovering over the scatter points if dragging is enabled.
        /// </summary>
        public ScottPlot.Cursor DragCursor { get => dragCursor; set { dragCursor = value; OnPropertyChanged(); } }

        private double dragXLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the points cannot be dragged more negative than this position
        /// </summary>
        public double DragXLimitMin { get => dragXLimitMin; set { dragXLimitMin = value; OnPropertyChanged(); } }

        private double dragXLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the points cannot be dragged more positive than this position
        /// </summary>
        public double DragXLimitMax { get => dragXLimitMax; set { dragXLimitMax = value; OnPropertyChanged(); } }

        private double dragYLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the points cannot be dragged more negative than this position
        /// </summary>
        public double DragYLimitMin { get => dragYLimitMin; set { dragYLimitMin = value; OnPropertyChanged(); } }

        private double dragYLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the points cannot be dragged more positive than this position
        /// </summary>
        public double DragYLimitMax { get => dragYLimitMax; set { dragYLimitMax = value; OnPropertyChanged(); } }

        /// <summary>
        /// This event is invoked after the plot is dragged
        /// </summary>
        public event EventHandler Dragged = delegate { };

        /// <summary>
        /// Move a scatter point to a new coordinate in plot space.
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

            if (DragEnabledX) Xs[CurrentIndex] = coordinateX;
            if (DragEnabledY) Ys[CurrentIndex] = coordinateY;

            Dragged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return True if a scatter point is within a certain number of pixels (snap) to the mouse
        /// </summary>
        /// <param name="coordinateX">mouse position (coordinate space)</param>
        /// <param name="coordinateY">mouse position (coordinate space)</param>
        /// <param name="snapX">snap distance (pixels)</param>
        /// <param name="snapY">snap distance (pixels)</param>
        /// <returns></returns>
        public new bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY)
        {
            bool test = false;
            for (int i = 0; i < PointCount; i++)
            {
                test = Math.Abs(Ys[i] - coordinateY) <= snapY && Math.Abs(Xs[i] - coordinateX) <= snapX;
                if (test)
                {
                    CurrentIndex = i;
                    return test;
                }
            }
            return test;
        }

        public ScatterPlotDraggable(double[] xs, double[] ys, double[] errorX = null, double[] errorY = null) : base(xs, ys, errorX, errorY)
        {
            this.Xs = xs;
            this.Ys = ys;
            this.XError = errorX;
            this.YError = errorY;
        }
    }
}
