using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NP.Avalonia.Visuals.Converters
{
    public class StringFormatConverter : IMultiValueConverter
    {
        public static StringFormatConverter Instance { get; } = 
            new StringFormatConverter();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count == 0)
                return null;

            string str = values[0].ToString();

            if (values.Count == 1)
                return str;

            object[] args = values.Skip(1).ToArray();

            if (args.Any(a => a == null))
                return null;

            return string.Format(str, values.Skip(1).ToArray());
        }
    }
}
