using Avalonia;
using Avalonia.Controls.Primitives;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class DockItem : HeaderedContentControl, IRemovable, ISelectableItem<DockItem>, IDockGroup
    {
        #region IsSelectedProperty Direct Avalonia Property
        public static readonly DirectProperty<DockItem, bool> IsSelectedProperty =
            AvaloniaProperty.RegisterDirect<DockItem, bool>
            (
                nameof(IsSelected),
                o => o.IsSelected, 
                (o, v) => o.IsSelected = v
            );
        #endregion IsSelectedProperty Direct Avalonia Property

        private bool _isSelected = false;
        public bool IsSelected 
        {
            get => _isSelected;
            set
            {
                SetAndRaise(IsSelectedProperty, ref _isSelected, value);
            }
        }

        public DockManager? TheDockManager { get; set; }

        public IDockGroup? DockParent { get; set; }

        // dock item is the end item, so it has no dock children.
        public IList<IDockGroup>? DockChildren => null;

        public event Action<IRemovable>? RemoveEvent;
        public event Action<DockItem>? IsSelectedChanged;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        public void Select()
        {
            IsSelected = true;
        }

        public override string ToString() =>
            $"TheDockItem: {Header?.ToString()} IsSelected={IsSelected}";

        private void FireSelectionChanged()
        {
            IsSelectedChanged?.Invoke(this);
        }

        static DockItem()
        {
            IsSelectedProperty.Changed.Subscribe(OnIsSelectedChanged);
        }

        private static void OnIsSelectedChanged(AvaloniaPropertyChangedEventArgs<bool> change)
        {
            DockItem dockItem = (DockItem)change.Sender;

            dockItem.FireSelectionChanged();

        }
    }
}
