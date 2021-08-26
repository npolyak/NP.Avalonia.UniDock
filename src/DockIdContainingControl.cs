using Avalonia;
using Avalonia.Controls;
using System;

namespace NP.Avalonia.UniDock
{
    public class DockIdContainingControl : Control
    {
        public event Action<IDockGroup>? DockIdChanged;

        #region DockId Styled Avalonia Property
        public string DockId
        {
            get { return GetValue(DockIdProperty); }
            set { SetValue(DockIdProperty, value); }
        }

        public static readonly StyledProperty<string> DockIdProperty =
            AvaloniaProperty.Register<DockTabbedGroup, string>
            (
                nameof(DockId)
            );
        #endregion Id Styled Avalonia Property

        private void FireDockIdChanged()
        {
            DockIdChanged?.Invoke((IDockGroup) this);
        }

        protected void OnDockIdChanged(AvaloniaPropertyChangedEventArgs e)
        {
            FireDockIdChanged();
        }

        public override string ToString() => DockId;
    }
}
