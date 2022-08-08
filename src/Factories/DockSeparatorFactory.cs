using Avalonia.Controls;
using Avalonia.Layout;

namespace NP.Avalonia.UniDock.Factories
{
    public class DockSeparatorFactory : IDockSeparatorFactory
    {
        public IControlWithOrientation GetDockSeparator(Orientation orientation)
        {
            return new DockSeparator();
        }
    }
}
