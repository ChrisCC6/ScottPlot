using ScottPlot.Drawing;
using System;
using System.Linq;
using System.Drawing;
using System.ComponentModel;

namespace ScottPlot.Plottable
{
    public abstract class AxisLineVector : PropertyNotifier, IPlottable, IDraggable, IHasLine, IHasColor
    {
        private double[] positions;
        /// <summary>
        /// Location of the line (Y position if horizontal line, X position if vertical line)
        /// </summary>
        protected double[] Positions { get => positions; set { positions = value; OnPropertyChanged(); OnPropertyChanged(nameof(PointCount)); } }

        public int PointCount => Positions.Length;

        private int currentIndex = 0;
        public int CurrentIndex { get => currentIndex; set { currentIndex = value; OnPropertyChanged(); } }

        private int offset = 0;
        /// <summary>
        /// Add this value to each datapoint value before plotting (axis units)
        /// </summary>
        protected int Offset { get => offset; set { offset = value; OnPropertyChanged(); } }

        private bool positionLabel = false;
        /// <summary>
        /// If True, the position will be labeled on the axis using the PositionFormatter
        /// </summary>
        public bool PositionLabel { get => positionLabel; set { positionLabel = value; OnPropertyChanged(); } }

        private ScottPlot.Drawing.Font positionLabelFont;
        /// <summary>
        /// Font to use for position labels (labels drawn over the axis)
        /// </summary>
        //public Drawing.Font PositionLabelFont = new(){ Color = Color.White, Bold = true };
        public ScottPlot.Drawing.Font PositionLabelFont { 
            get => positionLabelFont; 
            set 
            { 
                if (positionLabelFont != null)
                    positionLabelFont.PropertyChanged -= Internal_PropertyChanged;
                positionLabelFont = value;
                if (positionLabelFont != null)
                    positionLabelFont.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            } 
        }

        private void Internal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {            
            OnPropertyChanged(nameof(sender));
        }

        private Color positionLabelBackground = Color.Black;
        /// <summary>
        /// Color to use behind the position labels
        /// </summary>
        public Color PositionLabelBackground { get => positionLabelBackground; set { positionLabelBackground = value; OnPropertyChanged(); } }

        private bool positionLabelOppositeAxis = false;
        /// <summary>
        /// If true the position label will be drawn on the right or top of the data area.
        /// </summary>
        public bool PositionLabelOppositeAxis { get => positionLabelOppositeAxis; set { positionLabelOppositeAxis = value; OnPropertyChanged(); } }

        /// <summary>
        /// This method generates the position label text for numeric (non-DateTime) axes.
        /// For DateTime axes assign your own format string that uses DateTime.FromOADate(position).
        /// </summary>
        public Func<double, string> PositionFormatter = position => position.ToString("F2");

        // customization
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private Color color = Color.Black;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }
        public Color LineColor { get => Color; set { Color = value; OnPropertyChanged(); } }
        private LineStyle lineStyle = LineStyle.Solid;
        public LineStyle LineStyle { get => lineStyle; set { lineStyle = value; OnPropertyChanged(); } }
        private MarkerShape markerShape = MarkerShape.filledCircle;
        public MarkerShape MarkerShape { get => markerShape; set { markerShape = value; OnPropertyChanged(); } }
        private double lineWidth = 1;
        public double LineWidth { get => lineWidth; set { lineWidth = value; OnPropertyChanged(); } }
        private float errorLineWidth = 1;
        public float ErrorLineWidth { get => errorLineWidth; set { errorLineWidth = value; OnPropertyChanged(); } }
        private float errorCapSize = 3;
        public float ErrorCapSize { get => errorCapSize; set { errorCapSize = value; OnPropertyChanged(); } }
        private float markerSize = 5;
        public float MarkerSize { get => markerSize; set { markerSize = value; OnPropertyChanged(); } }
        private bool stepDisplay = false;
        public bool StepDisplay { get => stepDisplay; set { stepDisplay = value; OnPropertyChanged(); } }

        /// <summary>
        /// Indicates whether the line is horizontal (position in Y units) or vertical (position in X units)
        /// </summary>
        readonly private bool IsHorizontal;

        private bool ignoreAxisAuto = false;
        /// <summary>
        /// If true, AxisAuto() will ignore the position of this line when determining axis limits
        /// </summary>
        public bool IgnoreAxisAuto { get => ignoreAxisAuto; set { ignoreAxisAuto = value; OnPropertyChanged(); } }

        private string label = string.Empty;
        /// <summary>
        /// Text that appears in the legend
        /// </summary>
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        private bool dragEnabled = false;
        /// <summary>
        /// Indicates whether this line is draggable in user controls.
        /// </summary>
        public bool DragEnabled { get => dragEnabled; set { dragEnabled = value; OnPropertyChanged(); } }


        /// <summary>
        /// Cursor to display while hovering over this line if dragging is enabled.
        /// </summary>
        public Cursor DragCursor => IsHorizontal ? Cursor.NS : Cursor.WE;

        private double dragLimitMin = double.NegativeInfinity;
        /// <summary>
        /// If dragging is enabled the line cannot be dragged more negative than this position
        /// </summary>
        public double DragLimitMin { get => dragLimitMin; set { dragLimitMin = value; OnPropertyChanged(); } }

        private double dragLimitMax = double.PositiveInfinity;
        /// <summary>
        /// If dragging is enabled the line cannot be dragged more positive than this position
        /// </summary>
        public double DragLimitMax { get => dragLimitMax; set { dragLimitMax = value; OnPropertyChanged(); } }

        /// <summary>
        /// This event is invoked after the line is dragged
        /// </summary>
        public event EventHandler Dragged = delegate { };


        private double min = double.NegativeInfinity;
        /// <summary>
        /// The lower bound of the axis line.
        /// </summary>
        public double Min { get => min; set { min = value; OnPropertyChanged(); } }

        private double max = double.PositiveInfinity;
        /// <summary>
        /// The upper bound of the axis line.
        /// </summary>
        public double Max { get => max; set { max = value; OnPropertyChanged(); } }
    
        public AxisLineVector(bool isHorizontal)
        {
            IsHorizontal = isHorizontal;
            PositionLabelFont = new ScottPlot.Drawing.Font() { Color = Color.White, Bold = true };
        }

        public AxisLimits GetAxisLimits()
        {
            if (IgnoreAxisAuto)
                return new AxisLimits(double.NaN, double.NaN, double.NaN, double.NaN);

            if (IsHorizontal)
                return new AxisLimits(double.NaN, double.NaN, Positions.Min(), Positions.Max());
            else
                return new AxisLimits(Positions.Min(), Positions.Max(), double.NaN, double.NaN);
        }

        public void ValidateData(bool deep = false)
        {
            Validate.AssertHasElements("Positions", Positions);
            if (deep)
            {
                Validate.AssertAllReal("Positions", Positions);
            }
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (!IsVisible)
                return;

            RenderLine(dims, bmp, lowQuality);

            if (PositionLabel)
                RenderPositionLabel(dims, bmp, lowQuality);
        }

        public void RenderLine(PlotDimensions dims, Bitmap bmp, bool lowQuality)
        {
            var gfx = GDI.Graphics(bmp, dims, lowQuality);
            var pen = GDI.Pen(Color, LineWidth, LineStyle, true);

            if (IsHorizontal)
            {
                for (int i = 0; i < PointCount; i++)
                {
                    float pixelY = dims.GetPixelY(Positions[i] + Offset);

                    double xMin = Math.Max(Min, dims.XMin);
                    double xMax = Math.Min(Max, dims.XMax);
                    float pixelX1 = dims.GetPixelX(xMin);
                    float pixelX2 = dims.GetPixelX(xMax);

                    gfx.DrawLine(pen, pixelX1, pixelY, pixelX2, pixelY);

                }
            }
            else
            {
                for (int i = 0; i < PointCount; i++)
                {
                    float pixelX = dims.GetPixelX(Positions[i] + Offset);

                    double yMin = Math.Max(Min, dims.YMin);
                    double yMax = Math.Min(Max, dims.YMax);
                    float pixelY1 = dims.GetPixelY(yMin);
                    float pixelY2 = dims.GetPixelY(yMax);

                    gfx.DrawLine(pen, pixelX, pixelY1, pixelX, pixelY2);

                }
            }
        }

        private void RenderPositionLabel(PlotDimensions dims, Bitmap bmp, bool lowQuality)
        {
            var gfx = GDI.Graphics(bmp, dims, lowQuality, clipToDataArea: false);
            var pen = GDI.Pen(Color, LineWidth, LineStyle, true);

            var fnt = GDI.Font(PositionLabelFont);
            var fillBrush = GDI.Brush(PositionLabelBackground);
            var fontBrush = GDI.Brush(PositionLabelFont.Color);

            if (IsHorizontal)
            {
                for (int i = 0; i < PointCount; i++)
                {
                    double lineposition = Positions[i] + Offset;
                    if (lineposition > dims.YMax || lineposition < dims.YMin)
                    { }
                    else
                    {

                        float pixelY = dims.GetPixelY(lineposition);
                        string yLabel = PositionFormatter(lineposition);
                        SizeF yLabelSize = GDI.MeasureString(yLabel, PositionLabelFont);
                        float xPos = PositionLabelOppositeAxis ? dims.DataOffsetX + dims.DataWidth : dims.DataOffsetX - yLabelSize.Width;
                        float yPos = pixelY - yLabelSize.Height / 2;
                        RectangleF xLabelRect = new RectangleF(xPos, yPos, yLabelSize.Width, yLabelSize.Height);
                        gfx.FillRectangle(fillBrush, xLabelRect);
                        var sf = GDI.StringFormat(HorizontalAlignment.Left, VerticalAlignment.Middle);
                        gfx.DrawString(yLabel, fnt, fontBrush, xPos, pixelY, sf);
                    }
                }
            }
            else
            {
                for (int i = 0; i < PointCount; i++)
                {
                    double lineposition = Positions[i] + Offset;
                    if (lineposition > dims.XMax || lineposition < dims.XMin)
                    { }
                    else
                    {

                        float pixelX = dims.GetPixelX(lineposition);
                        string xLabel = PositionFormatter(lineposition);
                        SizeF xLabelSize = GDI.MeasureString(xLabel, PositionLabelFont);
                        float xPos = pixelX - xLabelSize.Width / 2;
                        float yPos = PositionLabelOppositeAxis ? dims.DataOffsetY - xLabelSize.Height : dims.DataOffsetY + dims.DataHeight;
                        RectangleF xLabelRect = new RectangleF(xPos, yPos, xLabelSize.Width, xLabelSize.Height);
                        gfx.FillRectangle(fillBrush, xLabelRect);
                        var sf = GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper);
                        gfx.DrawString(xLabel, fnt, fontBrush, pixelX, yPos, sf);
                    }
                }
            }
        }

        /// <summary>
        /// Move the reference line to a new coordinate in plot space.
        /// </summary>
        /// <param name="coordinateX">new X position</param>
        /// <param name="coordinateY">new Y position</param>
        /// <param name="fixedSize">This argument is ignored</param>
        public void DragTo(double coordinateX, double coordinateY, bool fixedSize)
        {
            if (!DragEnabled)
                return;

            if (IsHorizontal)
            {
                if (coordinateY < DragLimitMin) coordinateY = DragLimitMin;
                if (coordinateY > DragLimitMax) coordinateY = DragLimitMax;
                Positions[CurrentIndex] = coordinateY;
            }
            else
            {
                if (coordinateX < DragLimitMin) coordinateX = DragLimitMin;
                if (coordinateX > DragLimitMax) coordinateX = DragLimitMax;
                Positions[CurrentIndex] = coordinateX;
            }
            OnPropertyChanged(nameof(Positions));
            Dragged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return True if the reference line is within a certain number of pixels (snap) to the mouse
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
                for (int i = 0; i < PointCount; i++)
                {
                    if (Math.Abs(Positions[i] - coordinateY) <= snapY)
                    {
                        CurrentIndex = i;
                        return true;
                    }
                };
            }
            else
            {
                for (int i = 0; i < PointCount; i++)
                {
                    if (Math.Abs(Positions[i] - coordinateX) <= snapX)
                    {
                        CurrentIndex = i;
                        return true;
                    }
                }
            }
            return false;
        }

        public LegendItem[] GetLegendItems()
        {
            var singleItem = new LegendItem(this)
            {
                Label = this.Label,
                Color = this.Color,
                LineStyle = this.LineStyle,
                LineWidth = this.LineWidth,
                MarkerShape = MarkerShape.none
            };
            return new LegendItem[] { singleItem };
        }
    }

    /// <summary>
    /// Vertical line at an X position
    /// </summary>
    public class VLineVector : AxisLineVector
    {
        /// <summary>
        /// X position to render the line
        /// </summary>
        public double[] Xs { get => Positions; set { Positions = value; OnPropertyChanged(); } }

        public new int Offset { get => base.Offset; set { base.Offset = value; OnPropertyChanged(); } }

        public override string ToString() => $"{PointCount} lines positions X={Xs}, with an offset of {Offset}";
        public VLineVector() : base(false) { }
    }

    /// <summary>
    /// Horizontal line at an Y position
    /// </summary>
    public class HLineVector : AxisLineVector
    {
        /// <summary>
        /// Y position to render the line
        /// </summary>
        public double[] Ys { get => Positions; set { Positions = value; OnPropertyChanged(); } }

        public new int Offset { get => base.Offset; set { base.Offset = value; OnPropertyChanged(); } }

        public override string ToString() => $"{PointCount} lines positions Y={Ys}, with an offset of {Offset}";
        public HLineVector() : base(true) { }
    }
}
