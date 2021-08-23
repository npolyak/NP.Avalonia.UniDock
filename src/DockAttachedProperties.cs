using Avalonia;
using Avalonia.Controls;
using NP.Utilities;
using System;
using System.Linq;

namespace NP.AvaloniaDock
{
    public static class DockAttachedProperties
    {
        #region TheDockManager Attached Avalonia Property
        public static DockManager GetTheDockManager(AvaloniaObject obj)
        {
            return obj.GetValue(TheDockManagerProperty);
        }

        public static void SetTheDockManager(AvaloniaObject obj, DockManager value)
        {
            obj.SetValue(TheDockManagerProperty, value);
        }

        public static readonly AttachedProperty<DockManager> TheDockManagerProperty =
            AvaloniaProperty.RegisterAttached<object, Control, DockManager>
            (
                "TheDockManager"
            );
        #endregion TheDockManager Attached Avalonia Property

        static DockAttachedProperties()
        {
            TheDockManagerProperty.Changed.Subscribe(OnDockManagerChanged);
        }

        private static void OnDockManagerChanged(AvaloniaPropertyChangedEventArgs<DockManager> dockManagerChangeArgs)
        {
            var oldDockManager = dockManagerChangeArgs.OldValue.Value;

            var sender = dockManagerChangeArgs.Sender;

            var dockManager = dockManagerChangeArgs.NewValue.Value;

            if (sender is DockWindow window)
            {
                if (oldDockManager != null)
                {
                    oldDockManager.DockWindows.Remove(window);
                }

                if (dockManager != null)
                {
                    dockManager.DockWindows.Add(window);
                }
            }
            else if (sender is ILeafDockObj group)
            {
                if (oldDockManager != null)
                {
                    oldDockManager.DockLeafObjs.Remove(group);

                    group.GetDockGroupDescendants()
                         .OfType<ILeafDockObj>()
                         .Where(item => item.TheDockManager == oldDockManager)
                         .ToList()
                         .DoForEach(item => oldDockManager.DockLeafObjs.Remove(item));
                }

                if (dockManager != null)
                {
                    if (!group.GetDockGroupAncestors().Any(item => item is ILeafDockObj))
                    {
                        dockManager.DockLeafObjs.Add(group);
                    }
                }
            }
        }

        #region DockSide Attached Avalonia Property
        public static DockKind? GetDockSide(AvaloniaObject obj)
        {
            return obj.GetValue(DockSideProperty);
        }

        public static void SetDockSide(AvaloniaObject obj, DockKind? value)
        {
            obj.SetValue(DockSideProperty, value);
        }

        public static readonly AttachedProperty<DockKind?> DockSideProperty =
            AvaloniaProperty.RegisterAttached<object, Control, DockKind?>
            (
                "DockSide"
            );
        #endregion DockSide Attached Avalonia Property
    }
}
