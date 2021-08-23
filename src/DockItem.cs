using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals.Behaviors;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DockItem :
        HeaderedContentControl, 
        ILeafDockObj,
        ISelectableItem<DockItem>
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

        public DockItemPresenter? TheVisual { get; internal set; }

        public IControl GetVisual() => (TheVisual as IControl) ?? this;

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
            this?.TheVisual?.GetVisualDescendants()?.OfType<DropPanelWithCompass>().FirstOrDefault();

        public DockKind? CurrentGroupDock =>
            DropPanel?.DockSide;

        public DockManager? TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value!);
        }
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


        #region ShowCompass Styled Avalonia Property
        public bool ShowCompass
        {
            get { return GetValue(ShowCompassProperty); }
            set { SetValue(ShowCompassProperty, value); }
        }

        public static readonly StyledProperty<bool> ShowCompassProperty =
            AvaloniaProperty.Register<DockItem, bool>
            (
                nameof(ShowCompass)
            );
        #endregion ShowCompass Styled Avalonia Property

        public void CleanSelfOnRemove()
        {
            if (Header is IControl headerControl)
            {
                headerControl.DisconnectVisualParentContentPresenter();
            }

            if (Content is IControl contentControl)
            {
                contentControl.DisconnectVisualParentContentPresenter();
            }

            this.TheDockManager = null;

            this.DropPanel?.FinishPointerDetection();

            if (this.TheVisual != null)
            {
                TheVisual.DockContext = null;
                this.TheVisual = null;
            }

            IsSelected = false;
        }

        public IEnumerable<DockItem> LeafItems => this.ToCollection();
    }
}
