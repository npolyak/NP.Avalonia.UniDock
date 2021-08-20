using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.VisualTree;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class VisualHelper
    {
        public static TItem GetControlUnderCurrentMousePosition<TItem>(this PointerEventArgs e, Control itemContainer)
            where TItem : Control
        {
            Point pointerPositionWithinTabContainer = e.GetPosition(itemContainer);

            TItem tabMouseOver =
                    itemContainer
                        .GetVisualDescendants()
                        .OfType<TItem>()
                        .FirstOrDefault(tab => tab.IsPointerWithinControl(e));

            return tabMouseOver;
        }

        public static void DisconnectVisualParentContentPresenter(this IControl control)
        {
            IContentPresenter? parent =
                control.GetVisualParent() as IContentPresenter;

            if (parent != null)
            {
                parent.Content = null;
            }
        }
    }
}
