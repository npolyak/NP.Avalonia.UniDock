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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public static class DockAttachedProperties
    {
        #region TheDockManager Attached Avalonia Property
        public static DockManager GetTheDockManager(AvaloniaObject obj)
        {
            return obj.GetValue(TheDockManagerProperty);
        }

        public static void SetTheDockManager(AvaloniaObject obj, DockManager? value)
        {
            obj.SetValue(TheDockManagerProperty!, value);
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

            if (sender is Window window)
            {
                if (oldDockManager != null)
                {
                    oldDockManager.RemoveWindow(window);
                }

                if (dockManager != null)
                {
                    dockManager.AddWindow(window);
                }
            }
            else if (sender is IDockGroup group)
            {
                if (oldDockManager != null)
                {
                    if (group is ILeafDockObj leafObj)
                    {
                        oldDockManager.DockLeafObjs.Remove(leafObj);
                    }

                    oldDockManager.RemoveGroup(group);
                }

                if (dockManager != null)
                {
                    dockManager.AddGroup(group);

                    if (group is ILeafDockObj leafObj)
                    {
                        dockManager.DockLeafObjs.Add(leafObj);
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


        #region IconButtonForeground Attached Avalonia Property
        public static IBrush GetIconButtonForeground(AvaloniaObject obj)
        {
            return obj.GetValue(IconButtonForegroundProperty);
        }

        public static void SetIconButtonForeground(AvaloniaObject obj, IBrush value)
        {
            obj.SetValue(IconButtonForegroundProperty, value);
        }

        public static readonly AttachedProperty<IBrush> IconButtonForegroundProperty =
            AvaloniaProperty.RegisterAttached<object, Control, IBrush>
            (
                "IconButtonForeground"
            );
        #endregion IconButtonForeground Attached Avalonia Property


        #region WindowId Attached Avalonia Property
        public static string? GetWindowId(AvaloniaObject obj)
        {
            return obj.GetValue(WindowIdProperty);
        }

        public static void SetWindowId(AvaloniaObject obj, string? value)
        {
            obj.SetValue(WindowIdProperty, value);
        }

        public static readonly AttachedProperty<string?> WindowIdProperty =
            AvaloniaProperty.RegisterAttached<object, Control, string?>
            (
                "WindowId"
            );
        #endregion WindowId Attached Avalonia Property

        #region DockChildWindowOwner Attached Avalonia Property
        // specifies if the floating windows that have been pulled out of this window
        // or out of its floating descendants, should be owned by this window.
        public static Window GetDockChildWindowOwner(AvaloniaObject obj)
        {
            return obj.GetValue(DockChildWindowOwnerProperty);
        }

        public static void SetDockChildWindowOwner(AvaloniaObject obj, Window value)
        {
            obj.SetValue(DockChildWindowOwnerProperty, value);
        }

        public static readonly AttachedProperty<Window> DockChildWindowOwnerProperty =
            AvaloniaProperty.RegisterAttached<object, Control, Window>
            (
                "DockChildWindowOwner"
            );
        #endregion DockChildWindowOwner Attached Avalonia Property


        #region SizeGridLength Attached Avalonia Property
        public static GridLength? GetSizeGridLength(AvaloniaObject obj)
        {
            return obj.GetValue(SizeGridLengthProperty);
        }

        public static void SetSizeGridLength(AvaloniaObject obj, GridLength? value)
        {
            obj.SetValue(SizeGridLengthProperty, value);
        }

        public static readonly AttachedProperty<GridLength?> SizeGridLengthProperty =
            AvaloniaProperty.RegisterAttached<object, Control, GridLength?>
            (
                "SizeGridLength"
            );
        #endregion SizeGridLength Attached Avalonia Property
    }
}
