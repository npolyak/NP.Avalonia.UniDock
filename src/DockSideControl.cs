using Avalonia;
using Avalonia.Controls.Primitives;

namespace NP.AvaloniaDock
{
    public class DockSideControl : TemplatedControl
    {
        #region DockSide Styled Avalonia Property
        public GroupDock? DockSide
        {
            get { return GetValue(DockSideProperty); }
            set { SetValue(DockSideProperty, value); }
        }

        public static readonly StyledProperty<GroupDock?> DockSideProperty =
            AvaloniaProperty.Register<DockSideControl, GroupDock?>
            (
                nameof(DockSide),
                null
            );
        #endregion DockSide Styled Avalonia Property
    }
}
