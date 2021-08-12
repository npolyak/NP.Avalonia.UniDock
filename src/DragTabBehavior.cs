using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public static class DragTabBehavior
    {
        #region IsSet Attached Avalonia Property
        public static bool GetIsSet(AvaloniaObject obj)
        {
            return obj.GetValue(IsSetProperty);
        }

        public static void SetIsSet(AvaloniaObject obj, bool value)
        {
            obj.SetValue(IsSetProperty, value);
        }

        public static readonly AttachedProperty<bool> IsSetProperty =
            AvaloniaProperty.RegisterAttached<object, Control, bool>
            (
                "IsSet"
            );
        #endregion IsSet Attached Avalonia Property

        static DragTabBehavior()
        {
            IsSetProperty.Changed.Subscribe(OnIsSetChanged);
        }

        private static void OnIsSetChanged(AvaloniaPropertyChangedEventArgs<bool> change)
        {
            Control tabContainer = (Control) change.Sender;

            if (change.NewValue.Value)
            {
                tabContainer.AddHandler
                (
                    Control.PointerPressedEvent,
                    Control_PointerPressed!,
                    RoutingStrategies.Bubble,
                    true);
            }
            else
            {
                tabContainer.RemoveHandler(Control.PointerPressedEvent, Control_PointerPressed!);
            }
        }

        private static Point2D? _startMousePoint;

        private static bool _allowDrag = false;

        private static TabItem _startTabItem;

        private static DockItem? _draggedDockItem;
        private static void Control_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Control tabContainer = (Control) sender;

            _startMousePoint = e.GetPosition(tabContainer).ToPoint2D();

            _startTabItem = e.GetTabItemCurrentPosition((ItemsPresenter) sender)!;

            if (_startTabItem == null)
            {
                return;
            }

            _draggedDockItem = _startTabItem?.Content as DockItem;

            tabContainer.AddHandler
            (
                Control.PointerMovedEvent, 
                DragControl_PointerMoved!, 
                RoutingStrategies.Bubble, 
                true);

            tabContainer.AddHandler
            (
                Control.PointerReleasedEvent,
                ClearHandlers!,
                RoutingStrategies.Bubble,
                true);

            _allowDrag = false;
        }

        private static void ClearHandlers(object sender, PointerEventArgs e)
        {
            ClearHandlers(sender);
        }

        private static void ClearHandlers(object sender)
        {
            Control control = (Control)sender;

            control.RemoveHandler(Control.PointerReleasedEvent, ClearHandlers!);
            control.RemoveHandler(Control.PointerMovedEvent, DragControl_PointerMoved!);
        }


        public static TabItem? GetTabItemCurrentPosition(this PointerEventArgs e, ItemsPresenter itemsPresenter)
        {
            Point pointerPositionWithinTabContainer = e.GetPosition(itemsPresenter);

            TabItem? tabMouseOver =
                    itemsPresenter
                        .GetVisualDescendants()
                        .OfType<TabItem>()
                        .FirstOrDefault(tab => tab.IsPointerWithinControl(e));

            return tabMouseOver;
        }

        public static DockItem GetDockItemCurrentPosition(this PointerEventArgs e, ItemsPresenter itemsPresenter)
        {
            return (DockItem)e.GetTabItemCurrentPosition(itemsPresenter)!.Content;
        }

        private static void DragControl_PointerMoved(object sender, PointerEventArgs e)
        {
            if (_startTabItem == null)
            {
                return;
            }

            ItemsPresenter tabContainer = (ItemsPresenter)sender;

            DockManager dockManager = DockAttachedProperties.GetTheDockManager(tabContainer);

            Point2D currentPoint = e.GetPosition(tabContainer).ToPoint2D();

            if (currentPoint.Minus(_startMousePoint).ToAbs().GreaterOrEqual(PointHelper.MinimumDragDistance).Any)
            {
                CurrentScreenPointBehavior.Capture(tabContainer);

                _allowDrag = true;
            }

            if (CurrentScreenPointBehavior.CapturedControl != tabContainer|| !_allowDrag)
                return;

            var siblingTabs =
                tabContainer.GetVisualDescendants().OfType<TabItem>().ToList();

            Point pointerPositionWithinTabContainer = e.GetPosition(tabContainer);

            IList itemsList = (IList)tabContainer.Items;

            if (tabContainer.IsPointWithinControl(pointerPositionWithinTabContainer))
            {
                TabItem? tabMouseOver = 
                    siblingTabs?.FirstOrDefault(tab => tab.IsPointerWithinControl(e));

                if (tabMouseOver != null && tabMouseOver != _startTabItem)
                {
                    int draggedDockItemIdx = itemsList.IndexOf(_draggedDockItem);

                    DockItem dropDockItem = (DockItem)tabMouseOver.Content;
                    int dropIdx = itemsList!.IndexOf(dropDockItem);

                    itemsList?.Remove(_draggedDockItem);

                    itemsList?.Insert(dropIdx, _draggedDockItem);

                    _draggedDockItem!.IsSelected = true;
                }
            }
            else
            {
                // remove from tabs within the tab group
                itemsList?.Remove(_draggedDockItem);

                // create the window
                ClearHandlers(sender);

                DockWindow dockWindow = new DockWindow();

                DockAttachedProperties.SetTheDockManager(dockWindow, dockManager);

                var pointerScreenPosition = tabContainer.PointToScreen(pointerPositionWithinTabContainer);

                dockWindow.Width = 400;
                dockWindow.Height = 300;

                dockWindow.Items.Add(_draggedDockItem);

                dockWindow.SetMovePtr();

                dockWindow.Show();
                dockWindow.Activate();
            }
        }

        private static void CustomWindow_PointerEnter(object? sender, PointerEventArgs e)
        {
        }
    }
}
