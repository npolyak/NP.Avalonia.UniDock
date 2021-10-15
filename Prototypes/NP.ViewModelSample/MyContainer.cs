using NP.Avalonia.UniDock;
using NP.Avalonia.UniDock.Factories;
using NP.Avalonia.UniDockService;
using NP.IoCy;

namespace NP.ViewModelSample
{
    public static class MyContainer
    {
        public static IoCContainer? TheContainer { get; }

        static MyContainer()
        {
            TheContainer = new IoCContainer();

            TheContainer.Map<IFloatingWindowFactory, MyCustomFloatingWindowFactory>();
            TheContainer.MapSingleton<IUniDockService, DockManager>();

            TheContainer?.CompleteConfiguration();
        }
    }
}
