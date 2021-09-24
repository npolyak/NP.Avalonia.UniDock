using Avalonia.Data.Converters;
using NP.Utilities;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class ToValueContainerConverter : IValueConverter
    {
        public static ToValueContainerConverter Instance { get; } = 
            new ToValueContainerConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return new ValueContainer(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
