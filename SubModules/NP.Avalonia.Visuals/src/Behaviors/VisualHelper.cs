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
                        .GetSelfAndVisualDescendants()
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

        public static TWindow GetControlsWindow<TWindow>(this IVisual visual)
            where TWindow : Window
        {
            if (visual.IsAttachedToVisualTree)
            {
                return 
                    visual.GetSelfAndVisualAncestors()
                          .OfType<TWindow>()
                          .FirstOrDefault();
            }

            return null;
        }

        public static void ShowWindow(this Window window, Window ownerWindow)
        {
            if (ownerWindow != null)
            {
                window.Show(ownerWindow);
            }
            else
            {
                window.Show();
            }
        }
    }
}
