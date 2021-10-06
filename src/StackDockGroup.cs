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
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NP.Utilities;

namespace NP.Avalonia.UniDock
{
    public class StackDockGroup : DockIdContainingControl, IDockGroup, IDisposable, IStyleable
    {
        Type IStyleable.StyleKey => typeof(StackDockGroup);

        public event Action<IDockGroup>? IsDockVisibleChangedEvent;

        void IDockGroup.FireIsDockVisibleChangedEvent()
        {
            IsDockVisibleChangedEvent?.Invoke(this);
        }

        public StackGroup<IControl> _stackGroup = new StackGroup<IControl>();

        public bool ShowChildHeaders { get; } = true;

        public bool IsStableGroup { get; set; } = false;

        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value);
        }

        public Orientation TheOrientation
        {
            get => _stackGroup.TheOrientation;
            set => _stackGroup.TheOrientation = value;
        }
        #region NumberDockChildren Direct Avalonia Property
        public static readonly DirectProperty<StackDockGroup, int> NumberDockChildrenProperty =
            AvaloniaProperty.RegisterDirect<StackDockGroup, int>
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
                SetAndRaise(NumberDockChildrenProperty, ref _numChildren, value);
            }
        }

        public IDockGroup? DockParent { get; set; }

        [Content]
        public IList<IDockGroup> DockChildren { get; } = new ObservableCollection<IDockGroup>();

        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        
        static StackDockGroup()
        {
            DockIdProperty.Changed.AddClassHandler<StackDockGroup>((g, e) => g.OnDockIdChanged(e));
        }

        IDisposable? _behavior;
        IDisposable? _setDockGroupBehavior;

        public StackDockGroup()
        {
            AffectsMeasure<StackDockGroup>(NumberDockChildrenProperty);
            AffectsMeasure<StackDockGroup>(DockAttachedProperties.IsDockVisibleProperty);

            ((ISetLogicalParent)_stackGroup).SetParent(this);
            this.VisualChildren.Add(_stackGroup);
            this.LogicalChildren.Add(_stackGroup);

            _setDockGroupBehavior = new SetDockGroupBehavior(this, DockChildren!);
            _behavior = DockChildren?.AddDetailedBehavior(OnDockChildAdded, OnDockChildRemoved);

            this.GetObservable(DockAttachedProperties.TheDockManagerProperty).Subscribe(OnDockManagerChanged);
        }

        private void OnDockManagerChanged(DockManager dockManager)
        {
            if (TheDockManager != null)
            {
                _stackGroup.TheDockSeparatorFactory = TheDockManager?.TheDockSeparatorFactory;
            }
        }

        public void Dispose()
        {
            _setDockGroupBehavior?.Dispose();
            _setDockGroupBehavior = null;

            _behavior?.Dispose();
            _behavior = null!;
        }

        private void SetNumberDockChildren()
        {
            NumberDockChildren = DockChildren.Count;
        }

        private void OnDockChildAdded(IEnumerable<IDockGroup> groups, IDockGroup dockChild, int idx)
        {
            SetNumberDockChildren();
            AddChildToStackGroup(dockChild);

            dockChild.IsDockVisibleChangedEvent += OnDockChild_IsDockVisibleChangedEvent;
        }

        private void OnDockChild_IsDockVisibleChangedEvent(IDockGroup dockChild)
        {
            if (dockChild.GetIsDockVisible())
            {
                AddChildToStackGroup(dockChild);
            }
            else
            {
                RemoveChildFromStackGroup(dockChild);
            }
        }

        private void AddChildToStackGroup(IDockGroup dockChild)
        {
            if (!dockChild.GetIsDockVisible())
            {
                return;
            }
            
            IControl newVisualChildToInsert =
               TheDockManager!.TheDockVisualItemGenerator!.Generate(dockChild);

            DockAttachedProperties.SetOriginalDockGroup(newVisualChildToInsert, dockChild);

            int CompareGroups(IControl control1, IControl control2)
            {
                IDockGroup originalGroup1 =
                    DockAttachedProperties.GetOriginalDockGroup(control1)!;
                IDockGroup originalGroup2 =
                    DockAttachedProperties.GetOriginalDockGroup(control2)!;

                int idx1 = DockChildren.IndexOf(originalGroup1);
                int idx2 = DockChildren.IndexOf(originalGroup2);

                return idx1 > idx2 ? 1 : idx1 == idx2 ? 0 : -1;
            }

            _stackGroup.Items.InsertInOrder(newVisualChildToInsert, CompareGroups);

            this.SetIsDockVisible();
        }

        private void OnDockChildRemoved(IEnumerable<IDockGroup> groups, IDockGroup dockChild, int idx)
        {
            dockChild.IsDockVisibleChangedEvent -= OnDockChild_IsDockVisibleChangedEvent;

            dockChild.CleanSelfOnRemove();
            SetNumberDockChildren();

            RemoveChildFromStackGroup(dockChild);
        }

        private void RemoveChildFromStackGroup(IDockGroup dockChild)
        {
            int idx = 
                _stackGroup.Items
                           .IndexOf(item => DockAttachedProperties.GetOriginalDockGroup(item) == dockChild);

            _stackGroup.Items.RemoveAt(idx);

            this.SetIsDockVisible();
        }

        void IDockGroup.SimplifySelf()
        {
            this.SimplifySelfImpl();
        }

        public bool AutoDestroy { get; set; } = true;

        public double GetSizeCoefficient(int idx)
        {
            return _stackGroup.GetSizeCoefficient(idx);
        }

        public void SetSizeCoefficient(int idx, double coeff)
        {
            _stackGroup.SetSizeCoefficient(idx, coeff);
        }

        public double[] GetSizeCoefficients()
        {
            return _stackGroup.GetSizeCoefficients();
        }

        public void SetSizeCoefficients(double[]? coeffs)
        {
            _stackGroup.SetSizeCoefficients(coeffs);
        }
    }
}
