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
using Avalonia.Media;

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
                .Select(g => (g, g.GetVisual().GetScreenBounds())).ToList();

            _pointerMovedSubscription = 
                CurrentScreenPointBehavior.CurrentScreenPoint
                                          .Subscribe(OnPointerMoved);
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
                        g => g.GetVisual().GetVisualAncestors()
                              .OfType<Window>()
                              .FirstOrDefault()).ToList();

            CurrentLeafObjToInsertWithRespectTo = pointerAboveGroups.FirstOrDefault();

            if (CurrentLeafObjToInsertWithRespectTo == null)
            {
                return;
            }

            Window w = CurrentLeafObjToInsertWithRespectTo.GetVisual().GetVisualAncestors().OfType<Window>().First();

            w.Activate();
        }

        private void DropWithOrientation(DockKind? dock, IDockGroup draggedGroup)
        {
            if (dock == null || dock == DockKind.Tabs)
            {
                throw new Exception("Programming ERROR: dock should be one of Left, Top, Right, Bottom");
            }

            draggedGroup.RemoveItselfFromParent();
            IDockGroup parentGroup = CurrentLeafObjToInsertWithRespectTo.DockParent!;

            Orientation orientation = (Orientation) dock.ToOrientation();

            int childIdx =
                parentGroup
                    .DockChildren.IndexOf(CurrentLeafObjToInsertWithRespectTo);

            if (parentGroup is DockStackGroup dockStackGroup && dockStackGroup.TheOrientation == orientation)
            {
                int insertIdx = childIdx.ToInsertIdx(dock);
                parentGroup.DockChildren.Insert(insertIdx, draggedGroup);
            }
            else
            {
                CurrentLeafObjToInsertWithRespectTo.RemoveItselfFromParent();
                DockStackGroup insertGroup = new DockStackGroup { TheOrientation = orientation };
                parentGroup.DockChildren.Insert(childIdx, insertGroup);

                insertGroup.DockChildren?.Insert(0, CurrentLeafObjToInsertWithRespectTo);

                int insertIdx = 0.ToInsertIdx(dock);
                insertGroup.DockChildren?.Insert(insertIdx, draggedGroup);
            }

            DraggedWindow?.Close();
        }

        public void CompleteDragDropAction()
        {
            try
            {
                _pointerMovedSubscription?.Dispose();
                _pointerMovedSubscription = null;

                IDockGroup? draggedGroup = DraggedWindow?.TheDockGroup?.TheChild;

                DockKind? currentDock = CurrentLeafObjToInsertWithRespectTo?.CurrentGroupDock;
                if (draggedGroup != null)
                {
                    switch (currentDock)
                    {
                        case DockKind.Tabs:
                        {
                            var leafItems = DraggedWindow?.LeafItems.ToList();

                            if (leafItems.IsNullOrEmptyCollection())
                            {
                                return;
                            }

                            leafItems.DoForEach(item => item.RemoveItselfFromParent());
                            IDockGroup currentGroup =
                                CurrentLeafObjToInsertWithRespectTo?.GetContainingGroup()!;

                            var groupToInsertItemsInto = currentGroup as DockTabbedGroup;

                            if (groupToInsertItemsInto == null)
                            {
                                groupToInsertItemsInto = new DockTabbedGroup();

                                int currentLeafObjIdx = currentGroup.DockChildren.IndexOf(CurrentLeafObjToInsertWithRespectTo!);
                                currentGroup.DockChildren?.Remove(CurrentLeafObjToInsertWithRespectTo!);

                                CurrentLeafObjToInsertWithRespectTo?.CleanSelfOnRemove();

                                var additionaLeafItems = CurrentLeafObjToInsertWithRespectTo?.LeafItems;

                                additionaLeafItems?.DoForEach(item => item.RemoveItselfFromParent());

                                if (additionaLeafItems != null)
                                {
                                    leafItems = leafItems.Union(additionaLeafItems).ToList();
                                }
                                
                                currentGroup.DockChildren?.Insert(currentLeafObjIdx, groupToInsertItemsInto);

                                groupToInsertItemsInto.ApplyTemplate();
                            }

                            groupToInsertItemsInto.DockChildren.InsertCollectionAtStart(leafItems);
                            var firstLeafItem = leafItems.FirstOrDefault();

                            DraggedWindow?.Close();

                            firstLeafItem?.Select();
                         break;
                        }
                        case DockKind.Left:
                        case DockKind.Top:
                        case DockKind.Right:
                        case DockKind.Bottom:
                        {
                            DropWithOrientation(currentDock, draggedGroup);

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
