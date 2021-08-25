using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Utilities;
using System;
using System.Linq;

namespace NP.AvaloniaDock
{
    public abstract class DragItemBehavior<TItem>
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
                    false);
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

        protected TItem? _startItem;

        protected DockItem? _draggedDockItem;

        protected Func<TItem, DockItem> _dockItemGetter;

        public DragItemBehavior(Func<TItem, DockItem> dockItemGetter)
        {
            _dockItemGetter = dockItemGetter;
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
                OnDragPointerMoved!,
                RoutingStrategies.Bubble,
                false);

            itemsContainer.AddHandler
            (
                Control.PointerReleasedEvent,
                ClearHandlers!,
                RoutingStrategies.Bubble,
                false);

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
            control.RemoveHandler(Control.PointerMovedEvent, OnDragPointerMoved!);
        }

        protected abstract bool MoveItemWithinContainer(Control itemsContainer, PointerEventArgs e);

        protected virtual void OnDragPointerMoved(object sender, PointerEventArgs e)
        {
            if (_startItem == null)
            {
                return;
            }

            Control itemsContainer = (Control)sender;

            DockManager dockManager = _draggedDockItem!.TheDockManager!;

            Point2D currentPoint = e.GetPosition(itemsContainer).ToPoint2D();

            if (currentPoint.Minus(_startMousePoint).ToAbs().GreaterOrEqual(PointHelper.MinimumDragDistance).Any)
            {
                CurrentScreenPointBehavior.Capture(itemsContainer);

                _allowDrag = true;
            }

            if (CurrentScreenPointBehavior.CapturedControl != itemsContainer || !_allowDrag)
                return;

            bool allDone = MoveItemWithinContainer(itemsContainer, e);

            if (allDone)
            {
                return;
            }

            IDockGroup? parentItem = _draggedDockItem.DockParent;

            // remove from the current items
            _draggedDockItem?.RemoveItselfFromParent();

            Window parentWindow = parentItem.GetVisualAncestors().OfType<Window>().First();

            parentItem?.Simplify();

            // create the window
            var dockWindow = dockManager.CreateDockItemWindow(_draggedDockItem!);

            Window ownerWindow = DockAttachedProperties.GetDockChildWindowOwner(parentWindow);

            DockAttachedProperties.SetDockChildWindowOwner(dockWindow, ownerWindow);

            if (ownerWindow != null)
            {
                dockWindow.Show(parentWindow);
            }
            else
            {
                dockWindow.Show();
            }

            dockWindow.Activate();

            ClearHandlers(sender);
        }
    }
}
