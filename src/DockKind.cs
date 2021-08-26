using Avalonia.Layout;

namespace NP.Avalonia.UniDock
{
    public enum DockKind
    {
        Tabs,
        Left,
        Top,
        Right,
        Bottom
    }

    public static class DockKindHelper
    {
        public static Orientation? ToOrientation(this DockKind? dock)
        {
            return dock switch
            {
                DockKind.Left => Orientation.Horizontal,
                DockKind.Right => Orientation.Horizontal,
                DockKind.Top => Orientation.Vertical,
                DockKind.Bottom => Orientation.Vertical,
                _ => null
            };
        }

        public static int ToInsertIdx(this int idx, DockKind? dock)
        {
            switch(dock)
            {
                case DockKind.Right:
                case DockKind.Bottom:
                    return idx + 1;

                default:
                    return idx;
            }
        }
    }
}
