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
//
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace NP.Avalonia.Visuals.Converters
{
    public class ToWindowIconConverter : IValueConverter
    {
        public static ToWindowIconConverter Instance { get; } = new ToWindowIconConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IBitmap bitmap)
            {
                return new WindowIcon(bitmap);
            }

            if (value is string fileName)
            {
                return new WindowIcon(fileName);
            }

            if (value is Stream stream)
            {
                return new WindowIcon(stream);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
