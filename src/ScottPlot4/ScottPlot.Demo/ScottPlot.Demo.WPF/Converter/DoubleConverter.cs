using ScottPlot.Plottable;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ScottPlot.Demo.WPF.Converter
{
    public class DoubleConverter : IValueConverter
    {
        internal static ColorConverter Converter { get; } = new ColorConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color mediaColor)
            {
                return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
            }
            else if (value is System.Drawing.Color drawingColor)
            {
                return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color mediaColor)
            {
                return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
            }
            else if (value is System.Drawing.Color drawingColor)
            {
                return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
            }
            return value;
        }
            
        
    }
}
