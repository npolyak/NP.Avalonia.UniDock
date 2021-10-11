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

using Avalonia.Controls;
using NP.Avalonia.Visuals.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System;
using NP.Avalonia.Visuals;
using NP.Utilities;
using Avalonia.VisualTree;
using Avalonia.Layout;
using System.Collections.ObjectModel;
using NP.Concepts.Behaviors;
using System.ComponentModel;
using System.IO;
using NP.Avalonia.UniDock.Serialization;
using NP.Avalonia.UniDock.Factories;
using NP.Utilities.BasicInterfaces;
using NP.Utilities.Attributes;

namespace NP.Avalonia.UniDock
{
    public class DockManager
    {
        // To be used in the future when multiple DockManagers become available
        public string? Id { get; set; }

        [Part]
        private IStackGroupFactory StackGroupFactory { get; set; } = 
            new StackGroupFactory();

        [Part]
        private ITabbedGroupFactory TabbedGroupFactory { get; set; } = 
            new TabbedGroupFactory();

        [Part]
        internal IFloatingWindowFactory FloatingWindowFactory { get; set; } =
            new FloatingWindowFactory();

        [Part]
        internal IDockVisualItemGenerator? TheDockVisualItemGenerator { get; set; } =
            new DockVisualItemGenerator();

        [Part]
        internal IDockSeparatorFactory? TheDockSeparatorFactory { get; set; } =
            new DockSeparatorFactory();

        IObjectComposer? _dockObjectComposer = null;
        public IObjectComposer? TheDockObjectComposer
        {
            get => _dockObjectComposer;
            set
            {
                if (_dockObjectComposer == value)
                    return;

                _dockObjectComposer = value;

                _dockObjectComposer?.ComposeObject(this, true);
            }
        }

        IList<Window> _windows = new ObservableCollection<Window>();
        public IEnumerable<Window> Windows => _windows;
        internal void AddWindow(Window window) => _windows.Add(window);
        internal void RemoveWindow(Window window) => _windows.Remove(window);
        

        private IList<IDockGroup> _connectedGroups = new ObservableCollection<IDockGroup>();
        public IEnumerable<IDockGroup> ConnectedGroups => _connectedGroups;
        internal void AddConnectedGroup(IDockGroup group)
        {
            if (group.IsPredefined)
            {
                _disconnectedGroups.Remove(group);
            }

            _connectedGroups.Add(group);
        }
            
        internal void RemoveConnectedGroup(IDockGroup group)
        {
            _connectedGroups.Remove(group);

            if (group.IsPredefined)
            {
                _disconnectedGroups.Add(group);
            }
        }

        private IList<IDockGroup> _disconnectedGroups = new List<IDockGroup>();

        public IEnumerable<IDockGroup> DisconnectedGroups => _disconnectedGroups;

        public IEnumerable<IDockGroup> AllGroups => _connectedGroups.Union(_disconnectedGroups);

        public IList<ILeafDockObj> DockLeafObjs { get; } =
            new List<ILeafDockObj>();

        public IEnumerable<ILeafDockObj> DockLeafObjsWithoutLeafParents =>
            DockLeafObjs.Where(leaf => !leaf.HasLeafAncestor()).ToList();

        public void ClearGroups()
        {
            foreach(IDockGroup group in _connectedGroups.ToList())
            {
                if (!group.IsRoot)
                {
                    group.RemoveItselfFromParent();
                }
            }
        }

        FloatingWindow? _draggedWindow;
        public FloatingWindow? DraggedWindow
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
                DockLeafObjsWithoutLeafParents
                .Where(leaf => leaf.GetVisual().IsAttachedToLogicalTree)
                .Except(_draggedWindow.LeafItems)
                .Select(g => (g, g.GetVisual().GetScreenBounds())).ToList();

            _pointerMovedSubscription = 
                CurrentScreenPointBehavior.CurrentScreenPoint
                                          .Subscribe(OnPointerMoved);
        }

        /// <summary>
        /// group into which we insert dragged item(s)
        /// </summary>
        private ILeafDockObj? _currentLeafObjToInsertWithRespectTo = null;
        private ILeafDockObj? CurrentLeafObjToInsertWithRespectTo
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

            //if (CurrentLeafObjToInsertWithRespectTo == null)
            //{
            //    return;
            //}

            //Window w = CurrentLeafObjToInsertWithRespectTo.GetVisual().GetVisualAncestors().OfType<Window>().First();

            //w.Activate();

            //_draggedWindow?.Activate();
        }

        private void DropWithOrientation(DockKind? dock, IDockGroup originalDraggedGroup)
        {
            if (dock == null || dock == DockKind.Tabs)
            {
                throw new Exception("Programming ERROR: dock should be one of Left, Top, Right, Bottom");
            }

            var draggedGroup = originalDraggedGroup.CloneIfStable();

            IDockGroup parentGroup = CurrentLeafObjToInsertWithRespectTo!.DockParent!;

            Orientation orientation = (Orientation) dock.ToOrientation()!;

            int childIdx =
                parentGroup
                    .DockChildren.IndexOf(CurrentLeafObjToInsertWithRespectTo);

            if (parentGroup is StackDockGroup dockStackGroup && dockStackGroup.TheOrientation == orientation)
            {
                int insertIdx = childIdx.ToInsertIdx(dock);
                parentGroup.DockChildren.Insert(insertIdx, draggedGroup);
            }
            else // create an extra DockStackGroup insert the dragged object and the 
                 // the object it is dropped on (drop object) into that group and insert
                 // this new group in place of the drop object.
            {
                double sizeCoeff = parentGroup.GetSizeCoeff(childIdx);

                CurrentLeafObjToInsertWithRespectTo.RemoveItselfFromParent();
                StackDockGroup insertGroup = StackGroupFactory.Create();

                insertGroup.TheOrientation = orientation;
                
                parentGroup.DockChildren.Insert(childIdx, insertGroup);

                parentGroup.SetSizeCoeff(childIdx, sizeCoeff);

                insertGroup.DockChildren?.Insert(0, CurrentLeafObjToInsertWithRespectTo);

                int insertIdx = 0.ToInsertIdx(dock);
                insertGroup.DockChildren?.Insert(insertIdx, draggedGroup);
            }

            (draggedGroup.GetLeafItems().FirstOrDefault() as IActiveItem<DockItem>)?.MakeActive();

            DraggedWindow?.Close();
        }

        private readonly IDisposable _groupsBehavior;
        private readonly IDisposable _windowsBehavior;
        public DockManager()
        {
            DockGroupHelper.SetIsDockVisibleChangeSubscription();

            _groupsBehavior = 
                ConnectedGroups.AddBehavior(OnGroupItemAdded, OnGroupItemRemoved);

            _windowsBehavior = 
                Windows.AddBehavior(OnWindowItemAdded, OnWindowItemRemoved);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Window window = (Window) sender;

            IEnumerable<IDockGroup> dockGroups;

            if (window is FloatingWindow dockWindow)
            {
                dockGroups = dockWindow.TheDockGroup.ToCollection();
            }
            else
            {
                dockGroups =
                    window.GetVisualDescendants()
                      .OfType<IDockGroup>()
                      .Where(group => group.IsRoot).ToList();
            }

            dockGroups.DoForEach(group => group.TheDockManager = null);
        }

        private void OnWindowItemAdded(Window window)
        {
            window.Closing += Window_Closing!;

            string? windowId = DockAttachedProperties.GetWindowId(window);

            if (windowId == null)
            {
                string prefix = window.GetType().Name;
                windowId =
                    _windowIdGenerator.GetUniqueName
                    (
                        Windows.Except(window.ToCollection()).Select(w => DockAttachedProperties.GetWindowId(w)), prefix);

                DockAttachedProperties.SetWindowId(window, windowId);
            }
        }

        private void OnWindowItemRemoved(Window window)
        {
            window.Closing -= Window_Closing!;
        }

        private void VerifyDockIdUnique(IDockGroup group)
        {
            if (AllGroups.Count(g => g.DockId == group.DockId) > 1)
            {
                throw new Exception($"Programming Error - two or more groups cannot have the same Id {group.DockId}. Id should be unique within the DockManager.");
            }
        }

        private void AddedGroup_DockIdChanged(IDockGroup group)
        {
            VerifyDockIdUnique(group);
        }

        UniqueNameGeneratorWithMaxMemory _dockIdGenerator = new UniqueNameGeneratorWithMaxMemory();
        UniqueNameGeneratorWithMaxMemory _windowIdGenerator = new UniqueNameGeneratorWithMaxMemory();
        private void OnGroupItemAdded(IDockGroup addedGroup)
        {
            if (addedGroup.DockId == null)
            {
                string prefix = addedGroup.GetType().Name;

                addedGroup.DockId = _dockIdGenerator.GetUniqueName(AllGroups.Except(addedGroup.ToCollection()).Select(group => group.DockId), prefix);
            }
            else
            {
                VerifyDockIdUnique(addedGroup);
            }

            addedGroup.DockIdChanged += AddedGroup_DockIdChanged;
        }

        private void OnGroupItemRemoved(IDockGroup removedGroup)
        {
            removedGroup.DockIdChanged -= AddedGroup_DockIdChanged;
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

                            var groupToInsertItemsInto = currentGroup as TabbedDockGroup;

                            if (groupToInsertItemsInto == null)
                            {
                                groupToInsertItemsInto = TabbedGroupFactory.Create();

                                int currentLeafObjIdx = currentGroup.DockChildren.IndexOf(CurrentLeafObjToInsertWithRespectTo!);
                                double sizeCoeff = currentGroup.GetSizeCoeff(currentLeafObjIdx);

                                currentGroup.DockChildren?.Remove(CurrentLeafObjToInsertWithRespectTo!);

                                currentGroup.DockChildren?.Insert(currentLeafObjIdx, groupToInsertItemsInto);
                                currentGroup.SetSizeCoeff(currentLeafObjIdx, sizeCoeff);

                                CurrentLeafObjToInsertWithRespectTo?.CleanSelfOnRemove();

                                var additionaLeafItems = CurrentLeafObjToInsertWithRespectTo?.LeafItems;

                                additionaLeafItems?.DoForEach(item => item.RemoveItselfFromParent());

                                if (additionaLeafItems != null)
                                {
                                    leafItems = leafItems?.Union(additionaLeafItems).ToList();
                                }

                                groupToInsertItemsInto.ApplyTemplate();
                            }

                            groupToInsertItemsInto.DockChildren.InsertCollectionAtStart(leafItems);
                            var firstLeafItem = leafItems?.FirstOrDefault();

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

        public void SaveToFile(string filePath)
        {

            var dockManagerParams = this.ToParams();

            string serializationStr =
                XmlSerializationUtils.Serialize(dockManagerParams);

            using StreamWriter writer = new StreamWriter(filePath);

            writer.Write(serializationStr);

            writer.Flush();
        }

        public void RestoreFromFile(string filePath)
        {
            using StreamReader reader = new StreamReader(filePath);

            string serializationStr = reader.ReadToEnd();

            DockManagerParams dmp =
                XmlSerializationUtils.Deserialize<DockManagerParams>(serializationStr);

            this.SetDockManagerFromParams(dmp);
        }

        internal IDockGroup? FindGroupById(string? dockId)
        {
            var result = this.AllGroups.FirstOrDefault(g => g.DockId == dockId);

            return result;
        }
    }
}
