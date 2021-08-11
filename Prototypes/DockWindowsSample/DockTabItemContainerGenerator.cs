using Avalonia.Controls;
using Avalonia.Controls.Generators;

namespace DockWindowsSample
{
    public class DockTabItemContainerGenerator : ItemContainerGenerator<TabItem>
    {
        public DockTabItemContainerGenerator(DockTabsControl owner) : 
            base(owner, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty)
        {
        }
    }
}
