using Avalonia.Controls;

namespace NP.AvaloniaDock
{
    public interface IDockVisualItemGenerator
    {
        IControl Generate(IDockGroup dockObj);
    }

    public class DockVisualItemGenerator : IDockVisualItemGenerator
    {
        public IControl Generate(IDockGroup dockObj)
        {
            IControl result;
            if (dockObj is DockItem dockItem)
            {
                result = new DockItemPresenter();
            }
            else
            {
                result = dockObj;
            }

            DockAttachedProperties.SetDockContext((Control) result, dockObj);

            return result;
        }
    }
}
