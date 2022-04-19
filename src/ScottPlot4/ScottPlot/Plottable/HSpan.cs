using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// Shaded horizontal region between two X values
    /// </summary>
    public class HSpan : AxisSpan
    {
        public double X1 { get => Start; set { Start = value; OnPropertyChanged(); } }
        public double X2 { get => End; set { End = value; OnPropertyChanged(); } }
        public HSpan() : base(true) { base.Dragged += HSpan_Dragged; }

        private void HSpan_Dragged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(X1));
            OnPropertyChanged(nameof(X2));
        }

        public override string ToString() => $"Horizontal span between Y1={X1} and Y2={X2}";
    }
}
