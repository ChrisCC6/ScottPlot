using ScottPlot.MinMaxSearchStrategies;
using System;

namespace ScottPlot.Plottable
{
    /// <summary>
    /// A variation of the SignalPlot optimized for unevenly-spaced ascending X values.
    /// </summary>
    public class SignalPlotXY : SignalPlotXYGeneric<double, double>, ISelectable2DSeries
    {

        public SignalPlotXY() : base()
        {
            Strategy = new LinearDoubleOnlyMinMaxStrategy();
        }

        public override string ToString()
        {
            string label = string.IsNullOrWhiteSpace(this.Label) ? "" : $" ({this.Label})";
            return $"PlottableSignalXY{label} with {PointCount} points";
        }

        public bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY)
        {
            bool test = false;
            for (int i = 0; i < PointCount; i++)
            {
                test = Math.Abs(Ys[i] - coordinateY) <= snapY && Math.Abs(Xs[i] - coordinateX) <= snapX;
                if (test)
                {
                    return test;
                }
            }
            return test;
        }
    }
}
