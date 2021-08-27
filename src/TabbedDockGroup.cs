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
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public class TabbedDockGroup : TemplatedControl, ILeafDockObj
    {
        public event Action<IDockGroup>? DockIdChanged;

        #region DockId Styled Avalonia Property
        public string DockId
        {
            get { return GetValue(DockIdProperty); }
            set { SetValue(DockIdProperty, value); }
        }

        public static readonly StyledProperty<string> DockIdProperty =
            AvaloniaProperty.Register<TabbedDockGroup, string>
            (
                nameof(DockId)
            );
        #endregion Id Styled Avalonia Property

        private void FireDockIdChanged()
        {
            DockIdChanged?.Invoke(this);
        }

        static TabbedDockGroup()
        {
            DockIdProperty.Changed.AddClassHandler<TabbedDockGroup>((g, e) => g.OnDockIdChanged(e));
        }

        private void OnDockIdChanged(AvaloniaPropertyChangedEventArgs e)
        {
            FireDockIdChanged();
        }

        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value);
        }

        IDisposable? _setItemsBehavior;

        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<TabbedDockGroup, IList<IDockGroup>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<TabbedDockGroup, IList<IDockGroup>>
            (
                nameof(Items),
                o => o.Items,
                (o, v) => o.Items = v);

        private IList<IDockGroup> _items = new ObservableCollection<IDockGroup>();
        /// <summary>
        /// Gets or sets the items to display.
        /// </summary>
        [Content]
        public IList<IDockGroup> Items
        {
            get
            {
                return _items;
            }

            set
            {
                DisposeBehavior();

                SetAndRaise(ItemsProperty, ref _items, value);

                SetBehavior();
            }
        }

        public IList<IDockGroup>? DockChildren => Items;

        #region NumberDockChildren Direct Avalonia Property
        public static readonly DirectProperty<TabbedDockGroup, int> NumberDockChildrenProperty =
            AvaloniaProperty.RegisterDirect<TabbedDockGroup, int>
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
        public DropPanelWithCompass? DropPanel =>
            this.GetVisualDescendants().OfType<DropPanelWithCompass>().FirstOrDefault();

        public DockKind? CurrentGroupDock =>
            DropPanel?.DockSide;

        public IDockGroup? DockParent { get; set; }

        private readonly SingleSelectionFirstByDefaultBehavior<DockItem> _singleSelectionBehavior =
            new SingleSelectionFirstByDefaultBehavior<DockItem>();

        private readonly MimicCollectionBehavior<IDockGroup, DockItem, ObservableCollection<DockItem>> _mimicCollectionBehavior =
            new MimicCollectionBehavior<IDockGroup, DockItem, ObservableCollection<DockItem>>(dockGroup => (DockItem)dockGroup);

        public TabbedDockGroup()
        {
            AffectsMeasure<DockTabsPresenter>(TabStripPlacementProperty);
            SetBehavior();

            _singleSelectionBehavior.PropertyChanged += 
                _singleSelectionBehavior_PropertyChanged;

            SetSelectedItem();
        }

        private void _singleSelectionBehavior_PropertyChanged
        (
            object? sender, 
            PropertyChangedEventArgs e)
        {
            SetSelectedItem();
        }

        private void SetSelectedItem()
        {
            SelectedItem = _singleSelectionBehavior.TheSelectedItem;
        }

        public void ClearSelectedItem()
        {
            _singleSelectionBehavior.TheSelectedItem = null!;
        }

        public void ClearSelfOnRemove()
        {
            ClearSelectedItem();
            this.TheDockManager = null;
        }

        public void SelectFirst()
        {
            _singleSelectionBehavior.SelectFirst();
        }


        #region SelectedItem Styled Avalonia Property
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<TabbedDockGroup, object>
            (
                nameof(SelectedItem)
            );
        #endregion SelectedItem Styled Avalonia Property

        #region TabStripPlacement Styled Avalonia Property
        public Dock TabStripPlacement
        {
            get { return GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            AvaloniaProperty.Register<TabbedDockGroup, Dock>
            (
                nameof(TabStripPlacement),
                Dock.Top
            );
        #endregion TabStripPlacement Styled Avalonia Property

        #region HorizontalContentAlignment Styled Avalonia Property
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<TabbedDockGroup, HorizontalAlignment>
            (
                nameof(HorizontalContentAlignment)
            );
        #endregion HorizontalContentAlignment Styled Avalonia Property


        #region VerticalContentAlignment Styled Avalonia Property
        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<TabbedDockGroup, VerticalAlignment>
            (
                nameof(VerticalContentAlignment)
            );
        #endregion VerticalContentAlignment Styled Avalonia Property


        #region ShowCompass Styled Avalonia Property
        public bool ShowCompass
        {
            get { return GetValue(ShowCompassProperty); }
            set { SetValue(ShowCompassProperty, value); }
        }

        public static readonly StyledProperty<bool> ShowCompassProperty =
            AvaloniaProperty.Register<TabbedDockGroup, bool>
            (
                nameof(ShowCompass)
            );
        #endregion ShowCompass Styled Avalonia Property

        IDisposable? _behavior;
        private void SetBehavior()
        {
            _setItemsBehavior = new SetDockGroupBehavior<IDockGroup>(this, Items);

            _mimicCollectionBehavior.InputCollection = Items;
            _singleSelectionBehavior.TheCollection = _mimicCollectionBehavior.OutputCollection;

            _behavior = Items?.AddBehavior(OnItemAdded, OnItemRemoved);
        }

        private void SetNumberItems()
        {
            NumberDockChildren = Items?.Count ?? 0;
        }

        private void OnItemAdded(IDockGroup child)
        {
            SetNumberItems();
        }

        private void OnItemRemoved(IDockGroup child)
        {
            SetNumberItems();

            child.TheDockManager = null;
        }

        private void DisposeBehavior()
        {
            _setItemsBehavior?.Dispose();

            _setItemsBehavior = null;

            _singleSelectionBehavior.TheCollection = null;

            _mimicCollectionBehavior.InputCollection = null;
        }


        public IEnumerable<DockItem> LeafItems => Items.NullToEmpty().Cast<DockItem>().ToList();

        public IDockGroup? GetContainingGroup() => this;

        public bool AutoDestroy
        {
            get; set;
        } = true;

        void IDockGroup.SimplifySelf()
        {
            this.SimplifySelfImpl();
        }
    }
}
