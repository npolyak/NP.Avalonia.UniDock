using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public class DockItemPresenter : TemplatedControl
    {
        static DockItemPresenter()
        {
            DockContextProperty.Changed.AddClassHandler<DockItemPresenter>((x,e) => x.OnDockContextChanged(e));
        }

        #region DockContext Styled Avalonia Property
        public DockItem? DockContext
        {
            get { return GetValue(DockContextProperty); }
            set { SetValue(DockContextProperty, value); }
        }

        public static readonly StyledProperty<DockItem?> DockContextProperty =
            AvaloniaProperty.Register<DockItemPresenter, DockItem?>
            (
                nameof(DockContext)
            );
        #endregion DockContext Styled Avalonia Property

        private void OnDockContextChanged(AvaloniaPropertyChangedEventArgs e)
        {
            DockItem? oldDockItem = (DockItem?)e.OldValue;

            if (oldDockItem != null)
            {
                oldDockItem.TheVisual = null;
            }

            DockItem? newDockItem = (DockItem?)e.NewValue;

            if (newDockItem != null)
            {
                newDockItem.TheVisual = this;
            }
        }
    }
}
