using Avalonia;
using Avalonia.Controls;
using System;

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
            else if (sender is DockTabbedGroup group)
            {
                if (oldDockManager != null)
                {
                    oldDockManager.TabbedGroups.Remove(group);
                }

                if (dockManager != null)
                {
                    dockManager.TabbedGroups.Add(group);
                }
            }

        }
    }
}
