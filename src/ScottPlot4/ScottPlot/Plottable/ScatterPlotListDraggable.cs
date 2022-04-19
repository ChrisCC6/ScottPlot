﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot.Plottable
{
    public class ScatterPlotListDraggable : ScatterPlotList<double>, IDraggable
    {
        private int IndexUnderMouse = -1;

        private bool dragEnabled = true;
        public bool DragEnabled { get => dragEnabled; set { dragEnabled = value; OnPropertyChanged(); } }

        public Cursor DragCursor => Cursor.Hand;

        public event EventHandler Dragged = delegate { };

        public Func<List<double>, List<double>, int, Coordinate, Coordinate> MovePointFunc = (xs, ys, index, moveTo) => moveTo;

        public void DragTo(double coordinateX, double coordinateY, bool fixedSize)
        {
            if (!DragEnabled || IndexUnderMouse < 0)
                return;

            Coordinate requested = new(coordinateX, coordinateY);
            Coordinate actual = MovePointFunc(Xs.ToList(), Ys.ToList(), IndexUnderMouse, requested);
            Xs[IndexUnderMouse] = actual.X;
            Ys[IndexUnderMouse] = actual.Y;

            Dragged(this, EventArgs.Empty);
        }

        public bool IsUnderMouse(double coordinateX, double coordinateY, double snapX, double snapY)
        {
            for (int i = 0; i < Count; i++)
            {
                double dX = Math.Abs(Convert.ToDouble(Xs[i]) - coordinateX);
                double dY = Math.Abs(Convert.ToDouble(Ys[i]) - coordinateY);

                if (dX <= snapX && dY <= snapY)
                {
                    IndexUnderMouse = i;
                    return true;
                }
            }

            IndexUnderMouse = -1;
            return false;
        }
    }
}
