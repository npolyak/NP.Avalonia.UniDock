using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using NP.Avalonia.Visuals.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class VisualFlowConverter<T> : IValueConverter, IMultiValueConverter
    {
        public T NormalValue { get; }

        public T ReverseValue { get; }

        public VisualFlowConverter(T normal, T reverse)
        {
            NormalValue = normal;

            ReverseValue = reverse;
        }

        private object Convert(object value)
        {
            if (value is VisualFlow visualFlow)
            {
                if (visualFlow == VisualFlow.Reverse)
                    return ReverseValue;
            }

            return NormalValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count == 0)
                return null;

            return Convert(values[0]);
        }
    }

    public class VisualFlowToScalingFactorConverter : VisualFlowConverter<double>
    {
        public static VisualFlowToScalingFactorConverter Instance { get; } = new VisualFlowToScalingFactorConverter();

        public VisualFlowToScalingFactorConverter() : base(1, -1)
        {

        }
    }

    public class VisualFlowToTextAlignmentConverter : VisualFlowConverter<TextAlignment>
    {
        public static VisualFlowToTextAlignmentConverter Instance { get; } = new VisualFlowToTextAlignmentConverter();

        public VisualFlowToTextAlignmentConverter() : base(TextAlignment.Left, TextAlignment.Right)
        {

        }
    }

    public class VisualFlowToHorizontalAlignmentConverter : VisualFlowConverter<HorizontalAlignment>
    {
        public static VisualFlowToHorizontalAlignmentConverter Instance { get; } = new VisualFlowToHorizontalAlignmentConverter();

        public VisualFlowToHorizontalAlignmentConverter() : base(HorizontalAlignment.Left, HorizontalAlignment.Right)
        {

        }
    }
}
