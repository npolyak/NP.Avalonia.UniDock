using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;

namespace NP.Avalonia.Visuals.Controls
{
    public class LabeledControl : TemplatedControl
    {
        #region Text Styled Avalonia Property
        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<LabeledControl, string>
            (
                nameof(Text)
            );
        #endregion Text Styled Avalonia Property


        #region TheTextWrapping Styled Avalonia Property
        public TextWrapping TheTextWrapping
        {
            get { return GetValue(TheTextWrappingProperty); }
            set { SetValue(TheTextWrappingProperty, value); }
        }

        public static readonly StyledProperty<TextWrapping> TheTextWrappingProperty =
            AvaloniaProperty.Register<LabeledControl, TextWrapping>
            (
                nameof(TheTextWrapping)
            );
        #endregion TheTextWrapping Styled Avalonia Property


        #region TextClasses Styled Avalonia Property
        public string TextClasses
        {
            get { return GetValue(TextClassesProperty); }
            set { SetValue(TextClassesProperty, value); }
        }

        public static readonly StyledProperty<string> TextClassesProperty =
            AvaloniaProperty.Register<LabeledControl, string>
            (
                nameof(TextClasses)
            );
        #endregion TextClasses Styled Avalonia Property


        #region TextWidth Styled Avalonia Property
        public double TextWidth
        {
            get { return GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }

        public static readonly StyledProperty<double> TextWidthProperty =
            AvaloniaProperty.Register<LabeledControl, double>
            (
                nameof(TextWidth),
                double.NaN
            );
        #endregion TextWidth Styled Avalonia Property


        #region MaxTextWidth Styled Avalonia Property
        public double MaxTextWidth
        {
            get { return GetValue(MaxTextWidthProperty); }
            set { SetValue(MaxTextWidthProperty, value); }
        }

        public static readonly StyledProperty<double> MaxTextWidthProperty =
            AvaloniaProperty.Register<LabeledControl, double>
            (
                nameof(MaxTextWidth),
                double.PositiveInfinity
            );
        #endregion MaxTextWidth Styled Avalonia Property



        #region ContainedControlTemplate Styled Avalonia Property
        public ControlTemplate ContainedControlTemplate
        {
            get { return GetValue(ContainedControlTemplateProperty); }
            set { SetValue(ContainedControlTemplateProperty, value); }
        }

        public static readonly StyledProperty<ControlTemplate> ContainedControlTemplateProperty =
            AvaloniaProperty.Register<LabeledControl, ControlTemplate>
            (
                nameof(ContainedControlTemplate)
            );
        #endregion ContainedControlTemplate Styled Avalonia Property


        #region HorizontalContainedControlAlignment Styled Avalonia Property
        public HorizontalAlignment HorizontalContainedControlAlignment
        {
            get { return GetValue(HorizontalContainedControlAlignmentProperty); }
            set { SetValue(HorizontalContainedControlAlignmentProperty, value); }
        }


        #region HorizontalTextAlignment Styled Avalonia Property
        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> HorizontalTextAlignmentProperty =
            AvaloniaProperty.Register<LabeledControl, HorizontalAlignment>
            (
                nameof(HorizontalTextAlignment),
                HorizontalAlignment.Left
            );
        #endregion HorizontalTextAlignment Styled Avalonia Property


        #region VerticalTextAlignment Styled Avalonia Property
        public VerticalAlignment VerticalTextAlignment
        {
            get { return GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalTextAlignmentProperty =
            AvaloniaProperty.Register<LabeledControl, VerticalAlignment>
            (
                nameof(VerticalTextAlignment)
            );
        #endregion VerticalTextAlignment Styled Avalonia Property


        public static readonly StyledProperty<HorizontalAlignment> HorizontalContainedControlAlignmentProperty =
            AvaloniaProperty.Register<LabeledControl, HorizontalAlignment>
            (
                nameof(HorizontalContainedControlAlignment)
            );
        #endregion HorizontalContainedControlAlignment Styled Avalonia Property

        #region VerticalContainedControlAlignment Styled Avalonia Property
        public VerticalAlignment VerticalContainedControlAlignment
        {
            get { return GetValue(VerticalContainedControlAlignmentProperty); }
            set { SetValue(VerticalContainedControlAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalContainedControlAlignmentProperty =
            AvaloniaProperty.Register<LabeledControl, VerticalAlignment>
            (
                nameof(VerticalContainedControlAlignment)
            );
        #endregion VerticalContainedControlAlignment Styled Avalonia Property


        #region ControlRow Styled Avalonia Property
        public int ControlRow
        {
            get { return GetValue(ControlRowProperty); }
            set { SetValue(ControlRowProperty, value); }
        }

        public static readonly StyledProperty<int> ControlRowProperty =
            AvaloniaProperty.Register<LabeledControl, int>
            (
                nameof(ControlRow)
            );
        #endregion ControlRow Styled Avalonia Property


        #region ControlColumn Styled Avalonia Property
        public int ControlColumn
        {
            get { return GetValue(ControlColumnProperty); }
            set { SetValue(ControlColumnProperty, value); }
        }

        public static readonly StyledProperty<int> ControlColumnProperty =
            AvaloniaProperty.Register<LabeledControl, int>
            (
                nameof(ControlColumn)
            );
        #endregion ControlColumn Styled Avalonia Property

    }
}
