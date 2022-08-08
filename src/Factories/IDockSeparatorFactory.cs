using Avalonia.Controls;
using Avalonia.Layout;

namespace NP.Avalonia.UniDock.Factories
{
    public interface IDockSeparatorFactory
    {
        IControlWithOrientation GetDockSeparator(Orientation orientation);
    }
}
