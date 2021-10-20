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
using System.Linq;
using NP.Avalonia.UniDockService;

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

        private bool _isStableGroup = false;
        public bool IsStableGroup
        {
            get => _isStableGroup;
            set
            {
                if (_isStableGroup == value)
                    return;

                _isStableGroup = value;

                //this.SetStableParent();
            }
        }


        private List<double> _sizeCoefficients = new List<double>();

        public GroupKind TheGroupKind => GroupKind.Stack;

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


        private IDockGroup? _dockParent;
        public IDockGroup? DockParent
        {
            get => _dockParent;
            set
            {
                if (_dockParent == value)
                    return;

                _dockParent = value;

                this.SetStableParent();
            }
        }

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

                foreach (var child in DockChildren)
                {
                    AddChildToStackGroup(child);
                }
            }
            else
            {
                foreach (var child in DockChildren)
                {
                    RemoveChildFromStackGroup(child);
                }
            }
            this.SetIsDockVisible();
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

            _sizeCoefficients.Insert(idx, 1d);

            AddChildToStackGroup(dockChild);

            dockChild.IsDockVisibleChangedEvent += OnDockChild_IsDockVisibleChangedEvent;

            this.SetIsDockVisible();
        }

        private void OnDockChild_IsDockVisibleChangedEvent(IDockGroup dockChild)
        {
            if (dockChild.IsDockVisible)
            {
                AddChildToStackGroup(dockChild);
            }
            else
            {
                RemoveChildFromStackGroup(dockChild);
            }

            this.SetIsDockVisible();
        }

        private int GetInternalIdx(IDockGroup dockGroup)
        {
            int internalIdx = _stackGroup.Items.IndexOf(c => DockAttachedProperties.GetOriginalDockGroup(c) == dockGroup);

            return internalIdx;
        }

        private int GetInternalIdx(int idx)
        {
            return GetInternalIdx(DockChildren[idx]);
        }

        private int GetExternalIdx(IControl control)
        {
            IDockGroup? originalGroup = DockAttachedProperties.GetOriginalDockGroup(control);

            if (originalGroup == null)
            {
                throw new ProgrammingError("");
            }
            return this.DockChildren.IndexOf(originalGroup);
        }

        private int GetExternalIdx(int idx)
        {
            IControl c = _stackGroup.Items[idx];

            return GetExternalIdx(c);
        }

        private void AddChildToStackGroup(IDockGroup dockChild)
        {
            if (TheDockManager == null)
            {
                return;
            }

            if (!dockChild.IsDockVisible)
            {
                return;
            }

            IControl newVisualChildToInsert =
               TheDockManager!.TheDockVisualItemGenerator!.Generate(dockChild);

            DockAttachedProperties.SetOriginalDockGroup(newVisualChildToInsert, dockChild);

            int CompareGroups(IControl control1, IControl control2)
            {
                int idx1 = GetExternalIdx(control1);
                int idx2 = GetExternalIdx(control2);

                return idx1 > idx2 ? 1 : idx1 == idx2 ? 0 : -1;
            }

            bool hasAlready =
                _stackGroup.Items.Where(item => DockAttachedProperties.GetOriginalDockGroup(item) == dockChild).Any();

            if (!hasAlready)
            {
                _stackGroup.Items.InsertInOrder(newVisualChildToInsert, CompareGroups);
            }
        }

        private void OnDockChildRemoved(IEnumerable<IDockGroup> groups, IDockGroup dockChild, int idx)
        {
            _sizeCoefficients.RemoveAt(idx);

            dockChild.IsDockVisibleChangedEvent -= OnDockChild_IsDockVisibleChangedEvent;

            dockChild.CleanSelfOnRemove();
            SetNumberDockChildren();

            RemoveChildFromStackGroup(dockChild);

            this.SetIsDockVisible();
        }

        private void RemoveChildFromStackGroup(IDockGroup dockChild)
        {
            int idx = GetInternalIdx(dockChild);

            if (idx > -1)
            {
                if (DockChildren.Contains(dockChild))
                {
                    _sizeCoefficients[idx] = _stackGroup.GetSizeCoefficient(idx);
                }

                _stackGroup.Items.RemoveAt(idx);
            }
        }

        void IDockGroup.SimplifySelf()
        {
            this.SimplifySelfImpl();
        }

        public bool AutoDestroy { get; set; } = true;

        #region DefaultDockOrderInGroup Styled Avalonia Property
        public double DefaultDockOrderInGroup
        {
            get { return GetValue(DefaultDockOrderInGroupProperty); }
            set { SetValue(DefaultDockOrderInGroupProperty, value); }
        }

        public static readonly StyledProperty<double> DefaultDockOrderInGroupProperty =
            AvaloniaProperty.Register<StackDockGroup, double>
            (
                nameof(DefaultDockOrderInGroup)
            );
        #endregion DefaultDockOrderInGroup Styled Avalonia Property

        public double GetSizeCoefficient(int idx)
        {
            int internalIdx = GetInternalIdx(idx);

            double coeff = (internalIdx > -1) ? 
                            _stackGroup.GetSizeCoefficient(internalIdx) : _sizeCoefficients[idx];

            return coeff;
        }

        public void SetSizeCoefficient(int idx, double coeff)
        {
            _sizeCoefficients[idx] = coeff;

            int internalIdx = GetInternalIdx(idx);

            if (internalIdx > -1)
            {
                _stackGroup.SetSizeCoefficient(internalIdx, coeff);
            }
        }

        public double[] GetSizeCoefficients()
        {
            var result = new double[NumberDockChildren];
            for(int i = 0; i < NumberDockChildren; i++)
            {
                result[i] = GetSizeCoefficient(i);
            }

            return result;
        }

        public void SetSizeCoefficients(double[]? coeffs)
        {   
            if (coeffs == null)
            {
                return;
            }    

            for(int i = 0; i < coeffs.Length; i++)
            {
                SetSizeCoefficient(i, coeffs[i]);
            }
        }

        public IDockGroup CloneIfStable()
        {
            if (IsStableGroup)
            {
                StackDockGroup result = new StackDockGroup();
                result.AutoDestroy = this.AutoDestroy;
                result.TheOrientation = this.TheOrientation;

                result.TheDockManager = this.TheDockManager;

                var coefficients = this.GetSizeCoefficients();

                foreach (IDockGroup childGroup in this.DockChildren.ToList())
                {
                    result.DockChildren.Add(childGroup.CloneIfStable());
                }

                result.SetSizeCoefficients(coefficients);

                return result;
            }
            else
            {
                this.RemoveItselfFromParent();
                return this;
            }
        }
    }
}
