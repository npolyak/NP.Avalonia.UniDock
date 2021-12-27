using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NP.Avalonia.Visuals.Converters
{
    public class IntSumConverter : IMultiValueConverter
    {
        public static IntSumConverter Instance { get; } = new IntSumConverter();

        public object? Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            return values?.OfType<int>().Sum();
        }
    }
}
