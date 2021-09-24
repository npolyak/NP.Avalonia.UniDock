using Avalonia.Controls;
using Avalonia.Data.Converters;
using NP.Utilities;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class ToControlContainerConverter : IValueConverter
    {
        public static ToControlContainerConverter Instance { get; } = 
            new ToControlContainerConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IControl control = value as IControl;

            if (control == null)
                return value;

            return new ControlContainer(control);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
