using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class ShiftFromTopLeftMarginConverter : IValueConverter
    {
        public static ShiftFromTopLeftMarginConverter TheInstance { get; } =
       new ShiftFromTopLeftMarginConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
                return new Thickness();

            double doubleVal = (double)value;

            return new Thickness(doubleVal, doubleVal, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
