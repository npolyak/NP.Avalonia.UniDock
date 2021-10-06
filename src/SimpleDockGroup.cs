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

        public bool IsStable
        {
            get => true;
            set
            {

            }
        }

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

        static SimpleDockGroup()
        {
            DockIdProperty.Changed.AddClassHandler<SimpleDockGroup>((g, e) => g.OnDockIdChanged(e));
        }

        private IDisposable? _addRemoveChildBehavior;
        private SetDockGroupBehavior? _setBehavior;
        public SimpleDockGroup()
        {
            AffectsMeasure<SimpleDockGroup>(NumberDockChildrenProperty);

            _setBehavior = new SetDockGroupBehavior(this, DockChildren!);

            _addRemoveChildBehavior = 
                DockChildren.AddBehavior(OnChildAdded, OnChildRemoved);

            DockStaticEvents.PossibleDockChangeHappenedInsideEvent +=
                DockStaticEvents_PossibleDockChangeHappenedInsideEvent;

            _singleActiveBehavior.ActiveItemChangedEvent += _singleActiveBehavior_ActiveItemChangedEvent;
        }

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

        internal void MakeFirstItemActive()
        {
            _singleActiveBehavior.MakeFirstItemActive();
        }

        internal void MakeFirstItemActiveIfNoActive()
        {
            _singleActiveBehavior.MakeFirstItemActiveIfNoActive();
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
        }


        private void OnChildRemoved(IDockGroup childToRemove)
        {
            Control visualChildToRemove = FindVisualChild(childToRemove);

            ((ISetLogicalParent)visualChildToRemove).SetParent(null);
            VisualChildren.Remove(visualChildToRemove);
            LogicalChildren.Remove(visualChildToRemove);

            NumberDockChildren = DockChildren?.Count() ?? 0;
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
