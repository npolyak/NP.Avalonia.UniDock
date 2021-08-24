﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DragTabBehavior : DragItemBehavior<TabItem>
    {
        #region IsSet Attached Avalonia Property
        public static void SetIsSet(AvaloniaObject obj, bool value)
        {
            obj.SetValue(IsSetProperty, value);
        }
        #endregion IsSet Attached Avalonia Property

        private static DockItem? GetDockItem(TabItem tabItem) =>
            tabItem.Content as DockItem;

        private static IList<IDockGroup>? GetDockItemsCollection(DockItem dockItem) =>
            (dockItem.DockParent as DockTabbedGroup)?.Items;

        protected override bool MoveItemWithinContainer(Control itemsContainer, PointerEventArgs e)
        {
            Point pointerPositionWithinItemsContainer = e.GetPosition(itemsContainer);

            IList<IDockGroup> itemsList = GetDockItemsCollection(_draggedDockItem!)!;

            if (itemsContainer.IsPointWithinControl(pointerPositionWithinItemsContainer))
            {
                var siblingTabs =
                    itemsContainer.GetVisualDescendants().OfType<TabItem>().ToList();

                TabItem? tabMouseOver =
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

        public DragTabBehavior() : base(GetDockItem!)
        {

        }

        static DragTabBehavior()
        {
            Instance = new DragTabBehavior();
        }
    }
}
