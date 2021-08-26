using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace NP.Avalonia.UniDock
{
    public class DockTabsPresenter : ItemsPresenter
    {
        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new DockTabItemContainerGenerator(this);
        }

        private static readonly FuncTemplate<IPanel> DefaultPanel =
            new FuncTemplate<IPanel>(() => new WrapPanel() { Orientation = Orientation.Horizontal });

        static DockTabsPresenter()
        {
            ItemsPanelProperty.OverrideDefaultValue<DockTabsPresenter>(DefaultPanel);
        }
    }
}
