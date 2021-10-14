namespace NP.Avalonia.UniDock.Factories
{
    public class DockSeparatorFactory : IDockSeparatorFactory
    {
        public DockSeparator GetDockSeparator(object? separatorChooser)
        {
            return new DockSeparator();
        }
    }
}
