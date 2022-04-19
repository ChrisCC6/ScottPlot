using ScottPlot.Plottable;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ScottPlot.Demo.WPF.Converter
{
    public class SelectedObjectToPlottableConverter : IValueConverter
    {
        internal static PlottableToStringConverter Converter { get; } = new PlottableToStringConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (!typeof(IPlottable).IsAssignableFrom(value.GetType()))
                return null;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value;
    }
}
