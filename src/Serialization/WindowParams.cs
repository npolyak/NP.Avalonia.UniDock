using Avalonia.Controls;
using NP.Avalonia.Visuals;
using NP.Utilities;
using System;
using System.Xml.Serialization;

namespace NP.Avalonia.UniDock.Serialization
{
    public class WindowParams
    {
        [XmlAttribute]
        public string? FullWindowType { get; set; }

        public Point2D? TopLeft { get; set; }

        public Point2D? Size { get; set; }

        [XmlAttribute]
        public string? Title { get; set; }

        [XmlAttribute]
        public WindowState TheWindowState { get; set; }

        [XmlAttribute]
        public string? OwnerWindowId { get; set; }

        [XmlAttribute]
        public string? WindowId { get; set; }

        [XmlAttribute]
        public string? DockChildWindowOwnerId { get; set; }

        [XmlAttribute]
        public string? TopLevelGroupId { get; set; }
    }

    public static class WindowParamsHelper
    {
        public static WindowParams ToWindowParams(this Window w)
        {
            WindowParams wp = new WindowParams();

            wp.FullWindowType = w.GetType().FullName;

            wp.TopLeft = w.Position.ToPoint2D();

            wp.Size = w.Bounds.Size.ToPoint2D();

            wp.Title = w.Title;

            wp.TheWindowState = w.WindowState;

            wp.WindowId = DockAttachedProperties.GetWindowId(w);

            if (w.Owner != null)
            {
                wp.OwnerWindowId = DockAttachedProperties.GetWindowId(w.Owner);
            }

            Window dockChildWindowOwnerWindow =
                DockAttachedProperties.GetDockChildWindowOwner(w);

            if (dockChildWindowOwnerWindow != null)
            {
                wp.DockChildWindowOwnerId =
                    DockAttachedProperties.GetWindowId(dockChildWindowOwnerWindow);
            }

            if (w is DockWindow dockWindow)
            {
                wp.TopLevelGroupId = dockWindow.TheDockGroup.DockId;
            }

            return wp;
        }

        public static void  SetWindowFromParams(this Window w, WindowParams wp)
        {
            Type windowType = ReflectionUtils.RestoreType(wp.FullWindowType);

            if (windowType != w.GetType())
            {
                throw new Exception($"ERROR window types doe not match for the window titled {w.Title}.");
            }

            w.Position = wp.TopLeft.ToPixelPoint();
            w.Width = wp.Size!.X;
            w.Height = wp.Size.Y;

            w.Title = wp.Title;
            w.WindowState = wp.TheWindowState;

            string? windowId = wp.WindowId;

            if (windowId != null)
            {
                DockAttachedProperties.SetWindowId(w, windowId);
            }
        }

        public static Window? RestoreWindow(this WindowParams wp)
        {
            Type windowType = ReflectionUtils.RestoreType(wp.FullWindowType);

            // only DockWindows (floating windows) can be restored
            if (!typeof(DockWindow).IsAssignableFrom(windowType))
            {
                return null;
            }

            DockWindow w = (DockWindow) Activator.CreateInstance(windowType)!;

            w.SetWindowFromParams(wp);

            w.TheDockGroup.DockId = wp.TopLevelGroupId!;

            return w;
        }
    }
}
