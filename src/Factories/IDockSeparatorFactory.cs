using Avalonia.Layout;

namespace NP.Avalonia.UniDock.Factories
{
    public interface IDockSeparatorFactory
    {
        bool ResizePreview { get; set; }

        IControlWithOrientation GetDockSeparator(Orientation orientation);
    }
}
