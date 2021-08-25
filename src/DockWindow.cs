using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DockWindow : CustomWindow
    {
        public SimpleDockGroup TheDockGroup { get; } = 
            new SimpleDockGroup();

        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            private set
            {
                DockAttachedProperties.SetTheDockManager(this, value!);
                TheDockGroup.TheDockManager = value!;
            }
        }

        public DockWindow(DockManager dockManager)
        {
            Classes = new Classes(new[] { "PlainDockWindow" });
            HasCustomWindowFeatures = true;
            Content = TheDockGroup;
            DockAttachedProperties.SetTheDockManager(this, dockManager);
            TheDockGroup.TheDockManager = dockManager;

            TheDockGroup.HasNoChildrenEvent += TheDockGroup_HasNoChildrenEvent;

            this.Closing += DockWindow_Closing;
        }

        private void TheDockGroup_HasNoChildrenEvent(SimpleDockGroup obj)
        {
            if ((TheDockGroup as IDockGroup).AutoDestroy)
            {
                this.Close();
            }
        }

        private void DockWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            TheDockManager = null;

            var allGroups = TheDockGroup.GetDockGroupSelfAndAncestors().Reverse().ToList();

            allGroups?.DoForEach(item => item.RemoveItselfFromParent());
        }

        protected Point2D? StartPointerPosition { get; set; }
        protected Point2D? StartWindowPosition { get; set; }


        #region PointerShift Styled Avalonia Property
        public Point2D PointerShift
        {
            get { return GetValue(PointerShiftProperty); }
            set { SetValue(PointerShiftProperty, value); }
        }

        public static readonly StyledProperty<Point2D> PointerShiftProperty =
            AvaloniaProperty.Register<CustomWindow, Point2D>
            (
                nameof(PointerShift)
            );
        #endregion PointerShift Styled Avalonia Property


        public void SetMovePtr()
        {
            this.Activated += CustomWindow_Activated!;
        }

        private void CustomWindow_Activated(object sender, EventArgs e)
        {
            this.Activated -= CustomWindow_Activated!;
            StartPointerPosition = CurrentScreenPointBehavior.CurrentScreenPointValue;
            StartWindowPosition = StartPointerPosition.Minus(new Point2D(60, 10));
            Position = StartWindowPosition.ToPixelPoint();

            SetDragOnMovePointer();
        }

        protected override void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            SetDragOnMovePointer(e);
        }

        private void SetDragOnMovePointer(PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(HeaderControl).Properties.IsLeftButtonPressed)
            {
                return;
            }

            StartPointerPosition = GetCurrentPointInScreen(e);
            StartWindowPosition = this.Position.ToPoint2D();
            PointerShift = new Point2D();

            SetDragOnMovePointer();
        }

        private void SetDragOnMovePointer()
        {
            TheDockManager.DraggedWindow = this;

            CurrentScreenPointBehavior.Capture(HeaderControl);

            if (HeaderControl != null)
            {
                HeaderControl.PointerMoved += OnPointerMoved!;

                HeaderControl.PointerReleased += OnPointerReleased!;
            }
        }

        public Point2D GetCurrentPointInScreen(PointerEventArgs e)
        {
            var result = HeaderControl.PointToScreen(e.GetPosition(HeaderControl));

            return result.ToPoint2D();
        }

        private void UpdatePosition(PointerEventArgs e)
        {
            PointerShift = GetCurrentPointInScreen(e).Minus(StartPointerPosition);

            this.Position = StartWindowPosition.Plus(PointerShift).ToPixelPoint();
        }

        protected void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (HeaderControl != null)
            {
                HeaderControl.PointerMoved -= OnPointerMoved!;

                HeaderControl.PointerReleased -= OnPointerReleased!;
            }

            UpdatePosition(e);

            TheDockManager.CompleteDragDropAction();
        }

        protected void OnPointerMoved(object sender, PointerEventArgs e)
        {
            UpdatePosition(e);
        }

        private IEnumerable<ILeafDockObj> GetLeafGroups(DockManager dockManager)
        {
            return this.TheDockGroup
                        .GetDockGroupSelfAndDescendants(stopCondition:item => item is ILeafDockObj)
                        .OfType<ILeafDockObj>()
                        .Distinct()
                        .Where(g => ReferenceEquals(g.TheDockManager, dockManager));
        }

        public IEnumerable<DockItem> LeafItems
        {
            get
            {
                return GetLeafGroups(TheDockManager).SelectMany(g => g.LeafItems);
            }
        }
    }
}
