using System;
using System.ComponentModel;
using System.Drawing;

namespace ScottPlot.Plottable
{
    public abstract class BarPlotBase : PropertyNotifier, INotifyPropertyChanged
    {
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        private int xAxisIndex = 0;
        public int XAxisIndex { get => xAxisIndex; set { xAxisIndex = value; OnPropertyChanged(); } }
        private int yAxisIndex = 0;
        public int YAxisIndex { get => yAxisIndex; set { yAxisIndex = value; OnPropertyChanged(); } }

        private Orientation orientation = Orientation.Vertical;
        /// <summary>
        /// Orientation of the bars.
        /// Default behavior is vertical so values are on the Y axis and positions are on the X axis.
        /// </summary>
        public Orientation Orientation { get => orientation; set { orientation = value; OnPropertyChanged(); } }

        private double positionOffset;
        /// <summary>
        /// The position of each bar defines where the left edge of the bar should be.
        /// To center the bar at each position, adjust this value to be negative one-half of the BarWidth.
        /// </summary>
        public double PositionOffset { get => positionOffset; set { positionOffset = value; OnPropertyChanged(); } }

        private double[] values;
        /// <summary>
        /// Size of each bar (along the axis defined by Orientation) relative to ValueBase
        /// </summary>
        public double[] Values { get => values; set { values = value; OnPropertyChanged(); } }

        private double[] positions;
        /// <summary>
        /// Location of the left edge of each bar.
        /// To center bars on these positions, adjust PositionOffset to be negative one-half of the BarWidth.
        /// </summary>
        public double[] Positions { get => positions; set { positions = value; OnPropertyChanged(); } }

        private double[] valueOffsets;
        /// <summary>
        /// This array defines the base of each bar.
        /// Unless the user specifically defines it, this will be an array of zeros.
        /// </summary>
        public double[] ValueOffsets { get => valueOffsets; set { valueOffsets = value; OnPropertyChanged(); } }

        private double[] valueErrors;
        /// <summary>
        /// If populated, this array describes the height of errorbars for each bar
        /// </summary>
        public double[] ValueErrors { get => valueErrors; set { valueErrors = value; OnPropertyChanged(); } }

        private bool showValuesAboveBars;
        /// <summary>
        /// If true, errorbars will be drawn according to the values in the YErrors array
        /// </summary>
        public bool ShowValuesAboveBars { get => showValuesAboveBars; set { showValuesAboveBars = value; OnPropertyChanged(); } }

        /// <summary>
        /// Function to generate the strings placed above each bar based on its value
        /// </summary>
        public Func<double, string> ValueFormatter { get; set; } = x => $"{x}";

        private double valueBase;
        /// <summary>
        /// Bars are drawn from this level and extend according to the sizes defined in Values[]
        /// </summary>
        public double ValueBase { get => valueBase; set { valueBase = value; OnPropertyChanged(); } }

        private double barWidth = .8;
        /// <summary>
        /// Width of bars defined in axis units.
        /// If bars are evenly spaced, consider setting this to a fraction of the distance between the first two Positions.
        /// </summary>
        public double BarWidth { get => barWidth; set { barWidth = value; OnPropertyChanged(); } }

        private double errorCapSize = .4;
        /// <summary>
        /// Width of the errorbar caps defined in axis units.
        /// </summary>
        public double ErrorCapSize { get => errorCapSize; set { errorCapSize = value; OnPropertyChanged(); } }

        private float errorLineWidth = 1;
        /// <summary>
        /// Thickness of the errorbar lines (pixel units)
        /// </summary>
        public float ErrorLineWidth { get => errorLineWidth; set { errorLineWidth = value; OnPropertyChanged(); } }

        private Color borderColor = Color.Black;
        /// <summary>
        /// Outline each bar with this color. 
        /// Set this to transparent to disable outlines.
        /// </summary>
        public Color BorderColor { get => borderColor; set { borderColor = value; OnPropertyChanged(); } }

        private Color errorColor = Color.Black;
        /// <summary>
        /// Color of errorbar lines.
        /// </summary>
        public Color ErrorColor { get => errorColor; set { errorColor = value; OnPropertyChanged(); } }

        private Drawing.Font font;
        /// <summary>
        /// Font settings for labels drawn above the bars
        /// </summary>
        public Drawing.Font Font
        {
            get => font;
            set
            {
                if (font != null)
                    font.PropertyChanged -= Internal_PropertyChanged;
                font = value;
                if (font != null)
                    font.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            }
        }

        private void Internal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
        }

        public BarPlotBase()
        {
            Font = new();
        }

        /// <summary>
        /// Replace the arrays used to define positions and values.
        /// New error arrays will be created and filled with zeros.
        /// </summary>
        public void Replace(double[] positions, double[] values)
        {
            if (positions is null || values is null || positions.Length != values.Length)
                throw new ArgumentException();

            Positions = positions;
            Values = values;
            ValueErrors = new double[values.Length];
            ValueOffsets = new double[values.Length];
        }

        public virtual AxisLimits GetAxisLimits()
        {
            double valueMin = double.PositiveInfinity;
            double valueMax = double.NegativeInfinity;
            double positionMin = double.PositiveInfinity;
            double positionMax = double.NegativeInfinity;

            for (int i = 0; i < Positions.Length; i++)
            {
                valueMin = Math.Min(valueMin, ValueOffsets[i] - ValueErrors[i]);
                valueMax = Math.Max(valueMax, ValueOffsets[i] + ValueErrors[i] + Values[i]);
                positionMin = Math.Min(positionMin, Positions[i]);
                positionMax = Math.Max(positionMax, Positions[i]);
            }

            valueMin = Math.Min(valueMin, ValueBase);
            valueMax = Math.Max(valueMax, ValueBase);

            if (ShowValuesAboveBars)
                valueMax += (valueMax - valueMin) * .1; // increase by 10% to accommodate label

            positionMin -= BarWidth / 2;
            positionMax += BarWidth / 2;

            positionMin += PositionOffset;
            positionMax += PositionOffset;

            return Orientation == Orientation.Vertical ?
                new AxisLimits(positionMin, positionMax, valueMin, valueMax) :
                new AxisLimits(valueMin, valueMax, positionMin, positionMax);
        }

        [Obsolete("Reference the 'Orientation' field instead of this field")]
        public bool VerticalOrientation
        {
            get => Orientation == Orientation.Vertical;
            set => Orientation = value ? Orientation.Vertical : Orientation.Horizontal;
        }

        [Obsolete("Reference the 'Orientation' field instead of this field")]
        public bool HorizontalOrientation
        {
            get => Orientation == Orientation.Horizontal;
            set => Orientation = value ? Orientation.Horizontal : Orientation.Vertical;
        }

        [Obsolete("Reference the 'Values' field instead of this field")]
        public double[] Ys
        {
            get => Values;
            set => Values = value;
        }

        [Obsolete("Reference the 'Positions' field instead of this field")]
        public double[] Xs
        {
            get => Positions;
            set => Positions = value;
        }

        [Obsolete("Reference the 'PositionOffset' field instead of this field", true)]
        public double XOffset
        {
            get => PositionOffset;
            set => PositionOffset = value;
        }
    }
}
