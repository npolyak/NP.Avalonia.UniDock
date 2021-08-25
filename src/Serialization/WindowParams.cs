using Avalonia.Controls;
using NP.Avalonia.Visuals;
using NP.Utilities;
using System;
using System.Xml.Serialization;

namespace NP.AvaloniaDock.Serialization
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

            return wp;
        }

        public static Window ToWindow(this WindowParams wp)
        {
            Type windowType = ReflectionUtils.RestoreType(wp.FullWindowType);

            Window w =
                (Window) Activator.CreateInstance(windowType)!;

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

            string? ownerWindowId = wp.OwnerWindowId;

            if (ownerWindowId != null)
            {
                DockAttachedProperties.SetTemporaryOwnerWindowId(w, ownerWindowId);
            }

            string? dockChildWindowOwnerId = wp.DockChildWindowOwnerId;

            if (dockChildWindowOwnerId != null)
            {
                DockAttachedProperties
                    .SetTemporaryDockChildWindowOwnerId(w, dockChildWindowOwnerId);
            }

            return w;
        }
    }
}
