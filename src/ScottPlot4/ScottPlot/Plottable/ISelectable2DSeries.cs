using System.ComponentModel;

namespace ScottPlot.Plottable
{
    public interface ISelectable2DSeries
    {
        bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY);
        //(double x, double y, int index) GetPointNearestX(double x);
        int XAxisIndex { get; set; }
        int YAxisIndex { get; set; }
    }
}
