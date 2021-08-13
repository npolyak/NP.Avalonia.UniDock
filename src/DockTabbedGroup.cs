using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DockTabbedGroup : TemplatedControl
    {
        IDisposable? _behavior;

        private AvaloniaList<DockItem> _items = new AvaloniaList<DockItem>();

        SingleSelectionFirstByDefaultBehavior<DockItem> _singleSelectionBehavior =
            new SingleSelectionFirstByDefaultBehavior<DockItem>();

        public DockManager TheDockManager =>
            DockAttachedProperties.GetTheDockManager(this);

        public DockTabbedGroup()
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
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetSelectedItem();
        }

        private void SetSelectedItem()
        {
            SelectedItem = _singleSelectionBehavior.TheSelectedItem;
        }

        public void ClearSelectedItem()
        {
            _singleSelectionBehavior.TheSelectedItem = null;
        }

        public void SelectFirst()
        {
            _singleSelectionBehavior.SelectFirst();
        }

        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<DockTabbedGroup, AvaloniaList<DockItem>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<DockTabbedGroup, AvaloniaList<DockItem>>
            (
                nameof(Items), 
                o => o.Items, 
                (o, v) => o.Items = v);

        /// <summary>
        /// Gets or sets the items to display.
        /// </summary>
        [Content]
        public AvaloniaList<DockItem> Items
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

        #region SelectedItem Styled Avalonia Property
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<DockTabbedGroup, object>
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
            AvaloniaProperty.Register<DockTabbedGroup, Dock>
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
            AvaloniaProperty.Register<DockTabbedGroup, HorizontalAlignment>
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
            AvaloniaProperty.Register<DockTabbedGroup, VerticalAlignment>
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
            AvaloniaProperty.Register<DockTabbedGroup, bool>
            (
                nameof(ShowCompass)
            );
        #endregion ShowCompass Styled Avalonia Property


        public DropPanelWithCompass? DropPanel =>
            this.GetVisualDescendants().OfType<DropPanelWithCompass>().FirstOrDefault();

        public GroupDock? CurrentGroupDock =>
            DropPanel?.DockSide;

        private void SetBehavior()
        {
            _behavior = _items?.AddBehavior(OnItemAdded, OnItemRemoved);
            _singleSelectionBehavior.TheCollection = _items;
        }

        private void DisposeBehavior()
        {
            _behavior?.Dispose();

            _behavior = null;

            _singleSelectionBehavior.TheCollection = null; 
        }

        private void OnItemAdded(DockItem item)
        {
            item.RemoveEvent += Item_RemoveEvent;
        }

        private void Item_RemoveEvent(IRemovable item)
        {
            this.Items.Remove((DockItem) item);
        }

        private void OnItemRemoved(DockItem item)
        {
            item.RemoveEvent -= Item_RemoveEvent;
        }

        public void ClearAllItems()
        {
            if (Items == null)
                return;

            CollectionUtils.RemoveAll(Items);
        }
    }
}
