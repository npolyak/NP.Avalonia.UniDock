using Avalonia.Data.Converters;
using Avalonia.Media;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NP.Avalonia.Visuals.Converters
{
    public class LastNonDefaultValueConverter<T> : IMultiValueConverter
    {
        public T? DefaultValue { get; set; } = default(T);

        public object? Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return DefaultValue;

            T? lastValue = values.OfType<T>().LastOrDefault(v => !v.ObjEquals(DefaultValue));

            if (lastValue == null)
            {
                return DefaultValue;
            }

            return lastValue;
        }
    }

    public class LastNonDefaultDoubleConverter : LastNonDefaultValueConverter<double> { }


    public class LastNonDefaultColorConverter : LastNonDefaultValueConverter<Color> { }

    public class LastNonDefaultBrushConverter : LastNonDefaultValueConverter<IBrush> { }
}
