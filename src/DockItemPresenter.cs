// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

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
