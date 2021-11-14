using Avalonia;
using Avalonia.Controls.Presenters;

namespace NP.Avalonia.UniDock
{
    public class DockContentPresenter : ContentPresenter, IDockDataContextContainer
    {
        #region DockDataContext Styled Avalonia Property
        public object? DockDataContext
        {
            get { return GetValue(DockDataContextProperty); }
            set { SetValue(DockDataContextProperty, value); }
        }

        public static readonly StyledProperty<object?> DockDataContextProperty =
            AvaloniaProperty.Register<DockContentPresenter, object?>
            (
                nameof(DockDataContext)
            );
        #endregion DockDataContext Styled Avalonia Property
    }
}
