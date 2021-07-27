using Avalonia.Controls;
using Avalonia.Controls.Generators;

namespace DockWindowsSample
{
    public class DockTabItemContainerGenerator : ItemContainerGenerator<TabItem>
    {
        public DockTabItemContainerGenerator(DockTabControl owner) : 
            base(owner, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty)
        {
            Owner = owner;
        }

        public new DockTabControl Owner { get; }

        protected override IControl CreateContainer(object item)
        {
            var tabItem = (TabItem)base.CreateContainer(item);

            return tabItem;
        }
    }
}
