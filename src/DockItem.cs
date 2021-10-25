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
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using NP.Avalonia.UniDockService;
using NP.Avalonia.Visuals.Behaviors;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;

namespace NP.Avalonia.UniDock
{
    public class DockItem :
        HeaderedContentControl, 
        ILeafDockObj,
        ISelectableItem<DockItem>,
        IActiveItem<DockItem>
    {
        public event Action<IDockGroup>? IsDockVisibleChangedEvent;
        
        IDictionary<IDockGroup, IDisposable> IDockGroup.ChildSubscriptions => 
            throw new NotImplementedException();

        public Subject<Unit> DockChangedWithin { get; } = new Subject<Unit>();

        // IsActive in current top level (root) group (or in floating window) changed
        public event Action<DockItem>? IsActiveChanged;

        public GroupKind TheGroupKind => GroupKind.DockItem;

        public bool HasStableDescendant => false;

        #region IsActive Styled Avalonia Property
        /// <summary>
        /// IsActive in current top level (root) group (or in floating window)
        /// </summary>
        public bool IsActive
        {
            get { return GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<DockItem, bool>
            (
                nameof(IsActive)
            );
        #endregion IsActive Styled Avalonia Property


        #region ShowHeader Styled Avalonia Property
        public bool ShowHeader
        {
            get { return GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly AttachedProperty<bool> ShowHeaderProperty =
            DockAttachedProperties.ShowHeaderProperty.AddOwner<DockItem>();
        #endregion ShowHeader Styled Avalonia Property

        void IDockGroup.FireIsDockVisibleChangedEvent()
        {
            IsDockVisibleChangedEvent?.Invoke(this);

            IDockGroup topDockGroup = this.GetDockGroupRoot();
            DockStaticEvents.FirePossibleDockChangeHappenedInsideEvent(topDockGroup);

            this.FireChangeWithin();
        }

        public bool IsPredefined { get; set; } = true;

        public bool AutoInvisible { get => false; set { } }

        #region CanFloat Styled Avalonia Property
        public bool CanFloat
        {
            get { return GetValue(CanFloatProperty); }
            set { SetValue(CanFloatProperty, value); }
        }

        public static readonly StyledProperty<bool> CanFloatProperty =
            AvaloniaProperty.Register<DockItem, bool>
            (
                nameof(CanFloat),
                true
            );
        #endregion CanFloat Styled Avalonia Property


        #region CanClose Styled Avalonia Property
        public bool CanClose
        {
            get { return GetValue(CanRemoveProperty); }
            set { SetValue(CanRemoveProperty, value); }
        }

        public static readonly StyledProperty<bool> CanRemoveProperty =
            AvaloniaProperty.Register<DockItem, bool>
            (
                nameof(CanClose),
                true
            );
        #endregion CanClose Styled Avalonia Property

        public event Action<IDockGroup>? DockIdChanged;

        #region DockId Styled Avalonia Property
        public string DockId
        {
            get { return GetValue(DockIdProperty); }
            set { SetValue(DockIdProperty, value); }
        }

        public static readonly StyledProperty<string> DockIdProperty =
            DockGroupBaseControl.DockIdProperty.AddOwner<DockItem>();
        #endregion Id Styled Avalonia Property

        private void FireDockIdChanged()
        {
            DockIdChanged?.Invoke(this);
        }

        static DockItem()
        {
            DockIdProperty
                .Changed
                .AddClassHandler<DockItem>((g, e) => g.OnDockIdChanged(e));
            IsSelectedProperty
                .Changed
                .Subscribe(OnIsSelectedChanged);
        }


        public DockItem()
        {
            this.GetObservable(IsActiveProperty)
                .Subscribe(OnIsActiveInWindowChanged);

            this.GetObservable(DockAttachedProperties.IsDockVisibleProperty)
                .Subscribe(OnIsDockVisibleChanged);

            this.GetObservable(ContentTemplateResourceKeyProperty)
                .Subscribe(OnContentTemplateResourceKeyChanged!);

            this.GetObservable(HeaderContentTemplateResourceKeyProperty)
                .Subscribe(OnHeaderContentTemplateResourceKeyChanged!);

            ShowHeader = true;
        }

        private void OnHeaderContentTemplateResourceKeyChanged(string newResourceKey)
        {
            if (DockParent != null)
            {
                if (DockParent?.IsAttachedToLogicalTree == true)
                {
                    TrySetHeaderContentTemplate();
                }
            }
        }

        private void OnContentTemplateResourceKeyChanged(string newResourceKey)
        {
            if (DockParent != null)
            {
                if (DockParent?.IsAttachedToLogicalTree == true)
                {
                    TrySetContentTemplate();
                }
            }
        }


        #region HeaderContentTemplateResourceKey Styled Avalonia Property
        public string? HeaderContentTemplateResourceKey
        {
            get { return GetValue(HeaderContentTemplateResourceKeyProperty); }
            set { SetValue(HeaderContentTemplateResourceKeyProperty, value); }
        }

        public static readonly StyledProperty<string?> HeaderContentTemplateResourceKeyProperty =
            AvaloniaProperty.Register<DockItem, string?>
            (
                nameof(HeaderContentTemplateResourceKey)
            );
        #endregion HeaderContentTemplateResourceKey Styled Avalonia Property


        #region ContentTemplateResourceKey Styled Avalonia Property
        public string? ContentTemplateResourceKey
        {
            get { return GetValue(ContentTemplateResourceKeyProperty); }
            set { SetValue(ContentTemplateResourceKeyProperty, value); }
        }

        public static readonly StyledProperty<string?> ContentTemplateResourceKeyProperty =
            AvaloniaProperty.Register<DockItem, string?>
            (
                nameof(ContentTemplateResourceKey)
            );
        #endregion ContentTemplateResourceKey Styled Avalonia Property


        private void OnIsDockVisibleChanged(bool isDockVisible)
        {
            if (!isDockVisible)
            {
                this.DropPanel?.FinishPointerDetection();
            }
        }

        private void OnIsActiveInWindowChanged(bool isActiveInWindow)
        {
            IsActiveChanged?.Invoke(this);
        }

        private void OnDockIdChanged(AvaloniaPropertyChangedEventArgs e)
        {
            FireDockIdChanged();
        }

        #region IsSelectedProperty Direct Avalonia Property
        public static readonly DirectProperty<DockItem, bool> IsSelectedProperty =
            AvaloniaProperty.RegisterDirect<DockItem, bool>
            (
                nameof(IsSelected),
                o => o.IsSelected, 
                (o, v) => o.IsSelected = v
            );
        #endregion IsSelectedProperty Direct Avalonia Property

        public Point FloatingSize { get; set; } = new Point(700, 400);

        public DockItemPresenter? TheVisual { get; internal set; }

        public IControl GetVisual() => (TheVisual as IControl) ?? this;

        public IDockGroup? GetContainingGroup() => DockParent;

        private bool _isSelected = false;
        public bool IsSelected 
        {
            get => _isSelected;
            set
            {
                SetAndRaise(IsSelectedProperty, ref _isSelected, value);
            }
        }

        public DropPanelWithCompass? DropPanel =>
            this?.TheVisual
                ?.GetVisualDescendants()
                ?.OfType<DropPanelWithCompass>().FirstOrDefault();

        public DockKind? CurrentGroupDock =>
            DropPanel?.DockSide;

        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value!);
        }


        private bool _isAtContentInitStage = true;

        private bool _isAtHeaderContentInitStage = true;

        private bool IsAtContentInitState =>
            _isAtContentInitStage &&
            DockParent != null &&
            ContentTemplateResourceKey != null &&
            ContentTemplate == null;

        private bool IsAtHeaderContentInitState =>
            _isAtHeaderContentInitStage &&
            DockParent != null &&
            HeaderContentTemplateResourceKey != null &&
            HeaderTemplate == null;


        IDockGroup? _dockParent;
        public IDockGroup? DockParent
        { 
            get => _dockParent; 
            set
            {
                if (_dockParent == value)
                    return;

                _dockParent = value;

                if ((this as ILogical).IsAttachedToLogicalTree)
                {
                    if (IsAtHeaderContentInitState)
                    {
                        TrySetHeaderContentTemplate();
                    }
                    if (IsAtContentInitState)
                    {
                        TrySetContentTemplate();
                    }
                }
                else if (DockParent != null)
                {
                    DockParent!.AttachedToLogicalTree += _dockParent_AttachedToLogicalTree;
                }

                this.SetCanReattachToDefaultGroup();
            }
        }


        private void TrySetContentTemplate()
        {
            if (IsAtContentInitState)
            {
                var dataTemplate = 
                    DockParent.GetResource<DataTemplate>(ContentTemplateResourceKey!);
                
                if (dataTemplate != null)
                {
                    ContentTemplate = dataTemplate;

                    _isAtContentInitStage = false;
                }
            }
        }

        private void TrySetHeaderContentTemplate()
        {
            if (IsAtHeaderContentInitState)
            {
                var dataTemplate =
                    DockParent.GetResource<DataTemplate>(HeaderContentTemplateResourceKey!);

                if (dataTemplate != null)
                {
                    HeaderTemplate = dataTemplate;

                    _isAtHeaderContentInitStage = false;
                }
            }
        }

        private void _dockParent_AttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs e)
        {
            TrySetHeaderContentTemplate();
            TrySetContentTemplate();

            if (DockParent != null)
            {
                DockParent.AttachedToLogicalTree -= _dockParent_AttachedToLogicalTree;
                DockParent.AttachedToLogicalTree -= _dockParent_AttachedToLogicalTree;
            }
        }

        // dock item is the end item, so it has no dock children.
        public IList<IDockGroup>? DockChildren => null;

        public event Action<IRemovable>? RemoveEvent;
        public event Action<DockItem>? IsSelectedChanged;

        public bool AutoDestroy
        {
            get => false;
            set
            {

            }
        }

        public void Remove()
        {
            IDockGroup? parent = DockParent;

            IDockGroup topDockGroup = this.GetDockGroupRoot();

            RemoveEvent?.Invoke(this);

            parent?.Simplify();

            DockStaticEvents.FirePossibleDockChangeHappenedInsideEvent(topDockGroup);
        }

        public void Select()
        {
            IsSelected = true;
        }

        const string NO_HEADER = "NO_HEADER";

        public override string ToString() =>
            $"TheDockItem: {DockId} {Header?.ToString()} { HeaderTemplate?.ToString()?? NO_HEADER} {Content?.ToString()}";

        private void FireSelectionChanged()
        {
            IsSelectedChanged?.Invoke(this);
        }

        private static void OnIsSelectedChanged(AvaloniaPropertyChangedEventArgs<bool> change)
        {
            DockItem dockItem = (DockItem)change.Sender;

            dockItem.OnIsSelectedChanged();
        }

        private void OnIsSelectedChanged()
        {
            if (IsSelected)
            {
                IsActive = true;
            }

            this.FireSelectionChanged();
        }

        #region ShowCompass Styled Avalonia Property
        public bool ShowCompass
        {
            get { return GetValue(ShowCompassProperty); }
            set { SetValue(ShowCompassProperty, value); }
        }

        public static readonly StyledProperty<bool> ShowCompassProperty =
            DockAttachedProperties.ShowCompassProperty.AddOwner<DockItem>();
        #endregion ShowCompass Styled Avalonia Property

        public void CleanSelfOnRemove()
        {
            this.DropPanel?.FinishPointerDetection();

            if (this.DropPanel != null)
            {
                this.DropPanel.CanStartPointerDetection = false;
            }

            if (Header is IControl headerControl)
            {
                headerControl.DisconnectVisualParentContentPresenter();
            }

            if (Content is IControl contentControl)
            {
                contentControl.DisconnectVisualParentContentPresenter();
            }

            //this.TheDockManager = null;

            if (this.TheVisual != null)
            {
                TheVisual.DockContext = null;
                this.TheVisual = null;
            }

            IsSelected = false;
        }

        void IDockGroup.SimplifySelf()
        {

        }

        #region DefaultDockOrderInGroup Styled Avalonia Property
        public double DefaultDockOrderInGroup
        {
            get { return GetValue(DefaultDockOrderInGroupProperty); }
            set { SetValue(DefaultDockOrderInGroupProperty, value); }
        }

        public static readonly StyledProperty<double> DefaultDockOrderInGroupProperty =
            AvaloniaProperty.Register<DockItem, double>
            (
                nameof(DefaultDockOrderInGroup)
            );
        #endregion DefaultDockOrderInGroup Styled Avalonia Property

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
    }
}
