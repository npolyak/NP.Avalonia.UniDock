using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class AndConverter : IMultiValueConverter
    {
        public static AndConverter Instance { get; } = new AndConverter();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(object val in values)
            {
                if (val is bool b)
                {
                    if (!b)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
