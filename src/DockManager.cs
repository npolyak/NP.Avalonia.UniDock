using Avalonia.Controls;
using NP.Avalonia.Visuals.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System;
using Avalonia;
using NP.Avalonia.Visuals;
using NP.Utilities;
using Avalonia.VisualTree;

namespace NP.AvaloniaDock
{
    public class DockManager
    {
        public IList<DockWindow> DockWindows { get; } =
            new List<DockWindow>();

        public IList<DockTabbedGroup> TabbedGroups { get; } =
            new List<DockTabbedGroup>();

        DockWindow? _draggedWindow;
        public DockWindow? DraggedWindow
        {
            get => _draggedWindow;
            internal set
            {
                if (ReferenceEquals(_draggedWindow, value))
                    return;

                _draggedWindow = value;

                if (_draggedWindow != null)
                {
                    BeginDragAction();
                }
            }
        }

        private IList<(DockTabbedGroup Group, Rect2D Rect)>? _currentTabbedGroups;

        IDisposable? _pointerMovedSubscription;
        private void BeginDragAction()
        {
            if (_draggedWindow == null)
                return;

            _currentTabbedGroups = 
                TabbedGroups
                .Except(_draggedWindow.Groups)
                .Select(g => (g, g.GetScreenBounds())).ToList();

            _pointerMovedSubscription = CurrentScreenPointBehavior.CurrentScreenPoint.Subscribe(OnPointerMoved);
        }

        DockTabbedGroup? _currentGroup = null;

        private void OnPointerMoved(PixelPoint pointerScreenLocation)
        {
            if (_currentTabbedGroups == null)
                return;

            Point2D ponterScreenLocation2D = pointerScreenLocation.ToPoint2D();

            var pointerAboveGroups =
                _currentTabbedGroups.Where(gr => gr.Rect.ContainsPoint(ponterScreenLocation2D)).Select(gr => gr.Group);

            var pointerAboveWindows =
                pointerAboveGroups.Select(g => g.GetVisualAncestors().OfType<Window>().FirstOrDefault()).ToList();

            if (_currentGroup != null)
            {
                _currentGroup.ShowCompass = false;
            }

            _currentGroup = pointerAboveGroups.FirstOrDefault();

            if (_currentGroup == null)
                return;

            _currentGroup.ShowCompass = true;

            Window w = _currentGroup.GetVisualAncestors().OfType<Window>().First();

            w.Activate();
        }

        public void CompleteDragDropAction()
        {
            _pointerMovedSubscription?.Dispose();
            _pointerMovedSubscription = null;

            if (_currentGroup != null)
            {
                _currentGroup.ShowCompass = false;
                _currentGroup = null;
            }

            DraggedWindow = null;
        }
    }
}
