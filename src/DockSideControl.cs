using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace NP.AvaloniaDock
{
    public class DockSideControl : TemplatedControl
    {
        #region DockSide Styled Avalonia Property
        public Dock? DockSide
        {
            get { return GetValue(DockSideProperty); }
            set { SetValue(DockSideProperty, value); }
        }

        public static readonly StyledProperty<Dock?> DockSideProperty =
            AvaloniaProperty.Register<DockSideControl, Dock?>
            (
                nameof(DockSide),
                null
            );
        #endregion DockSide Styled Avalonia Property
    }
}
