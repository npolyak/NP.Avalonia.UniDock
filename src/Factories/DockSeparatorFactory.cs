using Avalonia.Layout;

namespace NP.Avalonia.UniDock.Factories
{
    public class DockSeparatorFactory : IDockSeparatorFactory
    {
        public DockSeparator GetDockSeparator(Orientation orientation)
        {
            return new DockSeparator();
        }
    }
}
