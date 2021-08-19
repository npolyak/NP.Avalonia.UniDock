using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Utilities;
using System;
using System.Collections;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DragItemBehavior<TItem>
        where TItem : Control, IControl
    {
        protected static DragItemBehavior<TItem>? Instance { get; set; }
        public static bool GetIsSet(AvaloniaObject obj)
        {
            return obj.GetValue(IsSetProperty);
        }

        public static readonly AttachedProperty<bool> IsSetProperty =
            AvaloniaProperty.RegisterAttached<object, Control, bool>
            (
                "IsSet"
            );
        protected static void OnIsSetChanged(AvaloniaPropertyChangedEventArgs<bool> change)
        {
            Control itemsContainer = (Control)change.Sender;

            if (change.NewValue.Value)
            {
                itemsContainer.AddHandler
                (
                    Control.PointerPressedEvent,
                    Instance!.Control_PointerPressed!,
                    RoutingStrategies.Bubble,
                    true);
            }
            else
            {
                itemsContainer.RemoveHandler(Control.PointerPressedEvent, Instance!.Control_PointerPressed!);
            }
        }

        static DragItemBehavior()
        {
            IsSetProperty.Changed.Subscribe(OnIsSetChanged);
        }

        private Point2D? _startMousePoint;

        private bool _allowDrag = false;

        private TItem? _startItem;

        private DockItem? _draggedDockItem;

        private Func<TItem, DockItem> _dockItemGetter;
        private Func<DockItem, IList> _itemsListGetter;

        public DragItemBehavior(Func<TItem, DockItem> dockItemGetter, Func<DockItem, IList> itemsListGetter)
        {
            _dockItemGetter = dockItemGetter;

            _itemsListGetter = itemsListGetter;
        }

        private void Control_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Control itemsContainer = (Control)sender;

            _startMousePoint = e.GetPosition(itemsContainer).ToPoint2D();

            _startItem = e.GetControlUnderCurrentMousePosition<TItem>((Control)sender)!;

            if (_startItem == null)
            {
                return;
            }

            _draggedDockItem = _dockItemGetter.Invoke(_startItem);

            itemsContainer.AddHandler
            (
                Control.PointerMovedEvent,
                DragControl_PointerMoved!,
                RoutingStrategies.Bubble,
                true);

            itemsContainer.AddHandler
            (
                Control.PointerReleasedEvent,
                ClearHandlers!,
                RoutingStrategies.Bubble,
                true);

            _allowDrag = false;
        }

        private void ClearHandlers(object sender, PointerEventArgs e)
        {
            ClearHandlers(sender);
        }

        private void ClearHandlers(object sender)
        {
            Control control = (Control)sender;

            control.RemoveHandler(Control.PointerReleasedEvent, ClearHandlers!);
            control.RemoveHandler(Control.PointerMovedEvent, DragControl_PointerMoved!);
        }


        private void DragControl_PointerMoved(object sender, PointerEventArgs e)
        {
            if (_startItem == null)
            {
                return;
            }

            Control itemsContainer = (Control)sender;

            DockManager dockManager = DockAttachedProperties.GetTheDockManager(itemsContainer);

            Point2D currentPoint = e.GetPosition(itemsContainer).ToPoint2D();

            if (currentPoint.Minus(_startMousePoint).ToAbs().GreaterOrEqual(PointHelper.MinimumDragDistance).Any)
            {
                CurrentScreenPointBehavior.Capture(itemsContainer);

                _allowDrag = true;
            }

            if (CurrentScreenPointBehavior.CapturedControl != itemsContainer || !_allowDrag)
                return;

            Point pointerPositionWithinItemsContainer = e.GetPosition(itemsContainer);

            IList itemsList = _itemsListGetter.Invoke(_draggedDockItem!);

            if (itemsContainer.IsPointWithinControl(pointerPositionWithinItemsContainer))
            {
                var siblingTabs =
                    itemsContainer.GetVisualDescendants().OfType<TItem>().ToList();

                TItem? tabMouseOver =
                    siblingTabs?.FirstOrDefault(tab => tab.IsPointerWithinControl(e));

                if (tabMouseOver != null && tabMouseOver != _startItem)
                {
                    int draggedDockItemIdx = itemsList.IndexOf(_draggedDockItem);

                    DockItem dropDockItem = _dockItemGetter(tabMouseOver);
                    int dropIdx = itemsList!.IndexOf(dropDockItem);

                    itemsList?.Remove(_draggedDockItem);

                    itemsList?.Insert(dropIdx, _draggedDockItem);

                    _draggedDockItem!.IsSelected = true;
                }
            }
            else
            {
                // remove from the current items
                itemsList?.Remove(_draggedDockItem);

                // create the window
                ClearHandlers(sender);

                DockWindow dockWindow = new DockWindow(dockManager);

                var pointerScreenPosition = itemsContainer.PointToScreen(pointerPositionWithinItemsContainer);

                dockWindow.Width = 400;
                dockWindow.Height = 300;

                dockWindow.TheTabbedGroup?.Items.Add(_draggedDockItem!);

                dockWindow.SetMovePtr();

                dockWindow.Show();
                dockWindow.Activate();
            }
        }
    }
}
