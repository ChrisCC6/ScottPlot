using ScottPlot.Drawing;
using System;
using System.Drawing;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// This class describes a single item that appears in the figure legend.
    /// </summary>
    public class LegendItem : PropertyNotifier
    {
        private string label { get; set; } = string.Empty;
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        private Color color;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }

        private Color hatchColor;
        public Color HatchColor { get => hatchColor; set { hatchColor = value; OnPropertyChanged(); } }

        private Color borderColor;
        public Color BorderColor { get => borderColor; set { borderColor = value; OnPropertyChanged(); } }

        private float borderWith;
        public float BorderWith { get => borderWith; set { borderWith = value; OnPropertyChanged(); } }

        private LineStyle borderLineStyle = LineStyle.Solid;
        public LineStyle BorderLineStyle { get => borderLineStyle; set { borderLineStyle = value; OnPropertyChanged(); } }

        private LineStyle lineStyle = LineStyle.Solid;
        public LineStyle LineStyle { get => lineStyle; set { lineStyle = value; OnPropertyChanged(); } }

        private double lineWidth = 0;
        public double LineWidth
        {
            get => (Parent is IHasLine parent) ? Math.Min(parent.LineWidth, 10) : lineWidth;
            set { lineWidth = value; OnPropertyChanged(); }
        }

        public Color LineColor => Parent is IHasLine p ? p.LineColor : color;

        private MarkerShape markerShape;
        public MarkerShape MarkerShape { get => markerShape; set { markerShape = value; OnPropertyChanged(); } }

        private float markerSize = 0;
        public float MarkerSize
        {
            get => (Parent is IHasMarker parent) ? parent.MarkerSize : markerSize;
            set { markerSize = value; OnPropertyChanged(); }
        }

        public float MarkerLineWidth =>
            Parent is IHasMarker parent ? Math.Min(parent.MarkerLineWidth, 3) : (float)LineWidth;

        public Color MarkerColor =>
            Parent is IHasMarker parent ? parent.MarkerColor : color;

        private HatchStyle hatchStyle;
        public HatchStyle HatchStyle { get => hatchStyle; set { hatchStyle = value; OnPropertyChanged(); } }

        public bool ShowAsRectangleInLegend
        {
            get
            {
                bool hasVeryLargeLineWidth = LineWidth >= 10;
                bool hasArea = (Parent is not null) && (Parent is IHasArea);
                return hasVeryLargeLineWidth || hasArea;
            }
        }

        public readonly IPlottable Parent;

        public LegendItem(IPlottable parent)
        {
            Parent = parent;
        }
    }
}
