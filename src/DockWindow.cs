using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
        SimpleDockGroup _dockGroup = new SimpleDockGroup();

        public DockManager TheDockManager
        {
            get => (_dockGroup as IDockGroup).TheDockManager!;
            set
            {
                DockManager dockManager = TheDockManager;

                if (ReferenceEquals(dockManager, value))
                {
                    return;
                }

                if (dockManager != null)
                {
                    dockManager.DockWindows.Remove(this);
                }

                (_dockGroup as IDockGroup).TheDockManager = value;

                dockManager = TheDockManager;

                if (dockManager != null)
                {
                    dockManager.DockWindows.Add(this);
                }
            }
        }

        public DockWindow(DockManager dockManager)
        {
            Classes = new Classes(new[] { "PlainCustomWindow" });
            HasCustomWindowFeatures = true;
            Content = _dockGroup;
            _dockGroup.DockChild = new DockTabbedGroup();
            TheDockManager = dockManager;

            this.Closing += DockWindow_Closing;
        }

        private void DockWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.TheDockManager = null!;
        }

        private void OnDockManagerChanged(AvaloniaPropertyChangedEventArgs<DockManager> dockManagerChange)
        {
            if (dockManagerChange.Sender != this)
            {
                return;
            }

            DockManager dockManager = dockManagerChange.NewValue.Value;

            //DockManager oldDockManager = dockManagerChange.NewValue.Value;
            //var groupsToChange = GetGroups(oldDockManager);
            //groupsToChange.DoForEach(g => DockAttachedProperties.SetTheDockManager(g, dockManager));
        }

        public IEnumerable<DockTabbedGroup> TabbedGroups =>
            _dockGroup.DockGroupDescendants().OfType<DockTabbedGroup>();

        public DockTabbedGroup? TheTabbedGroup => TabbedGroups?.FirstOrDefault();


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

        private IEnumerable<DockTabbedGroup> GetGroups(DockManager dockManager)
        {
            return this.GetVisualDescendants()
                        .OfType<DockTabbedGroup>()
                        .Where(g => ReferenceEquals((g as IDockGroup).TheDockManager, dockManager));
        }

        public IEnumerable<DockTabbedGroup> Groups
        {
            get
            {
                return GetGroups(TheDockManager);
            }
        }
    }
}
