using System.ComponentModel;

namespace ScottPlot.Plottable
{
    public interface ISelectable
    {
        bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY);
        //(double x, double y, int index) GetPointNearestX(double x);

    }
}
