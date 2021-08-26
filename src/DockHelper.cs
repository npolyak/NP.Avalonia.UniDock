namespace NP.Avalonia.UniDock
{
    public static class DockHelper
    {
        public static DockWindow CreateDockItemWindow(this DockManager dockManager, DockItem dockItem)
        {
            dockItem!.CleanSelfOnRemove();

            // create the window

            DockWindow dockWindow = new DockWindow(dockManager);

            dockWindow.Width = 400;
            dockWindow.Height = 300;

            dockWindow.TheDockGroup.DockChildren.Add(dockItem!);

            dockWindow.CustomHeaderIcon = null;
            dockWindow.Title = dockItem.Header?.ToString();
            dockWindow.TitleClasses = "WindowTitle";

            dockWindow.SetMovePtr();

            return dockWindow;
        }
    }
}
