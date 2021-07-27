using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using NP.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DockWindowsSample
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
            Control control = (Control) change.Sender;

            if (change.NewValue.Value)
            {
                control.PointerPressed += Control_PointerPressed;
            }
            else
            {
                control.PointerPressed -= Control_PointerPressed;
            }
        }

        private static Point2D? _startMousePoint;

        private static bool _allowDrag = false;
        private static void Control_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Control control = (Control) sender;

            _startMousePoint = e.GetPosition(control).ToPoint2D();

            control.PointerReleased += Control_PointerReleased;
            control.PointerMoved += Control_PointerMoved;

            _allowDrag = false;
        }

        private static void Control_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            Control control = (Control)sender;

            control.PointerReleased -= Control_PointerReleased;
            control.PointerMoved -= Control_PointerMoved;
        }

        private static void Control_PointerMoved(object sender, PointerEventArgs e)
        {
            TabItem draggedTabItem = (TabItem)sender;

            Point2D currentPoint = e.GetPosition(draggedTabItem).ToPoint2D();

            if (currentPoint.Minus(_startMousePoint).ToAbs().GreaterOrEqual(PointHelper.MinimumDragDistance).Any)
            {
                e.Pointer.Capture(draggedTabItem);
                _allowDrag = true;
            }

            if (e.Pointer.Captured != draggedTabItem || !_allowDrag)
                return;

            var tabContainer = draggedTabItem.FindAncestorOfType<ItemsPresenter>();

            var siblingTabs =
                tabContainer.GetVisualDescendants().OfType<TabItem>().ToList();

            siblingTabs.Remove(draggedTabItem);

            Point pointerPositionWithinTabContainer = e.GetPosition(tabContainer);
            if (tabContainer.IsPointWithinControl(pointerPositionWithinTabContainer))
            {
                TabItem? tabMouseOver = 
                    siblingTabs?.FirstOrDefault(tab => tab.IsPointerWithinControl(e));

                if (tabMouseOver != null)
                {
                    IList itemsList = (IList) tabContainer.Items;

                    DockItem draggedDockItem = (DockItem) draggedTabItem.Content ;

                    itemsList?.Remove(draggedDockItem);

                    DockItem dropDockItem = (DockItem) tabMouseOver.Content;

                    int dropIdx = itemsList!.IndexOf(dropDockItem) + 1;

                    itemsList.Insert(dropIdx, draggedDockItem);

                    draggedDockItem.IsSelected = true;
                }
            }
        }
    }
}
