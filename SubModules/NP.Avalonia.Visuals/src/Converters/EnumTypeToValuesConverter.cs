using Avalonia.Data.Converters;
using NP.Utilities;
using System;
using System.Globalization;
using System.Linq;

namespace NP.Avalonia.Visuals.Converters
{
    public class EnumTypeToValuesConverter : IValueConverter
    {
        public static EnumTypeToValuesConverter Instance { get; } =
            new EnumTypeToValuesConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type enumType = (value as Type) ?? targetType;

            string[] enumTypesToExclude =
                parameter?.ToString()?.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            return Enum.GetValues(enumType)
                        .Cast<object>()
                        .Where(enumVal => !enumVal.ToString().IsIn(enumTypesToExclude))
                        .ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
