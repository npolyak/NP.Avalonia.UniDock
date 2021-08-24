using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using System.Linq;
using System;
using Avalonia.Media;

namespace NP.AvaloniaDock
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

        public DropPanelWithCompass()
        {
            IsVisibleProperty.Changed.Subscribe(OnIsVisibleChanged);
        }

        private void OnIsVisibleChanged
        (
            AvaloniaPropertyChangedEventArgs<bool> isVisibleChange)
        {
            if (isVisibleChange.Sender != this)
                return;

            if (isVisibleChange.NewValue.Value == true)
            {
                if (CanStartPointerDetection)
                {
                    StartPointerDetection();
                }
            }
            else
            {
                FinishPointerDetection();
            }
        }


        #region CanStartPointerDetection Styled Avalonia Property
        public bool CanStartPointerDetection
        {
            get { return GetValue(CanStartPointerDetectionProperty); }
            set { SetValue(CanStartPointerDetectionProperty, value); }
        }

        public static readonly StyledProperty<bool> CanStartPointerDetectionProperty =
            AvaloniaProperty.Register<DropPanelWithCompass, bool>
            (
                nameof(CanStartPointerDetection), 
                true
            );
        #endregion CanStartPointerDetection Styled Avalonia Property


        private DockCompass? TheCompass => 
            this.GetVisualDescendants()
                .OfType<DockCompass>()
                .FirstOrDefault();

        public void StartPointerDetection()
        {
            TheCompass?.StartPointerDetection();
        }

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
