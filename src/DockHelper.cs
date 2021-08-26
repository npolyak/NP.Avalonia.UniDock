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
