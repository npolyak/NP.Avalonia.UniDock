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

            if (currentSideControl == null)
            {
                this.ClearValue(DockAttachedProperties.DockSideProperty);
                return;
            }

            var currentDockSide = DockAttachedProperties.GetDockSide(this);
            var dockSide = DockAttachedProperties.GetDockSide(currentSideControl);

            if (dockSide != currentDockSide)
            {
                DockAttachedProperties.SetDockSide(this, dockSide);
            }
        }

        public void FinishPointerDetection()
        {
            this.ClearValue(DockAttachedProperties.DockSideProperty);
        }
    }
}
