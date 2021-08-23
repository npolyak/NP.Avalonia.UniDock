using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class DragTabBehavior : DragTabItemBehavior<TabItem>
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

        public DragTabBehavior() : base(GetDockItem!, GetDockItemsCollection!)
        {

        }

        static DragTabBehavior()
        {
            Instance = new DragTabBehavior();
        }
    }
}
