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
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public class FloatingWindow : CustomWindow
    {
        public SimpleDockGroup TheDockGroup { get; } = 
            new SimpleDockGroup();

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

        public FloatingWindow()
        {
            DragOnBeginMove = false;

            Classes = new Classes(new[] { "PlainFloatingWindow" });
            HasCustomWindowFeatures = true;
            Content = TheDockGroup;

            TheDockGroup.HasNoChildrenEvent += TheDockGroup_HasNoChildrenEvent;

            this.Closing += FloatingWindow_Closing;
        }

        public FloatingWindow(DockManager dockManager) : this()
        {
            DockAttachedProperties.SetTheDockManager(this, dockManager);
        }

        private void TheDockGroup_HasNoChildrenEvent(SimpleDockGroup obj)
        {
            if ((TheDockGroup as IDockGroup).AutoDestroy)
            {
                this.Close();
            }
        }

        private void FloatingWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            TheDockManager = null;

            var allGroups = TheDockGroup.GetDockGroupSelfAndDescendants().Reverse().ToList();

            allGroups?.DoForEach(item => item.RemoveItselfFromParent());
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
                return GetLeafGroups(TheDockManager!).SelectMany(g => g.LeafItems);
            }
        }
    }
}
