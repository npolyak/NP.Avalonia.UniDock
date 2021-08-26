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
using System;
using Avalonia.Media;

namespace NP.Avalonia.UniDock
{
    public class DropPanelWithCompass : TemplatedControl
    {
        #region DockSide Styled Avalonia Property
        public DockKind? DockSide
        {
            get { return GetValue(DockSideProperty); }
            set { SetValue(DockSideProperty, value); }
        }

        public static readonly StyledProperty<DockKind?> DockSideProperty =
            AvaloniaProperty.Register<DropPanelWithCompass, DockKind?>
            (
                nameof(DockSide)
            );
        #endregion DockSide Styled Avalonia Property

        public bool CanStartPointerDetection
        {
            get => TheCompass?.CanStartPointerDetection ?? false;
            set
            {
                if (TheCompass != null)
                {
                    TheCompass.CanStartPointerDetection = value;
                }
            }
        }


        private DockCompass? TheCompass => 
            this.GetVisualDescendants()
                .OfType<DockCompass>()
                .FirstOrDefault();


        public void FinishPointerDetection()
        {
            TheCompass?.FinishPointerDetection();
        }

        #region SelectBackground Styled Avalonia Property
        public IBrush SelectBackground
        {
            get { return GetValue(SelectBackgroundProperty); }
            set { SetValue(SelectBackgroundProperty, value); }
        }

        public static readonly StyledProperty<IBrush> SelectBackgroundProperty =
            AvaloniaProperty.Register<DropPanelWithCompass, IBrush>
            (
                nameof(SelectBackground)
            );
        #endregion SelectBackground Styled Avalonia Property
    }
}
