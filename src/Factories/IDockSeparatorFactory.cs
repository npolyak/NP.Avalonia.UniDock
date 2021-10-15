using Avalonia.Layout;

namespace NP.Avalonia.UniDock.Factories
{
    public interface IDockSeparatorFactory
    {
        DockSeparator GetDockSeparator(Orientation orientation);
    }
}
