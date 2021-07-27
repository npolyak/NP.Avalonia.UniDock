using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using NP.Concepts.Behaviors;
using System;

namespace DockWindowsSample
{
    public class DockTabbedGroup : TemplatedControl
    {
        IDisposable? _behavior;

        private AvaloniaList<DockItem> _items = new AvaloniaList<DockItem>();

        SingleSelectionBehavior<DockItem> _singleSelectionBehavior =
            new SingleSelectionBehavior<DockItem>();

        public DockTabbedGroup()
        {
            SetBehavior();
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


        #region SelectedItem Direct Avalonia Property
        public static readonly DirectProperty<DockTabbedGroup, DockItem> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<DockTabbedGroup, DockItem>
            (
                nameof(SelectedItem),
                o => o.SelectedItem,
                (o, v) => o.SelectedItem = v
            );
        #endregion SelectedItem Direct Avalonia Property


        public DockItem SelectedItem
        {
            get => _singleSelectionBehavior.TheSelectedItem;

            set
            {
                value.IsSelected = true;
            }
        }

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
    }
}
