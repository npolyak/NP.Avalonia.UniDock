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
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using NP.Avalonia.Visuals.Behaviors;

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

        public bool AutoInvisible { get; set; } = true;

        public Point FloatingSize { get; set; } = new Point(700, 400);

        public StackGroup _stackGroup = new StackGroup();


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


        #region InitialSizeCoefficients Styled Avalonia Property
        public string? InitialSizeCoefficients
        {
            get { return GetValue(InitialSizeCoefficientsProperty); }
            set { SetValue(InitialSizeCoefficientsProperty, value); }
        }

        public static readonly StyledProperty<string?> InitialSizeCoefficientsProperty =
            AvaloniaProperty.Register<StackDockGroup, string?>
            (
                nameof(InitialSizeCoefficients)
            );
        #endregion InitialSizeCoefficients Styled Avalonia Property

        private GridLength[]? _initSizeCoefficients;
        private List<GridLength> _sizeCoefficients = new List<GridLength>();

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

        #region HeaderBackground Styled Avalonia Property
        public IBrush HeaderBackground
        {
            get { return GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public static readonly StyledProperty<IBrush> HeaderBackgroundProperty =
            AvaloniaProperty.Register<StackDockGroup, IBrush>
            (
                nameof(HeaderBackground)
            );
        #endregion HeaderBackground Styled Avalonia Property


        #region HeaderForeground Styled Avalonia Property
        public IBrush HeaderForeground
        {
            get { return GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        public static readonly StyledProperty<IBrush> HeaderForegroundProperty =
            AvaloniaProperty.Register<StackDockGroup, IBrush>
            (
                nameof(HeaderForeground)
            );
        #endregion HeaderForeground Styled Avalonia Property

        #region ShowHeader Styled Avalonia Property
        public bool ShowHeader
        {
            get { return GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly AttachedProperty<bool> ShowHeaderProperty =
            DockAttachedProperties.ShowHeaderProperty.AddOwner<StackDockGroup>();
        #endregion ShowHeader Styled Avalonia Property


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
                this.SetCanReattachToDefaultGroup();
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


        #region HeaderControlTemplate Styled Avalonia Property
        public ControlTemplate HeaderControlTemplate
        {
            get { return GetValue(HeaderControlTemplateProperty); }
            set { SetValue(HeaderControlTemplateProperty, value); }
        }

        public static readonly StyledProperty<ControlTemplate> HeaderControlTemplateProperty =
            AvaloniaProperty.Register<StackDockGroup, ControlTemplate>
            (
                nameof(HeaderControlTemplate)
            );
        #endregion HeaderControlTemplate Styled Avalonia Property

        
        string? _defaultDockGroupId;
        public string? DefaultDockGroupId
        {
            get => _defaultDockGroupId;
            set
            {
                if (_defaultDockGroupId == value)
                    return;

                _defaultDockGroupId = value;

                this.SetCanReattachToDefaultGroup();
            }
        }


        public StackDockGroup()
        {
            AffectsMeasure<StackDockGroup>(NumberDockChildrenProperty);
            AffectsMeasure<StackDockGroup>(DockAttachedProperties.IsDockVisibleProperty);
            AffectsMeasure<StackDockGroup>(HeaderControlTemplateProperty);

            // should go to the first row after the header
            Grid.SetRow(_stackGroup, 1);

            _setDockGroupBehavior = new SetDockGroupBehavior(this, DockChildren!);
            _behavior = DockChildren?.AddDetailedBehavior(OnDockChildAdded, OnDockChildRemoved);

            this.GetObservable(DockAttachedProperties.TheDockManagerProperty).Subscribe(OnDockManagerChanged);

            this.GetObservable(InitialSizeCoefficientsProperty).Subscribe(OnInitSizeCoeffsChanged!);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            Grid panel = e.NameScope.Find<Grid>("PART_RootPanel");

            _stackGroup.RemoveFromParentPanel();

            panel.Children.Add(_stackGroup);
        }

        private void OnInitSizeCoeffsChanged(string obj)
        {
            SetSizeCoefficientsFromInit();
        }

        private void SetSizeCoefficientsFromInit()
        {
            if (InitialSizeCoefficients == null)
                return;

            _initSizeCoefficients = 
                InitialSizeCoefficients
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => GridLength.Parse(str)).ToArray();

            InitialSizeCoefficients = null;
        }

        private void OnDockManagerChanged(DockManager dockManager)
        {
            if (TheDockManager != null)
            {
                _stackGroup.TheDockSeparatorFactory = TheDockManager?.TheDockSeparatorFactory;

                int i = 0;
                foreach (var child in DockChildren)
                {
                    AddChildToStackGroup(child);

                    if (i < _sizeCoefficients.Count)
                    {
                        SetSizeCoefficient(i, _sizeCoefficients[i]);
                    }

                    i++;
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

            GridLength sizeCoeff = 
               ((_initSizeCoefficients != null) && (idx < _initSizeCoefficients.Length)) ? _initSizeCoefficients[idx] : new GridLength(1, GridUnitType.Star);

            _sizeCoefficients.Insert(idx, sizeCoeff);

            dockChild.RemoveFromParentPanel();

            AddChildToStackGroup(dockChild);

            SetSizeCoefficient(idx, sizeCoeff);
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

        public GridLength GetSizeCoefficient(int idx)
        {
            int internalIdx = GetInternalIdx(idx);

            GridLength coeff = (internalIdx > -1) ? 
                            _stackGroup.GetSizeCoefficient(internalIdx) : _sizeCoefficients[idx];

            return coeff;
        }

        public void SetSizeCoefficient(int idx, GridLength coeff)
        {
            _sizeCoefficients[idx] = coeff;

            int internalIdx = GetInternalIdx(idx);

            if (internalIdx > -1)
            {
                _stackGroup.SetSizeCoefficient(internalIdx, coeff);
            }
        }

        public GridLength[] GetSizeCoefficients()
        {
            var result = new GridLength[NumberDockChildren];
            for(int i = 0; i < NumberDockChildren; i++)
            {
                result[i] = GetSizeCoefficient(i);
            }

            return result;
        }

        public void SetSizeCoefficients(GridLength[]? coeffs)
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
