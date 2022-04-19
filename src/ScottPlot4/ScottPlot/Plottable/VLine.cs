namespace ScottPlot.Plottable
{
    /// <summary>
    /// Vertical line at an X position
    /// </summary>
    public class VLine : AxisLine
    {
        /// <summary>
        /// X position to render the line
        /// </summary>
        public double X { get => Position; set { Position = value; OnPropertyChanged(); } }
        public override string ToString() => $"Vertical line at X={X}";
        public VLine() : base(false) { base.Dragged += VLine_Dragged; }

        private void VLine_Dragged(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(X));
        }
    }
}
