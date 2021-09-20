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
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Metadata;
using NP.Avalonia.UniDock.Factories;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NP.Avalonia.UniDock
{
    public class StackDockGroup : DockIdContainingControl, IDockGroup, IDisposable
    {
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

        private IDockVisualItemGenerator? TheDockVisualItemGenerator { get; set; } =
            new DockVisualItemGenerator();

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
                int idx = DockChildren.IndexOf(dockChild);
                RemoveChildFromStackGroup(idx);
            }
        }

        private void AddChildToStackGroup(IDockGroup dockChild)
        {
            if (!dockChild.GetIsDockVisible())
            {
                return;
            }
            
            IControl newVisualChildToInsert =
               TheDockVisualItemGenerator!.Generate(dockChild);

            int idx = DockChildren.IndexOf(dockChild);

            _stackGroup.Items.Insert(idx, newVisualChildToInsert);

            this.SetIsDockVisible();
        }

        private void OnDockChildRemoved(IEnumerable<IDockGroup> groups, IDockGroup dockChild, int idx)
        {
            dockChild.IsDockVisibleChangedEvent -= OnDockChild_IsDockVisibleChangedEvent;

            dockChild.CleanSelfOnRemove();
            SetNumberDockChildren();

            RemoveChildFromStackGroup(idx);
        }

        private void RemoveChildFromStackGroup(int idx)
        {
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
