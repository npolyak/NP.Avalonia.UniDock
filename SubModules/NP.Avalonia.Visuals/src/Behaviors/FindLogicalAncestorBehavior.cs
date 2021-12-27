using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using System;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class FindLogicalAncestorBehavior
    {
        #region AncestorType Attached Avalonia Property
        public static Type? GetAncestorType(IControl obj)
        {
            return obj.GetValue(AncestorTypeProperty);
        }

        public static void SetAncestorType(IControl obj, Type? value)
        {
            obj.SetValue(AncestorTypeProperty, value);
        }

        public static readonly AttachedProperty<Type?> AncestorTypeProperty =
            AvaloniaProperty.RegisterAttached<FindLogicalAncestorBehavior, IControl, Type?>
            (
                "AncestorType"
            );
        #endregion AncestorType Attached Avalonia Property


        #region AncestorName Attached Avalonia Property
        public static string? GetAncestorName(IControl obj)
        {
            return obj.GetValue(AncestorNameProperty);
        }

        public static void SetAncestorName(IControl obj, string? value)
        {
            obj.SetValue(AncestorNameProperty, value);
        }

        public static readonly AttachedProperty<string?> AncestorNameProperty =
            AvaloniaProperty.RegisterAttached<FindLogicalAncestorBehavior, IControl, string?>
            (
                "AncestorName"
            );
        #endregion AncestorName Attached Avalonia Property


        #region Ancestor Attached Avalonia Property
        public static IControl? GetAncestor(IControl obj)
        {
            return obj.GetValue(AncestorProperty);
        }

        private static void SetAncestor(IControl obj, IControl? value)
        {
            obj.SetValue(AncestorProperty, value);
        }

        public static readonly AttachedProperty<IControl?> AncestorProperty =
            AvaloniaProperty.RegisterAttached<FindLogicalAncestorBehavior, IControl, IControl?>
            (
                "Ancestor"
            );
        #endregion Ancestor Attached Avalonia Property


        static FindLogicalAncestorBehavior()
        {
            AncestorTypeProperty.Changed.Subscribe(OnTypeChanged!);
            AncestorNameProperty.Changed.Subscribe(OnNameChanged!);
        }

        private static void OnTypeChanged(AvaloniaPropertyChangedEventArgs<Type> args)
        {
            SetFinding((IControl)args.Sender);
        }

        private static void OnNameChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {

            SetFinding((IControl)args.Sender);
        }

        private static void SetFinding(IControl control)
        {
            control.AttachedToLogicalTree -= Control_AttachedToLogicalTree;

            if (!SetAncestor(control))
            {
                control.AttachedToLogicalTree += Control_AttachedToLogicalTree;
            }
        }

        private static void Control_AttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs e)
        {
            IControl control = (IControl)sender!;

            SetAncestor(control);
        }

        private static IControl? FindAncestor(IControl control)
        {
            if (!control.IsAttachedToLogicalTree)
            {
                return null;
            }

            Type? type = GetAncestorType(control);
            string? name = GetAncestorName(control);

            if (type == null && name == null)
            {
                return null;
            }

            IControl? result = 
                control.GetLogicalAncestors()
                   .OfType<IControl>()
                   .FirstOrDefault
                   (
                    ancestor => 
                        ((type == null) || type.IsAssignableFrom(ancestor.GetType())) && 
                        ((name == null) || (ancestor.Name == name)));

            return result;
        }

        public static bool SetAncestor(IControl control)
        {
            var ancestor = FindAncestor(control);

            bool foundAncestor = false;
            if (ancestor != null)
            {
                control.AttachedToLogicalTree -= Control_AttachedToLogicalTree;

                foundAncestor = true;
            }

            SetAncestor(control, ancestor);

            return foundAncestor;
        }
    }
}
