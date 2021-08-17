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
        DockTabbedGroup? CurrentGroup
        {
            get => _currentGroup;

            set
            {
                if (ReferenceEquals(_currentGroup, value))
                    return;

                if (_currentGroup != null)
                {
                    _currentGroup.ShowCompass = false;
                }

                _currentGroup = value;

                if (_currentGroup != null)
                {
                    _currentGroup.ShowCompass = true;
                }
            }
        }

        private void OnPointerMoved(Point2D pointerScreenLocation)
        {
            if (_currentTabbedGroups == null)
            {
                return;
            }

            var pointerAboveGroups =
                _currentTabbedGroups.Where(gr => gr.Rect.ContainsPoint(pointerScreenLocation)).Select(gr => gr.Group);

            var pointerAboveWindows =
                pointerAboveGroups
                    .Select
                    (
                        g => g.GetVisualAncestors()
                              .OfType<Window>()
                              .FirstOrDefault()).ToList();

            CurrentGroup = pointerAboveGroups.FirstOrDefault();

            if (CurrentGroup == null)
            {
                return;
            }

            Window w = CurrentGroup.GetVisualAncestors().OfType<Window>().First();

            w.Activate();
        }

        public void CompleteDragDropAction()
        {
            try
            {
                _pointerMovedSubscription?.Dispose();
                _pointerMovedSubscription = null;

                if (DraggedWindow != null)
                {
                    switch (CurrentGroup?.CurrentGroupDock)
                    {
                        case GroupDock.Tabs:
                        {
                            var allItems = DraggedWindow.Groups.SelectMany(g => g.Items).ToList();

                            var groupsToRemove = DraggedWindow.Groups.ToList();

                            DraggedWindow.Groups.DoForEach(g => g.ClearAllItems());

                            CurrentGroup.ClearSelectedItem();

                            CurrentGroup.Items.InsertRange(0, allItems);

                            CurrentGroup.SelectFirst();

                            groupsToRemove.DoForEach(g => g.ClearValue(DockAttachedProperties.TheDockManagerProperty));

                            DraggedWindow?.Close();
                            break;
                        }
                        case GroupDock.Top:
                        {

                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (CurrentGroup != null)
                {
                    CurrentGroup = null;
                }

                DraggedWindow = null;
            }
        }
    }
}
