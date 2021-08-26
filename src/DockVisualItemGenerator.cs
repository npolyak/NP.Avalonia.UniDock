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
