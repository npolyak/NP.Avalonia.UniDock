namespace NP.Avalonia.UniDock.Factories
{
    public class DockSeparatorFactory : IDockSeparatorFactory
    {
        public DockSeparator GetDockSeparator()
        {
            return new DockSeparator();
        }
    }
}
