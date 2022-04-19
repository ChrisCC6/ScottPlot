using ScottPlot.Plottable;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace ScottPlot
{
    public class PlottableCollection : ObservableCollection<IPlottable>
    {
        public PlottableCollection() : base()
        {
        }

        public IEnumerable<IPlottable> ByType(Type type)
        {
            return this.Where(x=>type.IsAssignableFrom(x.GetType()));
        }

    }
}
