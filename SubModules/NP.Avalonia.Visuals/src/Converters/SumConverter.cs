using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NP.Avalonia.Visuals.Converters
{
    public class SumConverter : IMultiValueConverter
    {
        public static SumConverter Instance { get; } = new SumConverter();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return 0d;

            return values.OfType<double>().Sum();
        }
    }
}
