using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace NP.Avalonia.UniDock
{
    public class DragHeaderBehavior : DragItemBehavior<DockItemHeaderControl>
    {
        #region IsSet Attached Avalonia Property
        public static void SetIsSet(AvaloniaObject obj, bool value)
        {
            obj.SetValue(IsSetProperty, value);
        }
        #endregion IsSet Attached Avalonia Property

        private static DockItem? GetDockItem(DockItemHeaderControl headerControl) =>
            headerControl.DataContext as DockItem;

        protected override bool MoveItemWithinContainer(Control itemsContainer, PointerEventArgs e)
        {
            return false;
        }

        public DragHeaderBehavior() : base(GetDockItem!)
        {

        }

        static DragHeaderBehavior()
        {
            Instance = new DragHeaderBehavior();
        }
    }
}
