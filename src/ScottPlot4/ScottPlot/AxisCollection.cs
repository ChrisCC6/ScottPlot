using ScottPlot.Renderable;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ScottPlot
{
    public class AxisCollection : ObservableCollection<Axis>
    {
        public AxisCollection() : base()
        {
            Axis defaultLeft = new();
            defaultLeft.Edge = Edge.Left;
            defaultLeft.AxisTicks.MajorGridVisible = true;
            Add(defaultLeft);

            Axis defaultRight = new();
            defaultRight.Edge = Edge.Right;
            defaultRight.AxisIndex = 1;
            defaultRight.AxisTicks.MajorTickVisible = false;
            defaultRight.AxisTicks.TickLabelVisible = false;
            defaultRight.AxisTicks.MinorTickVisible = false;
            defaultRight.AxisTicks.MajorGridVisible = false;
            Add(defaultRight);

            Axis defaultBottom = new();
            defaultBottom.Edge = Edge.Bottom;
            defaultBottom.AxisTicks.MajorGridVisible = true;
            Add(defaultBottom);

            Axis defaultTop = new();
            defaultTop.Edge = Edge.Top;
            defaultTop.AxisIndex = 1;
            defaultTop.AxisTicks.MajorTickVisible = false;
            defaultTop.AxisTicks.TickLabelVisible = false;
            defaultTop.AxisTicks.MinorTickVisible = false;
            defaultTop.AxisTicks.MajorGridVisible = false;
            defaultTop.AxisTicks.MajorGridVisible = false;
            Add(defaultTop);
        }

        public Axis Add(Renderable.Edge edge, int axisIndex, string title = null, Color? color = null)
        {
            if (axisIndex <= 1)
                throw new ArgumentException("The default axes already occupy indexes 0 and 1. Additional axes require higher indexes.");

            Axis axis = new();
            axis.Edge = edge;
            axis.AxisIndex = axisIndex;
            axis.AxisTicks.MajorTickVisible = true;
            axis.AxisTicks.TickLabelVisible = true;
            axis.AxisTicks.MinorTickVisible = true;
            axis.AxisTicks.MajorGridVisible = false;
            axis.AxisLabel.Label = title;

            if (color.HasValue)
            {
                axis.AxisLabel.Font.Color = color.Value;
                axis.AxisTicks.TickLabelFont.Color = color.Value;
                axis.AxisTicks.MajorTickColor = color.Value;
                axis.AxisTicks.MinorTickColor = color.Value;
                axis.AxisLine.Color = color.Value;
            }

            Add(axis);
            return axis;
        }

    }
}
