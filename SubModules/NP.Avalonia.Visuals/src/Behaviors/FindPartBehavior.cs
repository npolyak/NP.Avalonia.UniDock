using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class FindPartBehavior
    {
        #region VisualPart Attached Avalonia Property
        public static IControl GetVisualPart(AvaloniaObject obj)
        {
            return obj.GetValue(VisualPartProperty);
        }

        public static void SetVisualPart(AvaloniaObject obj, IControl value)
        {
            obj.SetValue(VisualPartProperty, value);
        }

        public static readonly AttachedProperty<IControl> VisualPartProperty =
            AvaloniaProperty.RegisterAttached<object, Control, IControl>
            (
                "VisualPart"
            );
        #endregion VisualPart Attached Avalonia Property


        #region AncestorObject Attached Avalonia Property
        public static IControl GetAncestorObject(AvaloniaObject obj)
        {
            return obj.GetValue(AncestorObjectProperty);
        }

        public static void SetAncestorObject(AvaloniaObject obj, IControl value)
        {
            obj.SetValue(AncestorObjectProperty, value);
        }

        public static readonly AttachedProperty<IControl> AncestorObjectProperty =
            AvaloniaProperty.RegisterAttached<object, Control, IControl>
            (
                "AncestorObject"
            );
        #endregion AncestorObject Attached Avalonia Property

        static FindPartBehavior()
        {
            AncestorObjectProperty.Changed.Subscribe(OnAncestorObjectChanged);
        }

        private static void OnAncestorObjectChanged(AvaloniaPropertyChangedEventArgs<IControl> args)
        {
            IControl? ancestor = args.NewValue.Value;

            if (ancestor == null)
                return;

            IControl sender = (IControl) args.Sender;

            ancestor.SetValue(VisualPartProperty, sender);
        }
    }
}
