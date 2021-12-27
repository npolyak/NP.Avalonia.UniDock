using Avalonia;
using Avalonia.Controls.Primitives;

namespace NP.Avalonia.Visuals.Controls
{
    public class PieSegment : TemplatedControl
    {
        #region InnerRadius Styled Avalonia Property
        public double InnerRadius
        {
            get { return GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly StyledProperty<double> InnerRadiusProperty =
            AvaloniaProperty.Register<PieSegment, double>
            (
                nameof(InnerRadius)
            );
        #endregion InnerRadius Styled Avalonia Property

        #region OuterRadius Styled Avalonia Property
        public double OuterRadius
        {
            get { return GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }

        public static readonly StyledProperty<double> OuterRadiusProperty =
            AvaloniaProperty.Register<PieSegment, double>
            (
                nameof(OuterRadius)
            );
        #endregion OuterRadius Styled Avalonia Property

        #region StartAngle Styled Avalonia Property
        public double StartAngle
        {
            get { return GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly StyledProperty<double> StartAngleProperty =
            AvaloniaProperty.Register<PieSegment, double>
            (
                nameof(StartAngle)
            );
        #endregion StartAngle Styled Avalonia Property

        #region EndAngle Styled Avalonia Property
        public double EndAngle
        {
            get { return GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        public static readonly StyledProperty<double> EndAngleProperty =
            AvaloniaProperty.Register<PieSegment, double>
            (
                nameof(EndAngle)
            );
        #endregion EndAngle Styled Avalonia Property
    }
}
