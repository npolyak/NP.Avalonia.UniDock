namespace NP.Avalonia.UniDock.Factories
{
    public class TabbedGroupFactory : ITabbedGroupFactory
    {
        public TabbedDockGroup Create(object? tabbedDockGroupChooser)
        {
            return new TabbedDockGroup();
        }
    }
}
