using Avalonia;
using Avalonia.Controls;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class LogicalChildrenBehavior
    {
        #region TheLogicalChildBehavior Attached Avalonia Property
        public static LogicalChildBehavior GetTheLogicalChildBehavior(AvaloniaObject obj)
        {
            return obj.GetValue(TheLogicalChildBehaviorProperty);
        }

        public static void SetTheLogicalChildBehavior(AvaloniaObject obj, LogicalChildBehavior value)
        {
            obj.SetValue(TheLogicalChildBehaviorProperty, value);
        }

        public static readonly AttachedProperty<LogicalChildBehavior> TheLogicalChildBehaviorProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, LogicalChildBehavior>
            (
                "TheLogicalChildBehavior"
            );
        #endregion TheLogicalChildBehavior Attached Avalonia Property

        static LogicalChildrenBehavior()
        {
            TheLogicalChildBehaviorProperty.Changed.Subscribe(OnLogicalChildBehaviorChanged);
        }

        private static void OnLogicalChildBehaviorChanged(AvaloniaPropertyChangedEventArgs<LogicalChildBehavior> change)
        {
            IControl control = (IControl) change.Sender;

            LogicalChildBehavior oldBehavior = change.OldValue.Value;

            oldBehavior?.Dispose();

            LogicalChildBehavior logicalChildBehavior = change.NewValue.Value;

            if (logicalChildBehavior != null)
            {
                logicalChildBehavior.TheControl = control;
            }
        }
    }
}
