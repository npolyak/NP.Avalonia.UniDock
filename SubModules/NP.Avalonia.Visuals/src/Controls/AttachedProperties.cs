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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace NP.Avalonia.Visuals.Controls
{
    public static class AttachedProperties
    {
        #region MouseOverBrush Attached Avalonia Property
        public static IBrush GetMouseOverBrush(AvaloniaObject obj)
        {
            return obj.GetValue(MouseOverBrushProperty);
        }

        public static void SetMouseOverBrush(AvaloniaObject obj, IBrush value)
        {
            obj.SetValue(MouseOverBrushProperty, value);
        }

        public static readonly AttachedProperty<IBrush> MouseOverBrushProperty =
            AvaloniaProperty.RegisterAttached<object, Control, IBrush>
            (
                "MouseOverBrush"
            );
        #endregion MouseOverBrush Attached Avalonia Property


        #region RealBackground Attached Avalonia Property
        public static IBrush GetRealBackground(AvaloniaObject obj)
        {
            return obj.GetValue(RealBackgroundProperty);
        }

        public static void SetRealBackground(AvaloniaObject obj, IBrush value)
        {
            obj.SetValue(RealBackgroundProperty, value);
        }

        public static readonly AttachedProperty<IBrush> RealBackgroundProperty =
            AvaloniaProperty.RegisterAttached<object, Control, IBrush>
            (
                "RealBackground"
            );
        #endregion RealBackground Attached Avalonia Property


        #region IconData Attached Avalonia Property
        public static Geometry GetIconData(AvaloniaObject obj)
        {
            return obj.GetValue(IconDataProperty);
        }

        public static void SetIconData(AvaloniaObject obj, Geometry value)
        {
            obj.SetValue(IconDataProperty, value);
        }

        public static readonly AttachedProperty<Geometry> IconDataProperty =
            AvaloniaProperty.RegisterAttached<object, Control, Geometry>
            (
                "IconData"
            );
        #endregion IconData Attached Avalonia Property


        #region IconMargin Attached Avalonia Property
        public static Thickness GetIconMargin(AvaloniaObject obj)
        {
            return obj.GetValue(IconMarginProperty);
        }

        public static void SetIconMargin(AvaloniaObject obj, Thickness value)
        {
            obj.SetValue(IconMarginProperty, value);
        }

        public static readonly AttachedProperty<Thickness> IconMarginProperty =
            AvaloniaProperty.RegisterAttached<object, Control, Thickness>
            (
                "IconMargin"
            );
        #endregion IconMargin Attached Avalonia Property


        #region IconWidth Attached Avalonia Property
        public static double GetIconWidth(AvaloniaObject obj)
        {
            return obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(AvaloniaObject obj, double value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        public static readonly AttachedProperty<double> IconWidthProperty =
            AvaloniaProperty.RegisterAttached<object, Control, double>
            (
                "IconWidth"
            );
        #endregion IconWidth Attached Avalonia Property


        #region IconHeight Attached Avalonia Property
        public static double GetIconHeight(AvaloniaObject obj)
        {
            return obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(AvaloniaObject obj, double value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        public static readonly AttachedProperty<double> IconHeightProperty =
            AvaloniaProperty.RegisterAttached<object, Control, double>
            (
                "IconHeight"
            );
        #endregion IconHeight Attached Avalonia Property


        #region IconStretch Attached Avalonia Property
        public static Stretch GetIconStretch(AvaloniaObject obj)
        {
            return obj.GetValue(IconStretchProperty);
        }

        public static void SetIconStretch(AvaloniaObject obj, Stretch value)
        {
            obj.SetValue(IconStretchProperty, value);
        }

        public static readonly AttachedProperty<Stretch> IconStretchProperty =
            AvaloniaProperty.RegisterAttached<object, Control, Stretch>
            (
                "IconStretch"
            );
        #endregion IconStretch Attached Avalonia Property


        #region CurrentScreenPoint Attached Avalonia Property
        public static PixelPoint GetCurrentScreenPoint(AvaloniaObject obj)
        {
            return obj.GetValue(CurrentScreenPointProperty);
        }

        public static void SetCurrentScreenPoint(AvaloniaObject obj, PixelPoint value)
        {
            obj.SetValue(CurrentScreenPointProperty, value);
        }

        public static readonly AttachedProperty<PixelPoint> CurrentScreenPointProperty =
            AvaloniaProperty.RegisterAttached<object, Control, PixelPoint>
            (
                "CurrentScreenPoint"
            );
        #endregion CurrentScreenPoint Attached Avalonia Property
    }
}
