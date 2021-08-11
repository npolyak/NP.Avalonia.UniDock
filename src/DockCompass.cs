using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace NP.AvaloniaDock
{
    public class DockCompass : TemplatedControl
    {
        #region DockSide Styled Avalonia Property
        public Dock? DockSide
        {
            get { return GetValue(DockSideProperty); }
            set { SetValue(DockSideProperty, value); }
        }

        public static readonly StyledProperty<Dock?> DockSideProperty =
            AvaloniaProperty.Register<DockCompass, Dock?>
            (
                nameof(DockSide)
            );
        #endregion DockSide Styled Avalonia Property
    }
}
