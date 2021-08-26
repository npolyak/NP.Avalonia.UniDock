using Avalonia.Controls;
using Avalonia.Controls.Generators;
using NP.Avalonia.Visuals.Controls;

namespace NP.Avalonia.UniDock
{
    public class DockTabItemContainerGenerator : ItemContainerGenerator<TabItem>
    {
        public DockTabItemContainerGenerator(DockTabsPresenter owner) : 
            base(owner, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty)
        {
        }

        protected override IControl CreateContainer(object item)
        {
            IControl control = base.CreateContainer(item);

            control.Classes = new Classes("Dock");

            return control;
        }
    }
}
