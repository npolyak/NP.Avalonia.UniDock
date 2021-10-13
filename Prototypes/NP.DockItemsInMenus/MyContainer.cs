using NP.Avalonia.UniDock.Factories;
using NP.IoCy;

namespace NP.DockItemsInMenus
{
    public static class MyContainer
    {
        public static IoCContainer? TheContainer { get; }

        static MyContainer()
        {
            TheContainer = new IoCContainer();

            TheContainer.Map<IFloatingWindowFactory, MyCustomFloatingWindowFactory>();

            TheContainer?.CompleteConfiguration();
        }
    }
}
