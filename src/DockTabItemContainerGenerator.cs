using Avalonia.Controls;
using Avalonia.Controls.Generators;

namespace NP.AvaloniaDock
{
    public class DockTabItemContainerGenerator : ItemContainerGenerator<TabItem>
    {
        public DockTabItemContainerGenerator(DockTabsPresenter owner) : 
            base(owner, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty)
        {
        }
    }
}
