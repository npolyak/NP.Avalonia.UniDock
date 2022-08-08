using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace NP.Avalonia.UniDock
{
    public class DockSeparator : GridSplitter, IControlWithOrientation
    {
        #region TheOrientation Styled Avalonia Property
        public Orientation TheOrientation
        {
            get { return GetValue(TheOrientationProperty); }
            set { SetValue(TheOrientationProperty, value); }
        }

        public static readonly StyledProperty<Orientation> TheOrientationProperty =
            AvaloniaProperty.Register<DockSeparator, Orientation>
            (
                nameof(TheOrientation)
            );
        #endregion TheOrientation Styled Avalonia Property
    }
}
