using ScottPlot.Plottable;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ScottPlot.Demo.WPF.Converter
{
    public class AxisToStringConverter : IValueConverter
    {
        internal static PlottableToStringConverter Converter { get; } = new PlottableToStringConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            //if (value is AxisLine p)
            //{
            //    return p.GetType().Name;
            //}
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
        
    }
}
