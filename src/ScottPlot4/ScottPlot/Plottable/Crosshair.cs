using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// The Crosshair plot type draws a vertical and horizontal line to label a point
    /// on the plot and displays the coordinates of that point in labels that overlap
    /// the axis tick labels. 
    /// 
    /// This plot type is typically used in combination with
    /// MouseMove events to track the location of the mouse and/or with plot types that
    /// have GetPointNearest() methods.
    /// </summary>
    public class Crosshair : PropertyNotifier, IPlottable, IHasLine, IHasColor
    {
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private HLine horizontalLine;
        public HLine HorizontalLine
        {
            get => horizontalLine;
            set
            {
                if (horizontalLine != null)
                    horizontalLine.PropertyChanged -= Internal_PropertyChanged;
                horizontalLine = value;
                if (horizontalLine != null)
                    horizontalLine.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            }
        }

        private VLine verticalLine;
        public VLine VerticalLine
        { 
            get => verticalLine;
            set 
            {
                if (verticalLine != null)
                    verticalLine.PropertyChanged -= Internal_PropertyChanged;
                verticalLine = value;
                if (verticalLine != null)
                    verticalLine.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            }
        }

        private void Internal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
        }

        /// <summary>
        /// X position (axis units) of the vertical line
        /// </summary>
        public double X { get => VerticalLine.X; set { VerticalLine.X = value; OnPropertyChanged(); } }

        /// <summary>
        /// X position (axis units) of the horizontal line
        /// </summary>
        public double Y { get => HorizontalLine.Y; set { HorizontalLine.Y = value; OnPropertyChanged(); } }

        /// <summary>
        /// Sets style for horizontal and vertical lines
        /// </summary>
        public LineStyle LineStyle
        {
            set
            {
                HorizontalLine.LineStyle = value;
                VerticalLine.LineStyle = value;
                OnPropertyChanged();
            }
            get => HorizontalLine.LineStyle;
        }

        /// <summary>
        /// Sets the line width for vertical and horizontal lines
        /// </summary>
        public double LineWidth
        {
            set
            {
                HorizontalLine.LineWidth = value;
                VerticalLine.LineWidth = value;
                OnPropertyChanged();
            }
            get => (float)HorizontalLine.LineWidth;
        }

        /// <summary>
        /// Sets font of the position labels for horizontal and vertical lines
        /// </summary>
        public Drawing.Font LabelFont
        {
            set
            {
                HorizontalLine.PositionLabelFont = value;
                VerticalLine.PositionLabelFont = value;
                OnPropertyChanged();
            }
            [Obsolete("The get method only remain for the compatibility. Get HorizontalLine.PositionLabelFont and VerticalLine.PositionLabelFont instead.")]
            get => HorizontalLine.PositionLabelFont;
        }

        /// <summary>
        /// Sets background color of the position labels for horizontal and vertical lines
        /// </summary>
        public Color LabelBackgroundColor
        {
            set
            {
                HorizontalLine.PositionLabelBackground = value;
                VerticalLine.PositionLabelBackground = value;
                OnPropertyChanged();
            }
            [Obsolete("The get method only remain for the compatibility. Get HorizontalLine.PositionLabelBackground and VerticalLine.PositionLabelBackground instead.")]
            get => HorizontalLine.PositionLabelBackground;
        }

        /// <summary>
        /// Sets visibility of the text labels for each line drawn over the axis
        /// </summary>
        public bool PositionLabel
        {
            set
            {
                HorizontalLine.PositionLabel = value;
                VerticalLine.PositionLabel = value;
                OnPropertyChanged();
            }
            get => HorizontalLine.PositionLabel;
        }

        /// <summary>
        /// Sets color for horizontal and vertical lines and their position label backgrounds
        /// </summary>
        public Color Color
        {
            set
            {
                HorizontalLine.Color = value;
                VerticalLine.Color = value;
                HorizontalLine.PositionLabelBackground = value;
                VerticalLine.PositionLabelBackground = value;
                OnPropertyChanged();
            }
            get
            {
                return HorizontalLine.Color;
            }
        }

        public Color LineColor { get => Color; set { Color = value; OnPropertyChanged(); } }

        public Crosshair()
        {
            HorizontalLine = new();
            VerticalLine = new();
            LineStyle = LineStyle.Dash;
            LineWidth = 1;
            Color = Color.FromArgb(200, Color.Red);
            PositionLabel = true;
        }

        public AxisLimits GetAxisLimits() => new(double.NaN, double.NaN, double.NaN, double.NaN);

        public LegendItem[] GetLegendItems() => null;

        public void ValidateData(bool deep = false) { }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (IsVisible == false)
                return;

            HorizontalLine.Render(dims, bmp, lowQuality);
            VerticalLine.Render(dims, bmp, lowQuality);
        }

        [Obsolete("Use VerticalLine.PositionFormatter()")]
        public bool IsDateTimeX
        {
            get => isDateTimeX;
            set
            {
                isDateTimeX = value;
                VerticalLine.PositionFormatter = value ?
                    position => DateTime.FromOADate(position).ToString(stringFormatX) :
                    position => position.ToString(stringFormatX);
            }
        }

        [Obsolete]
        private bool isDateTimeX = false;

        [Obsolete("Use VerticalLine.PositionFormatter()")]
        public string StringFormatX
        {
            get => stringFormatX;
            set
            {
                stringFormatX = value;
                VerticalLine.PositionFormatter = isDateTimeX ?
                    position => DateTime.FromOADate(position).ToString(stringFormatX) :
                    position => position.ToString(stringFormatX);
            }
        }

        [Obsolete]
        private string stringFormatX = "F2";

        [Obsolete("Use VerticalLine.IsVisible")]
        public bool IsVisibleX
        {
            get => VerticalLine.IsVisible;
            set => VerticalLine.IsVisible = value;
        }

        [Obsolete("Use HorizontalLine.PositionFormatter()")]
        public bool IsDateTimeY
        {
            get => isDateTimeY;
            set
            {
                isDateTimeY = value;
                HorizontalLine.PositionFormatter = value ?
                    position => DateTime.FromOADate(position).ToString(stringFormatY) :
                    (position) => position.ToString(stringFormatY);
            }
        }

        [Obsolete]
        private bool isDateTimeY = false;

        [Obsolete("Use HorizontalLine.PositionFormat()")]
        public string StringFormatY
        {
            get => stringFormatY;
            set
            {
                stringFormatY = value;
                HorizontalLine.PositionFormatter = isDateTimeY ?
                    position => DateTime.FromOADate(position).ToString(stringFormatY) :
                    position => position.ToString(stringFormatY);
            }
        }

        [Obsolete]
        private string stringFormatY = "F2";

        [Obsolete("Use HorizontalLine.IsVisible")]
        public bool IsVisibleY
        {
            get => HorizontalLine.IsVisible;
            set => HorizontalLine.IsVisible = value;
        }
    }
}
