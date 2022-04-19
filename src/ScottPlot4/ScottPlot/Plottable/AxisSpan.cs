using ScottPlot.Drawing;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace ScottPlot.Plottable
{
    public abstract class AxisSpan : PropertyNotifier, IPlottable, IDraggable, IHasColor, IHasArea, ISelectable
    {
        // location and orientation
        private double position1;
        protected double Start { get => position1;
            set { position1 = value; OnPropertyChanged();} }
        private double position2;
        protected double End { get => position2;
            set { position2 = value; OnPropertyChanged();} }

        private double Min { get => Math.Min(Start, End); }
        private double Max { get => Math.Max(Start, End); }
        readonly bool IsHorizontal;

        private bool ignoreAxisAuto = false;
        /// <summary>
        /// If true, AxisAuto() will ignore the position of this span when determining axis limits
        /// </summary>
        public bool IgnoreAxisAuto { get => ignoreAxisAuto; set { ignoreAxisAuto = value; OnPropertyChanged(); } }

        // configuration

        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private Color color = Color.FromArgb(128, Color.Magenta);
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }
        private Color borderColor = Color.Transparent;
        public Color BorderColor { get => borderColor; set { borderColor = value; OnPropertyChanged(); } }
        private float borderLineWidth = 0;
        public float BorderLineWidth { get => borderLineWidth; set { borderLineWidth = value; OnPropertyChanged(); } }
        private LineStyle borderLineStyle = LineStyle.None;
        public LineStyle BorderLineStyle { get => borderLineStyle; set { borderLineStyle = value; OnPropertyChanged(); } }
        private Color hatchColor = Color.Transparent;
        public Color HatchColor { get => hatchColor; set { hatchColor = value; OnPropertyChanged(); } }
        private HatchStyle hatchStyle = Drawing.HatchStyle.None;
        public HatchStyle HatchStyle { get => hatchStyle; set { hatchStyle = value; OnPropertyChanged(); } }
        private string label = string.Empty;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        // mouse interaction
        private bool dragEnabled;
        public bool DragEnabled { get => dragEnabled; set { dragEnabled = value; OnPropertyChanged(); } }
        private bool dragFixedSize;
        public bool DragFixedSize { get => dragFixedSize; set { dragFixedSize = value; OnPropertyChanged(); } }
        private double dragLimitMin = double.NegativeInfinity;
        public double DragLimitMin { get => dragLimitMin; set { dragLimitMin = value; OnPropertyChanged(); } }
        private double dragLimitMax = double.PositiveInfinity;
        public double DragLimitMax { get => dragLimitMax; set { dragLimitMax = value; OnPropertyChanged(); } }

        public Cursor DragCursor => IsHorizontal ? Cursor.WE : Cursor.NS;

        /// <summary>
        /// This event is invoked after the line is dragged 
        /// </summary>
        public event EventHandler Dragged = delegate { };

        public AxisSpan(bool isHorizontal)
        {
            IsHorizontal = isHorizontal;
        }

        public void ValidateData(bool deep = false)
        {
            if (double.IsNaN(Start) || double.IsInfinity(Start))
                throw new InvalidOperationException("position1 must be a valid number");

            if (double.IsNaN(End) || double.IsInfinity(End))
                throw new InvalidOperationException("position2 must be a valid number");
        }

        public LegendItem[] GetLegendItems()
        {
            var singleItem = new LegendItem(this)
            {
                Label = this.Label,
                Color = this.Color,
                BorderWith = Math.Min(BorderLineWidth, 3),
                BorderColor = this.BorderColor,
                BorderLineStyle = this.BorderLineStyle,
                HatchColor = this.HatchColor,
                HatchStyle = this.HatchStyle,
            };
            return new LegendItem[] { singleItem };
        }

        public AxisLimits GetAxisLimits()
        {
            if (IgnoreAxisAuto)
                return new AxisLimits(double.NaN, double.NaN, double.NaN, double.NaN);

            if (IsHorizontal)
                return new AxisLimits(Min, Max, double.NaN, double.NaN);
            else
                return new AxisLimits(double.NaN, double.NaN, Min, Max);
        }

        private enum Edge { Edge1, Edge2, Neither };
        Edge edgeUnderMouse = Edge.Neither;

        /// <summary>
        /// Return True if either span edge is within a certain number of pixels (snap) to the mouse
        /// </summary>
        /// <param name="coordinateX">mouse position (coordinate space)</param>
        /// <param name="coordinateY">mouse position (coordinate space)</param>
        /// <param name="snapX">snap distance (pixels)</param>
        /// <param name="snapY">snap distance (pixels)</param>
        /// <returns></returns>
        public bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY)
        {
            if (IsHorizontal)
            {
                if (Math.Abs(Start - coordinateX) <= snapX)
                    edgeUnderMouse = Edge.Edge1;
                else if (Math.Abs(End - coordinateX) <= snapX)
                    edgeUnderMouse = Edge.Edge2;
                else
                    edgeUnderMouse = Edge.Neither;
            }
            else
            {
                if (Math.Abs(Start - coordinateY) <= snapY)
                    edgeUnderMouse = Edge.Edge1;
                else if (Math.Abs(End - coordinateY) <= snapY)
                    edgeUnderMouse = Edge.Edge2;
                else
                    edgeUnderMouse = Edge.Neither;
            }

            return edgeUnderMouse != Edge.Neither;
        }

        /// <summary>
        /// Move the span to a new coordinate in plot space.
        /// </summary>
        /// <param name="coordinateX">new X position</param>
        /// <param name="coordinateY">new Y position</param>
        /// <param name="fixedSize">if True, both edges will be moved to maintain the size of the span</param>
        public void DragTo(double coordinateX, double coordinateY, bool fixedSize)
        {
            if (!DragEnabled)
                return;

            if (IsHorizontal)
            {
                coordinateX = Math.Max(coordinateX, DragLimitMin);
                coordinateX = Math.Min(coordinateX, DragLimitMax);
            }
            else
            {
                coordinateY = Math.Max(coordinateY, DragLimitMin);
                coordinateY = Math.Min(coordinateY, DragLimitMax);
            }

            double sizeBeforeDrag = End - Start;
            if (edgeUnderMouse == Edge.Edge1)
            {
                Start = IsHorizontal ? coordinateX : coordinateY;
                if (DragFixedSize || fixedSize)
                    End = Start + sizeBeforeDrag;
            }
            else if (edgeUnderMouse == Edge.Edge2)
            {
                End = IsHorizontal ? coordinateX : coordinateY;
                if (DragFixedSize || fixedSize)
                    Start = End - sizeBeforeDrag;
            }
            else
            {
                Debug.WriteLine("DragTo() called but no side selected. Call IsUnderMouse() to select a side.");
            }

            // ensure fixed-width spans stay entirely inside the allowable range
            double belowLimit = DragLimitMin - Start;
            double aboveLimit = End - DragLimitMax;
            if (belowLimit > 0)
            {
                Start += belowLimit;
                End += belowLimit;
            }
            if (aboveLimit > 0)
            {
                Start -= aboveLimit;
                End -= aboveLimit;
            }

            Dragged(this, EventArgs.Empty);
        }

        private RectangleF GetClippedRectangle(PlotDimensions dims)
        {
            // clip the rectangle to the size of the data area to avoid GDI rendering errors
            float ClippedPixelX(double x) => dims.GetPixelX(Math.Max(dims.XMin, Math.Min(x, dims.XMax)));
            float ClippedPixelY(double y) => dims.GetPixelY(Math.Max(dims.YMin, Math.Min(y, dims.YMax)));

            float left = IsHorizontal ? ClippedPixelX(Min) : dims.DataOffsetX;
            float right = IsHorizontal ? ClippedPixelX(Max) : dims.DataOffsetX + dims.DataWidth;
            float top = IsHorizontal ? dims.DataOffsetY : ClippedPixelY(Max);
            float bottom = IsHorizontal ? dims.DataOffsetY + dims.DataHeight : ClippedPixelY(Min);

            float width = right - left + 1;
            float height = bottom - top + 1;

            return new RectangleF(left, top, width, height);
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            using (var brush = GDI.Brush(Color, HatchColor, HatchStyle))
            using (var pen = GDI.Pen(BorderColor, BorderLineWidth, BorderLineStyle))
            {
                RectangleF rect = GetClippedRectangle(dims);
                gfx.FillRectangle(brush, rect);
                if (BorderLineWidth > 0 && BorderColor != Color.Transparent && BorderLineStyle != LineStyle.None)
                    gfx.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
    }
}
