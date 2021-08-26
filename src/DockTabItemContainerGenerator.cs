// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

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
