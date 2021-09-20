using Avalonia;

namespace NP.Avalonia.UniDock.Factories
{
    public class FloatingWindowFactory : IFloatingWindowFactory
    {
        public virtual FloatingWindow CreateFloatingWindow()
        {
            // create the window

            FloatingWindow dockWindow = new FloatingWindow();

            dockWindow.CustomHeaderIcon = null;
            dockWindow.Title = "Floating Window";
            dockWindow.TitleClasses = "WindowTitle";

            dockWindow.SetMovePtr();

            return dockWindow;
        }
    }
}
