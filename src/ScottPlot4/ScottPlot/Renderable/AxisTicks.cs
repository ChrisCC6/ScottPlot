/* The AxisTicks object contains:
 *   - A TickCollection responsible for calculating tick positions and labels
 *   - major tick label styling
 *   - major/minor tick mark styling
 *   - major/minor grid line styling
 */

using ScottPlot.Ticks;
using ScottPlot.Drawing;
using System;
using System.Drawing;
using System.Linq;
using System.ComponentModel;

namespace ScottPlot.Renderable
{
    public class AxisTicks : PropertyNotifier, IRenderable, INotifyPropertyChanged
    {
        // the tick collection determines where ticks should go and what tick labels should say
        public readonly TickCollection TickCollection = new TickCollection();

        // tick label styling
        private bool tickLabelVisible = true;
        public bool TickLabelVisible { get => tickLabelVisible; set { tickLabelVisible = value; OnPropertyChanged(); } }
        private float tickLabelRotation = 0;
        public float TickLabelRotation { get => tickLabelRotation; set { tickLabelRotation = value; OnPropertyChanged(); } }
        private Drawing.Font tickLabelFont;
        public Drawing.Font TickLabelFont
        {
            get => tickLabelFont;
            set
            {
                if (tickLabelFont != null)
                    tickLabelFont.PropertyChanged -= Internal_PropertyChanged;
                tickLabelFont = value;
                if (tickLabelFont != null)
                    tickLabelFont.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            }
        }
        private void Internal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
        }
        private bool ticksExtendOutward = true;
        public bool TicksExtendOutward { get => ticksExtendOutward; set { ticksExtendOutward = value; OnPropertyChanged(); } }

        // major tick/grid styling
        private bool majorTickVisible = true;
        public bool MajorTickVisible { get => majorTickVisible; set { majorTickVisible = value; OnPropertyChanged(); } }
        private float majorTickLength = 5;
        public float MajorTickLength { get => majorTickLength; set { majorTickLength = value; OnPropertyChanged(); } }
        private Color majorTickColor = Color.Black;
        public Color MajorTickColor { get => majorTickColor; set { majorTickColor = value; OnPropertyChanged(); } }
        private bool majorGridVisible = false;
        public bool MajorGridVisible { get => majorGridVisible; set { majorGridVisible = value; OnPropertyChanged(); } }
        private LineStyle majorGridStyle = LineStyle.Solid;
        public LineStyle MajorGridStyle { get => majorGridStyle; set { majorGridStyle = value; OnPropertyChanged(); } }
        private Color majorGridColor = ColorTranslator.FromHtml("#efefef");
        public Color MajorGridColor { get => majorGridColor; set { majorGridColor = value; OnPropertyChanged(); } }
        private float majorGridWidth = 1;
        public float MajorGridWidth { get => majorGridWidth; set { majorGridWidth = value; OnPropertyChanged(); } }

        // minor tick/grid styling
        private bool minorTickVisible = true;
        public bool MinorTickVisible { get => minorTickVisible; set { minorTickVisible = value; OnPropertyChanged(); } }
        private float minorTickLength = 2;
        public float MinorTickLength { get => minorTickLength; set { minorTickLength = value; OnPropertyChanged(); } }
        private Color minorTickColor = Color.Black;
        public Color MinorTickColor { get => minorTickColor; set { minorTickColor = value; OnPropertyChanged(); } }
        private bool minorGridVisible = false;
        public bool MinorGridVisible { get => minorGridVisible; set { minorGridVisible = value; OnPropertyChanged(); } }
        private LineStyle minorGridStyle = LineStyle.Solid;
        public LineStyle MinorGridStyle { get => minorGridStyle; set { minorGridStyle = value; OnPropertyChanged(); } }
        private Color minorGridColor = ColorTranslator.FromHtml("#efefef");
        public Color MinorGridColor { get => minorGridColor; set { minorGridColor = value; OnPropertyChanged(); } }
        private float minorGridWidth = 1;
        public float MinorGridWidth { get => minorGridWidth; set { minorGridWidth = value; OnPropertyChanged(); } }

        // misc configuration
        private Edge edge;
        public Edge Edge { get => edge; set { edge = value; OnPropertyChanged(); } }
        public bool IsHorizontal => Edge == Edge.Top || Edge == Edge.Bottom;
        public bool IsVertical => Edge == Edge.Left || Edge == Edge.Right;

        private bool isVisibl = true;
        public bool IsVisible { get => isVisibl; set { isVisibl = value; OnPropertyChanged(); } }

        private bool rulerMode = false;
        public bool RulerMode { get => rulerMode; set { rulerMode = value; OnPropertyChanged(); } }

        private bool snapPx = true;
        /// <summary>
        /// If true, grid lines will be drawn with anti-aliasing off to give the appearance of "snapping"
        /// to the nearest pixel and to avoid blurriness associated with drawing single-pixel anti-aliased lines.
        /// </summary>
        public bool SnapPx { get => snapPx; set { snapPx = value; OnPropertyChanged(); } }

        private float pixelOffset = 0;
        public float PixelOffset { get => pixelOffset; set { if (pixelOffset == value) return; pixelOffset = value; OnPropertyChanged(); } }

        // TODO: store the TickCollection in the Axis module, not in the Ticks module.
        // https://github.com/ScottPlot/ScottPlot/pull/1647

        public AxisTicks()
        {
            TickLabelFont = new Drawing.Font() { Size = 11 };
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (!IsVisible)
                return;

            double[] majorTicks = TickCollection.GetVisibleMajorTicks(dims)
                .Select(t => t.Position)
                .ToArray();

            double[] minorTicks = TickCollection.GetVisibleMinorTicks(dims)
                .Select(t => t.Position)
                .ToArray();

            RenderTicksAndGridLines(dims, bmp, lowQuality || SnapPx, majorTicks, minorTicks);
            RenderTickLabels(dims, bmp, lowQuality);
        }

        private void RenderTicksAndGridLines(PlotDimensions dims, Bitmap bmp, bool lowQuality, double[] visibleMajorTicks, double[] visibleMinorTicks)
        {
            using Graphics gfx = GDI.Graphics(bmp, dims, lowQuality, false);

            if (MajorGridVisible)
                AxisTicksRender.RenderGridLines(dims, gfx, visibleMajorTicks, MajorGridStyle, MajorGridColor, MajorGridWidth, Edge);

            if (MinorGridVisible)
                AxisTicksRender.RenderGridLines(dims, gfx, visibleMinorTicks, MinorGridStyle, MinorGridColor, MinorGridWidth, Edge);

            if (MinorTickVisible)
            {
                float tickLength = TicksExtendOutward ? MinorTickLength : -MinorTickLength;
                AxisTicksRender.RenderTickMarks(dims, gfx, visibleMinorTicks, tickLength, MinorTickColor, Edge, PixelOffset);
            }

            if (MajorTickVisible)
            {
                float tickLength = MajorTickLength;

                if (RulerMode)
                    tickLength *= 4;

                tickLength = TicksExtendOutward ? tickLength : -tickLength;

                AxisTicksRender.RenderTickMarks(dims, gfx, visibleMajorTicks, tickLength, MajorTickColor, Edge, PixelOffset);
            }
        }

        private void RenderTickLabels(PlotDimensions dims, Bitmap bmp, bool lowQuality)
        {
            using Graphics gfx = GDI.Graphics(bmp, dims, lowQuality, false);

            if (TickLabelVisible)
                AxisTicksRender.RenderTickLabels(dims, gfx, TickCollection, TickLabelFont, Edge, TickLabelRotation, RulerMode, PixelOffset, MajorTickLength, MinorTickLength);
        }
    }
}
