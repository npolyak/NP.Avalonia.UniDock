using Avalonia.Data.Converters;
using NP.Utilities;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class FromControlContainerConverter : IValueConverter
    {
        public static FromControlContainerConverter Instance { get; } =
            new FromControlContainerConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ControlContainer controlContainer)
                return controlContainer.Control;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
