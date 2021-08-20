using Avalonia.Controls;
using NP.Avalonia.Visuals.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System;
using Avalonia;
using NP.Avalonia.Visuals;
using NP.Utilities;
using Avalonia.VisualTree;
using Avalonia.Layout;

namespace NP.AvaloniaDock
{
    public class DockManager
    {
        public IList<DockWindow> DockWindows { get; } =
            new List<DockWindow>();

        public IList<ILeafDockObj> DockLeafObjs { get; } =
            new List<ILeafDockObj>();

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

        private IList<(ILeafDockObj Group, Rect2D Rect)>? _currentDockGroups;

        IDisposable? _pointerMovedSubscription;
        private void BeginDragAction()
        {
            if (_draggedWindow == null)
                return;

            _currentDockGroups = 
                DockLeafObjs
                .Except(_draggedWindow.LeafItems)
                .Select(g => (g, g.GetScreenBounds())).ToList();

            _pointerMovedSubscription = CurrentScreenPointBehavior.CurrentScreenPoint.Subscribe(OnPointerMoved);
        }

        /// <summary>
        /// group into which we insert dragged item(s)
        /// </summary>
        ILeafDockObj? _currentLeafObjToInsertWithRespectTo = null;
        ILeafDockObj? CurrentLeafObjToInsertWithRespectTo
        {
            get => _currentLeafObjToInsertWithRespectTo;

            set
            {
                if (ReferenceEquals(_currentLeafObjToInsertWithRespectTo, value))
                    return;

                if (_currentLeafObjToInsertWithRespectTo != null)
                {
                    _currentLeafObjToInsertWithRespectTo.ShowCompass = false;
                }

                _currentLeafObjToInsertWithRespectTo = value;

                if (_currentLeafObjToInsertWithRespectTo != null)
                {
                    _currentLeafObjToInsertWithRespectTo.ShowCompass = true;
                }
            }
        }

        private void OnPointerMoved(Point2D pointerScreenLocation)
        {
            if (_currentDockGroups == null)
            {
                return;
            }

            var pointerAboveGroups =
                _currentDockGroups.Where(gr => gr.Rect.ContainsPoint(pointerScreenLocation)).Select(gr => gr.Group);

            var pointerAboveWindows =
                pointerAboveGroups
                    .Select
                    (
                        g => g.GetVisualAncestors()
                              .OfType<Window>()
                              .FirstOrDefault()).ToList();

            CurrentLeafObjToInsertWithRespectTo = pointerAboveGroups.FirstOrDefault();

            if (CurrentLeafObjToInsertWithRespectTo == null)
            {
                return;
            }

            Window w = CurrentLeafObjToInsertWithRespectTo.GetVisualAncestors().OfType<Window>().First();

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
                    switch (CurrentLeafObjToInsertWithRespectTo?.CurrentGroupDock)
                    {
                        case DockKind.Tabs:
                        {
                            var leafItems = DraggedWindow.LeafItems.ToList();

                            leafItems.DoForEach(item => item.RemoveItselfFromParent());

                            IDockGroup groupToInsertInto =
                                CurrentLeafObjToInsertWithRespectTo?.GetContainingGroup()!;

                            groupToInsertInto.DockChildren.InsertCollectionAtStart(leafItems);

                            var firstLeafItem = leafItems.FirstOrDefault();

                            firstLeafItem?.Select();

                            DraggedWindow?.Close();
                            break;
                        }
                        case DockKind.Top:
                        {

                            //CurrentGroupToInsertInto.ClearSelectedItem();

                            IDockGroup parentGroup = CurrentLeafObjToInsertWithRespectTo.DockParent!;

                            if (parentGroup is DockStackGroup dockStackGroup)
                            {
                                if (dockStackGroup.TheOrientation == Orientation.Vertical)
                                {
                                }
                            }    

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
                if (CurrentLeafObjToInsertWithRespectTo != null)
                {
                    CurrentLeafObjToInsertWithRespectTo = null;
                }

                DraggedWindow = null;
            }
        }
    }
}
