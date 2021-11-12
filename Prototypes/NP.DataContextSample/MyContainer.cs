using NP.Avalonia.UniDock;
using NP.Avalonia.UniDock.Factories;
using NP.Avalonia.UniDockService;
using NP.IoCy;

namespace NP.DataContextSample
{
    public static class MyContainer
    {
        public static IoCContainer? TheContainer { get; }

        public static DockManager TheDockManager { get; } = new DockManager();

        static MyContainer()
        {
            TheContainer = new IoCContainer();

            TheDockManager.IsInEditableState = true;

            TheContainer.Map<IFloatingWindowFactory, MyCustomFloatingWindowFactory>();

            TheContainer.MapSingleton<IUniDockService, DockManager>(TheDockManager, null, true);

            TheContainer?.CompleteConfiguration();
        }
    }
}
