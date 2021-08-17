using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;

namespace NP.AvaloniaDock
{
    public class DropDockSideControl : TemplatedControl
    {
        public DockKind SelectDockSide { get; set; }

        private IDisposable? _disposableSubscription = null;
        public DropDockSideControl()
        {
            _disposableSubscription =
                 DockAttachedProperties.DockSideProperty.Changed.Subscribe(OnDockSideChanged);
        }

        private void OnDockSideChanged(AvaloniaPropertyChangedEventArgs<DockKind?> dockSideChange)
        {
            if (dockSideChange.Sender != this)
                return;

            IsSelected = dockSideChange.NewValue.Value == this.SelectDockSide;
        }

        #region IsSelected Direct Avalonia Property
        public static readonly DirectProperty<DropDockSideControl, bool> IsSelectedProperty =
            AvaloniaProperty.RegisterDirect<DropDockSideControl, bool>
            (
                nameof(IsSelected),
                o => o.IsSelected,
                (o, v) => o.IsSelected = v
            );

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            private set
            {
                SetAndRaise(IsSelectedProperty, ref _isSelected, value);
            }
        }
        #endregion IsSelected Direct Avalonia Property
    }
}
