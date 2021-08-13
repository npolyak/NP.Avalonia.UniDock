using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class DockCompass : TemplatedControl
    {
        private IList<DockSideControl>? SideControls { get; set; }

        #region DockSide Styled Avalonia Property
        public GroupDock? DockSide
        {
            get { return GetValue(DockSideProperty); }
            private set { SetValue(DockSideProperty, value); }
        }

        public static readonly StyledProperty<GroupDock?> DockSideProperty =
            AvaloniaProperty.Register<DockCompass, GroupDock?>
            (
                nameof(DockSide)
            );
        #endregion DockSide Styled Avalonia Property

        private IDisposable? _subscriptionDisposable = null;
        public void StartPointerDetection()
        {
            SideControls = 
                this.GetVisualDescendants()
                    .OfType<DockSideControl>()
                    .ToList();

            _subscriptionDisposable =
                CurrentScreenPointBehavior.CurrentScreenPoint.Subscribe(OnPointerMoved);
        }

        private void OnPointerMoved(Point2D pointerScreenLocation)
        {
            var currentSideControl =
                SideControls
                    ?.FirstOrDefault
                    (
                        c => PointHelper.GetScreenBounds(c).ContainsPoint(pointerScreenLocation));

            DockSide = currentSideControl?.DockSide;
        }

        public void FinishPointerDetection()
        {
            DockSide = null;
        }
    }
}
