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
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.VisualTree;
using NP.Avalonia.UniDockService;
using NP.Avalonia.Visuals.Behaviors;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public class SimpleDockGroup : DockIdContainingControl, IDockGroup, IDisposable
    {
        public event Action<IDockGroup>? IsDockVisibleChangedEvent;

        public event Action<SimpleDockGroup>? PossibleDockChangeInsideEvent;

        void IDockGroup.FireIsDockVisibleChangedEvent()
        {
            IsDockVisibleChangedEvent?.Invoke(this);
        }

        public bool IsStableGroup
        {
            get => this.GetControlsWindow<FloatingWindow>()?.IsStable ?? true;
            set
            {

            }
        }

        public SimpleDockGroup? ParentWindowGroup { get; set; }

        // should never be called on the window root group
        public IDockGroup CloneIfStable() => throw new NotImplementedException();

        public event Action<SimpleDockGroup>? HasNoChildrenEvent;

        private SingleActiveBehavior<DockItem> _singleActiveBehavior = new SingleActiveBehavior<DockItem>();


        #region ActiveDockItem Direct Avalonia Property
        private DockItem? _ActiveDockItem = default;

        public static readonly DirectProperty<SimpleDockGroup, DockItem?> ActiveDockItemProperty =
            AvaloniaProperty.RegisterDirect<SimpleDockGroup, DockItem?>
            (
                nameof(ActiveDockItem),
                o => o.ActiveDockItem
            );

        public DockItem? ActiveDockItem
        {
            get => _ActiveDockItem;
            private set
            {
                SetAndRaise(ActiveDockItemProperty, ref _ActiveDockItem, value);
            }
        }

        #endregion ActiveDockItem Direct Avalonia Property


        #region NumberDockChildren Direct Avalonia Property
        public static readonly DirectProperty<SimpleDockGroup, int> NumberDockChildrenProperty =
            AvaloniaProperty.RegisterDirect<SimpleDockGroup, int>
            (
                nameof(NumberDockChildren),
                o => o.NumberDockChildren,
                (o, c) => o.NumberDockChildren = c
            );
        #endregion NumberDockChildren Direct Avalonia Property

        private int _numChildren = 0;
        public int NumberDockChildren
        {
            get => _numChildren;
            private set
            {
                SetAndRaise(NumberDockChildrenProperty, ref _numChildren,   value);
            }
        }

        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        public bool ShowChildHeader => false;

        public bool ShowChildHeaders { get; } = false;

        public GroupKind TheGroupKind => GroupKind.Simple;

        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value);
        }

        public IDockGroup? DockParent
        {
            get => null;
            set => throw new NotImplementedException();
        }

        [Content]
        public IDockGroup? TheChild
        {
            get => DockChildren?.FirstOrDefault();
            set
            {
                DockChildren.RemoveAllOneByOne();

                if (value != null)
                {
                    DockChildren.Add(value);
                }
            }
        }


        public IList<IDockGroup> DockChildren { get; } = 
            new ObservableCollection<IDockGroup>();


        #region FloatingWindows Direct Avalonia Property
        private IList<FloatingWindowContainer>? _floatingWindows = 
            new ObservableCollection<FloatingWindowContainer>();

        public static readonly DirectProperty<SimpleDockGroup, IList<FloatingWindowContainer>?> FloatingWindowsProperty =
            AvaloniaProperty.RegisterDirect<SimpleDockGroup, IList<FloatingWindowContainer>?>
            (
                nameof(FloatingWindows),
                o => o.FloatingWindows
            );

        public IList<FloatingWindowContainer>? FloatingWindows
        {
            get => _floatingWindows;
        }

        #endregion FloatingWindows Direct Avalonia Property


        static SimpleDockGroup()
        {
            DockIdProperty.Changed.AddClassHandler<SimpleDockGroup>((g, e) => g.OnDockIdChanged(e));
        }

        private IDisposable? _addRemoveChildBehavior;
        private SetDockGroupBehavior? _setBehavior;

        private readonly AttachedPropToCollectionBindingBehavior<DockManager, FloatingWindowContainer>? _floatingWindowDockManagerSettingBehavior;
        private readonly AttachedPropToCollectionBindingBehavior<Window, FloatingWindowContainer>? _floatingWindowParentWindowSettingBehavior;

        public SimpleDockGroup()
        {
            AffectsMeasure<SimpleDockGroup>(NumberDockChildrenProperty);

            _setBehavior = new SetDockGroupBehavior(this, DockChildren!);

            _addRemoveChildBehavior = 
                DockChildren.AddBehavior(OnChildAdded, OnChildRemoved);

            DockStaticEvents.PossibleDockChangeHappenedInsideEvent +=
                DockStaticEvents_PossibleDockChangeHappenedInsideEvent;

            _singleActiveBehavior.ActiveItemChangedEvent +=
                _singleActiveBehavior_ActiveItemChangedEvent;

            _floatingWindowDockManagerSettingBehavior =
                new AttachedPropToCollectionBindingBehavior<DockManager, FloatingWindowContainer>
                (
                    this, 
                    DockAttachedProperties.TheDockManagerProperty,
                    _floatingWindows,
                    (w, dm) => w.TheDockManager = dm
                );

            this.AttachedToVisualTree += SimpleDockGroup_AttachedToVisualTree;


            FloatingWindows.AddBehavior(OnFloatingWindowAdded);

            _floatingWindowParentWindowSettingBehavior = new AttachedPropToCollectionBindingBehavior<Window, FloatingWindowContainer>
            (
                this,
                OwningWindowProperty!,
                FloatingWindows,
                (floatingWindowContainer, ownerWindow) => floatingWindowContainer.ParentWindow = ownerWindow
            );
        }


        private void OnFloatingWindowAdded(FloatingWindowContainer floatingWindowContainer)
        {
            floatingWindowContainer.ParentWindowGroup = this;
        }

        private void SimpleDockGroup_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            OwningWindow = this.GetVisualAncestors().OfType<Window>().FirstOrDefault();
        }

        #region OwningWindow Direct Avalonia Property
        private Window? _owningWindow = default;

        public static readonly DirectProperty<SimpleDockGroup, Window?> OwningWindowProperty =
            AvaloniaProperty.RegisterDirect<SimpleDockGroup, Window?>
            (
                nameof(OwningWindow),
                o => o.OwningWindow
            );

        public Window? OwningWindow
        {
            get => _owningWindow;
            private set
            {
                SetAndRaise(OwningWindowProperty, ref _owningWindow, value);
            }
        }

        #endregion OwningWindow Direct Avalonia Property


        private void _singleActiveBehavior_ActiveItemChangedEvent()
        {
            ActiveDockItem = _singleActiveBehavior.TheActiveItem;
        }

        private void DockStaticEvents_PossibleDockChangeHappenedInsideEvent(IDockGroup group)
        {
            if (group != this)
                return;

            PossibleDockChangeInsideEvent?.Invoke(this);

            _singleActiveBehavior.TheCollection = LeafItems.ToList();
        }

        private Control FindVisualChild(IDockGroup dockChild)
        {
            IControl control = dockChild;
            if (dockChild is ILeafDockObj leafDockChild)
            {
                control = leafDockChild.GetVisual();
            }

            return (Control)LogicalChildren.OfType<IControl>().FirstOrDefault(item => ReferenceEquals(item, control))!;
        }


        private void OnChildAdded(IDockGroup newChildToInsert)
        {
            // have to remove previous children before adding the new one.
            // Only one child is allowed
            var childrenToRemove = DockChildren.Except(newChildToInsert.ToCollection()).ToList();

            childrenToRemove.DoForEach(child => DockChildren.Remove(child));

            IControl newVisualChildToInsert =
                TheDockManager!.TheDockVisualItemGenerator!.Generate(newChildToInsert)!;

            ((ISetLogicalParent)newVisualChildToInsert).SetParent(this);
            VisualChildren.Add(newVisualChildToInsert);
            LogicalChildren.Add(newVisualChildToInsert);

            NumberDockChildren = DockChildren?.Count() ?? 0;
            newChildToInsert.IsDockVisibleChangedEvent += OnChildIsDockVisibleChanged;
            this.SetIsDockVisible();
        }

        private void OnChildIsDockVisibleChanged(IDockGroup obj)
        {
            this.SetIsDockVisible();
        }

        private void OnChildRemoved(IDockGroup childToRemove)
        {
            childToRemove.IsDockVisibleChangedEvent -= OnChildIsDockVisibleChanged;
            Control visualChildToRemove = FindVisualChild(childToRemove);

            ((ISetLogicalParent)visualChildToRemove).SetParent(null);
            VisualChildren.Remove(visualChildToRemove);
            LogicalChildren.Remove(visualChildToRemove);

            NumberDockChildren = DockChildren?.Count() ?? 0;
            this.SetIsDockVisible();
        }

        public void Dispose()
        {
            _setBehavior?.Dispose();
            _setBehavior = null;

            _addRemoveChildBehavior?.Dispose();
            _addRemoveChildBehavior = null;
        }

        void IDockGroup.SimplifySelf()
        {
            if (NumberDockChildren == 0)
            {
                HasNoChildrenEvent?.Invoke(this);
            }    
        }

        public bool AutoDestroy { get; set; } = true;

        public IEnumerable<DockItem> LeafItems => this.GetLeafItems();
    }
}
