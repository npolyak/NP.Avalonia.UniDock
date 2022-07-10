using NP.Avalonia.UniDock;
using NP.Avalonia.UniDock.Factories;
using NP.Avalonia.UniDockService;
using NP.IoCy;

namespace NP.ViewModelSample
{
    public static class MyContainer
    {
        public static IoCContainer? TheContainer { get; }

        public static DockManager TheDockManager { get; } = new DockManager();

        static MyContainer()
        {
            TheContainer = new IoCContainer();


            TheContainer.MapSingleton<IStackGroupFactory, StackGroupFactory>();
            TheContainer.MapSingleton<IFloatingWindowFactory, MyCustomFloatingWindowFactory>();
            TheContainer.MapSingleton<DockManager, DockManager>(TheDockManager);
            TheContainer.MapSingleton<IUniDockService, DockManager>(TheDockManager);

            TheContainer?.CompleteConfiguration();
        }
    }
}
