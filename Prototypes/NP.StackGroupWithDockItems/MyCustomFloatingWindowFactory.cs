using Avalonia.Controls;
using NP.Avalonia.UniDock;
using NP.Avalonia.UniDock.Factories;

namespace NP.StackGroupWithDockItems
{
    public class MyCustomFloatingWindowFactory : IFloatingWindowFactory
    {
        public virtual FloatingWindow CreateFloatingWindow()
        {
            // create the window

            FloatingWindow dockWindow = new FloatingWindow();

            dockWindow.Classes = new Classes("PlainFloatingWindow");
            dockWindow.Title = "My Floating Window";
            dockWindow.TitleClasses = "WindowTitle";

            dockWindow.SetMovePtr();

            return dockWindow;
        }
    }
}
