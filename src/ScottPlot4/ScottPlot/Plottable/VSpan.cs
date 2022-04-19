using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// Shade the region between two Y values
    /// </summary>
    public class VSpan : AxisSpan
    {
        public double Y1 { get => Start; set { Start = value; OnPropertyChanged(); } }
        public double Y2 { get => End; set { End = value; OnPropertyChanged(); } }
        public VSpan() : base(false) { base.Dragged += VSpan_Dragged; }

        private void VSpan_Dragged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Y1));
            OnPropertyChanged(nameof(Y2));
        }

        public override string ToString() => $"Vertical span between X1={Y1} and X2={Y2}";
    }
}
