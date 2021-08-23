using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DragTabItemBehavior<TItem> : DragItemBehavior<TItem>
        where TItem : Control, IControl
    {
        private Func<DockItem, IList<IDockGroup>> _itemsListGetter;

        protected override bool MoveItemWithinContainer(Control itemsContainer, PointerEventArgs e)
        {
            Point pointerPositionWithinItemsContainer = e.GetPosition(itemsContainer);

            IList<IDockGroup> itemsList = _itemsListGetter.Invoke(_draggedDockItem!);

            if (itemsContainer.IsPointWithinControl(pointerPositionWithinItemsContainer))
            {
                var siblingTabs =
                    itemsContainer.GetVisualDescendants().OfType<TItem>().ToList();

                TItem? tabMouseOver =
                    siblingTabs?.FirstOrDefault(tab => tab.IsPointerWithinControl(e));

                if (tabMouseOver != null && tabMouseOver != _startItem)
                {
                    int draggedDockItemIdx = itemsList.IndexOf(_draggedDockItem!);

                    DockItem dropDockItem = _dockItemGetter(tabMouseOver);
                    int dropIdx = itemsList!.IndexOf(dropDockItem);

                    itemsList?.Remove(_draggedDockItem!);

                    itemsList?.Insert(dropIdx, _draggedDockItem!);

                    _draggedDockItem!.IsSelected = true;
                }

                return true;
            }

            return false;
        }

        public DragTabItemBehavior(Func<TItem, DockItem> dockItemGetter, Func<DockItem, IList<IDockGroup>> itemsListGetter) : 
            base(dockItemGetter)
        {
            _itemsListGetter = itemsListGetter;
        }

    }
}
