// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;

namespace NP.Avalonia.UniDock
{
    public class FloatingWindow : CustomWindow, IDockManagerContainer
    {
        public bool AutoDestroy { get; set; } = true;

        public RootDockGroup TheDockGroup { get; } = 
            new RootDockGroup();

        [Content]
        public IDockGroup? DockContent
        {
            get => TheDockGroup.TheChild;

            set
            {
                TheDockGroup.TheChild = value;
            }
        }


        public RootDockGroup? ParentWindowGroup 
        { 
            get => TheDockGroup.ParentWindowGroup;
            set
            {
                TheDockGroup.ParentWindowGroup = value;
            }
        }


        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value!);
        }

        static FloatingWindow()
        {
            DockAttachedProperties
                .TheDockManagerProperty
                .Changed
                .AddClassHandler<FloatingWindow>((dw, e) => dw.OnDockManagerChanged(e));
        }

        private void OnDockManagerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            TheDockGroup.TheDockManager = TheDockManager;
        }

        private readonly IDisposable _subscription;
        public FloatingWindow() 
        {
            DragOnBeginMove = false;
            HasCustomWindowFeatures = true;
            Content = TheDockGroup;

            //TheDockGroup.HasNoChildrenEvent += 
            //    TheDockGroup_HasNoChildrenEvent;

            TheDockGroup.PossibleDockChangeInsideEvent += 
                TheDockGroup_PossibleDockChangeInsideEvent;

            this.Closing += FloatingWindow_Closing;

            this.Closed += FloatingWindow_Closed;

            TheDockGroup.IsDockVisibleChangedEvent += 
                TheDockGroup_IsDockVisibleChangedEvent;

            _subscription = 
                TheDockGroup.DockChangedWithin.Subscribe(OnDockChangedWithin);
        }

        #region
        // the following functionality is used in 
        private bool _isCloseAllowed = true;

        internal void SetCloseIsNotAllowed()
        {
            _isCloseAllowed = false;
        }

        internal void ResetIsCloseAllowed()
        {
            _isCloseAllowed = true;
        }
        #endregion

        private void OnDockChangedWithin(Unit obj)
        {
            CanClose = !TheDockGroup.HasStableDescendant;

            CloseIfNeeded();
        }

        private void FloatingWindow_Closed(object? sender, EventArgs e)
        {
            var allGroups = TheDockGroup.GetDockGroupSelfAndDescendants().Reverse().ToList();

            foreach (var group in allGroups)
            {
                group.RemoveItselfFromParent();
                group.TheDockManager = null;
            }
        }

        internal void CloseIfNeeded()
        {
            if (CanClose && this.LeafItems.Count() == 0 && AutoDestroy && _isCloseAllowed)
            {
                this.Close();
            }
        }

        private void TheDockGroup_IsDockVisibleChangedEvent(IDockGroup obj)
        {

        }

        private void TheDockGroup_PossibleDockChangeInsideEvent(RootDockGroup obj)
        {
            CanReattachToDefaultGroup = 
                LeafItemsWithDefaultPosition
                    .Where(item => item.IsAllowedToReattachToDefaultGroup())
                    .Any();
        }

        public void DoInvalidateStyles()
        {
            this.InvalidateArrange();
            this.InvalidateStyles();
        }

        protected virtual void BeforeClosing(CancelEventArgs e)
        {
            TheDockManager = null;
        }

        private void FloatingWindow_Closing(object? sender, CancelEventArgs e)
        {
            BeforeClosing(e);
        }

        public void SetMovePtr()
        {
            SetInitialPosition();
            this.Activated += CustomWindow_Activated!;
        }

        private void CustomWindow_Activated(object sender, EventArgs e)
        {
            this.Activated -= CustomWindow_Activated!;
            SetInitialPosition();
            SetDragOnMovePointer();
        }

        private void SetInitialPosition()
        {
            StartPointerPosition = CurrentScreenPointBehavior.CurrentScreenPointValue.ToPixelPoint();
            StartWindowPosition = StartPointerPosition - new PixelPoint(60, 10);
            Position = StartWindowPosition;
        }

        protected override void SetDragOnMovePointer()
        {
            TheDockManager!.DraggedWindow = this;

            CurrentScreenPointBehavior.Capture(HeaderControl);

            base.SetDragOnMovePointer();
        }

        protected override void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(sender, e);

            TheDockManager?.CompleteDragDropAction();
        }

        private IEnumerable<ILeafDockObj> GetLeafGroups() => 
            this.TheDockGroup.GetLeafGroups();

        public IEnumerable<DockItem> LeafItems => TheDockGroup.LeafItems;

        public IEnumerable<DockItem> LeafItemsWithDefaultPosition =>
            LeafItems.Where(item => !item.DefaultDockGroupId.IsNullOrEmpty());

        #region CanReattachToDefaultGroup Styled Avalonia Property
        public bool CanReattachToDefaultGroup
        {
            get { return GetValue(CanReattachToDefaultGroupProperty); }
            private set { SetValue(CanReattachToDefaultGroupProperty, value); }
        }

        public static readonly StyledProperty<bool> CanReattachToDefaultGroupProperty =
            AvaloniaProperty.Register<FloatingWindow, bool>
            (
                nameof(CanReattachToDefaultGroup)
            );
        #endregion CanReattachToDefaultGroup Styled Avalonia Property

        public void ReattachToDefaultGroup()
        {
            LeafItemsWithDefaultPosition
                .Where(item => item.IsAllowedToReattachToDefaultGroup())
                .ToList()
                .DoForEach(item => item.ReattachToDefaultGroup());
        }
    }
}
