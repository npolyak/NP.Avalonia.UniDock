using Avalonia;
using Avalonia.Controls;

namespace NP.Avalonia.UniDock.Factories
{
    public class FloatingWindowFactory : IFloatingWindowFactory
    {
        public virtual FloatingWindow CreateFloatingWindow(object? windowChooser)
        {
            // create the window

            FloatingWindow dockWindow = new FloatingWindow();

            dockWindow.Classes = new Classes("PlainFloatingWindow" );
            dockWindow.TitleClasses = "WindowTitle";

            return dockWindow;
        }
    }
}
