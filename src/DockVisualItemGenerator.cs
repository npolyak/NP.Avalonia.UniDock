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

namespace NP.Avalonia.UniDock
{
    public interface IDockVisualItemGenerator
    {
        IControl Generate(IDockGroup dockObj);
    }

    public class DockVisualItemGenerator : IDockVisualItemGenerator
    {
        public IControl Generate(IDockGroup dockObj)
        {
            if (dockObj is DockItem dockItem)
            {
                DockItemPresenter result = new DockItemPresenter();

                result.DockContext = dockItem;

                return result;
            }

            return dockObj;
        }
    }
}
