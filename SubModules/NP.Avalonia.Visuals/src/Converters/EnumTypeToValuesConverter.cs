// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

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
