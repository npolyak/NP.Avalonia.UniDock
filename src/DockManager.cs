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
using System.Collections.Specialized;
using Avalonia;
using NP.Avalonia.UniDock.ViewModels;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;

namespace NP.Avalonia.UniDock
{
    public class DockManager : VMBase
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

        private IList<IDockGroup> _disconnectedGroups = new ObservableCollection<IDockGroup>();

        public IEnumerable<IDockGroup> DisconnectedGroups => _disconnectedGroups;

        public IEnumerable<IDockGroup> AllGroups => _connectedGroups.Union(_disconnectedGroups);

        public UnionBehavior<IDockGroup> AllGroupsBehavior { get; }

        public ExpandoObjectBehavior<IDockGroup> DockIdKeysGroups { get; }

        public IList<ILeafDockObj> DockLeafObjs { get; } =
            new List<ILeafDockObj>();

        public IEnumerable<ILeafDockObj> DockLeafObjsWithoutLeafParents =>
            DockLeafObjs.Where(leaf => !leaf.HasLeafAncestor()).ToList();


        private IDisposable? _viewModelsBehavior;
        #region DockItemsViewModels Property
        private IEnumerable<DockItemViewModel>? _dockItemsViewModels;
        public IEnumerable<DockItemViewModel>? DockItemsViewModels
        {
            get
            {
                return this._dockItemsViewModels;
            }
            set
            {
                if (this._dockItemsViewModels == value)
                {
                    return;
                }

                _viewModelsBehavior?.Dispose();

                this._dockItemsViewModels = value;

                _viewModelsBehavior = 
                    _dockItemsViewModels?.AddBehavior
                    (
                        OnGroupViewModelAdded, 
                        OnGroupViewModelRemoved);

                this.OnPropertyChanged(nameof(DockItemsViewModels));
            }
        }

        private DockItem CreateDockItemFromVm(DockItemViewModel vm)
        {
            DockItem? dockItem =
                this.FindGroupById(vm.DockId) as DockItem;

            if (dockItem != null)
            {
                return dockItem;
            }

            dockItem = new DockItem
            {
                DockId = vm.DockId!,
                DefaultDockGroupId = vm.DefaultDockGroupId!,
                DefaultDockOrderInGroup = vm.DefaultDockOrderInGroup,
                CanFloat = vm.CanFloat,
                CanClose = vm.CanClose,
                IsPredefined = vm.IsPredefined,
                Header = vm.HeaderContent,
                Content = vm.Content
            };

            dockItem.DataContext = vm;
            void AddBind<T>
            (
                AvaloniaProperty<T> prop,
                string propName,
                BindingMode bindingMode = BindingMode.TwoWay)
            {
                Binding binding = new Binding(propName);
                binding.Mode = bindingMode;
                dockItem.Bind(prop, binding);
            }

            AddBind(DockAttachedProperties.IsDockVisibleProperty, "IsDockVisible");
            AddBind(DockItem.IsSelectedProperty, "IsSelected");
            AddBind(DockItem.IsActiveProperty, "IsActive");

            dockItem.TheDockManager = this;

            return dockItem;
        }

        private void SetDockItemFromVm(DockItemViewModel vm, IDockGroup dockGroup)
        {
            //if (vm.DockId == "FloatingDockItem1")
            //{
            //    return;
            //}

            if (vm.IsConstructed)
                return;

            DockItem? dockItem =
                this.FindGroupById(vm.DockId) as DockItem;

            if (dockItem == null)
            {
                return;
            }

            if (vm.HeaderContentTemplateResourceKey != null)
            {
                dockItem.HeaderTemplate =
                    (DataTemplate)dockGroup.FindResource(vm.HeaderContentTemplateResourceKey)!;
            }

            if (vm.ContentTemplateResourceKey != null)
            {
                dockItem.ContentTemplate =
                    (DataTemplate)dockGroup.FindResource(vm.ContentTemplateResourceKey)!;
            }

            dockItem.ReattachToDefaultGroup();

            vm.IsConstructed = true;
        }

        private void OnGroupViewModelAdded(DockItemViewModel vm)
        {
            DockItem dockItem = CreateDockItemFromVm(vm);

            string groupId = vm.DefaultDockGroupId!;

            IDockGroup? dockGroup =
                AllGroupsBehavior.Result.FirstOrDefault(g => g.DockId == groupId);

            if (dockGroup != null)
            {
                SetDockItemFromVm(vm, dockGroup);
            }
        }

        private void OnGroupViewModelRemoved(DockItemViewModel dockItemViewModel)
        {
            dockItemViewModel.IsConstructed = false;
        }
        #endregion DockItemsViewModels Property


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
                StackDockGroup insertGroup = StackGroupFactory.Create(null);

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

        private readonly IDisposable? _groupsBehavior;
        private readonly IDisposable? _windowsBehavior;
        public DockManager()
        {
            DockGroupHelper.SetIsDockVisibleChangeSubscription();

            _groupsBehavior = 
                ConnectedGroups.AddBehavior(OnGroupItemAdded, OnGroupItemRemoved);

            _windowsBehavior = 
                Windows.AddBehavior(OnWindowItemAdded, OnWindowItemRemoved);

            AllGroupsBehavior = new UnionBehavior<IDockGroup>(_disconnectedGroups, _connectedGroups);

            DockIdKeysGroups =
                new ExpandoObjectBehavior<IDockGroup>
                (
                    AllGroupsBehavior.Result, 
                    group => group.DockId,
                    group => group.GetObservable(DockIdContainingControl.DockIdProperty));
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

            foreach (var group in dockGroups)
            {
                if (!group.IsStableGroup)
                {
                    group.TheDockManager = null;
                }
            }
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

            IEnumerable<DockItemViewModel>? vms =
                DockItemsViewModels
                    ?.Where(viewModel => viewModel.DefaultDockGroupId == addedGroup.DockId);

            if (vms != null)
            {
                foreach(var vm in vms)
                {
                    if (vm.IsConstructed)
                        continue;

                    SetDockItemFromVm(vm, addedGroup);
                }
            }
        }

        private void OnGroupItemRemoved(IDockGroup removedGroup)
        {
            removedGroup.DockIdChanged -= AddedGroup_DockIdChanged;
        }

        public void CompleteDragDropAction()
        {
            FloatingWindow? currentWindowToDropInto =  CurrentLeafObjToInsertWithRespectTo?.GetGroupWindow(); 

            try
            {
                _pointerMovedSubscription?.Dispose();
                _pointerMovedSubscription = null;

                IDockGroup? draggedGroup = DraggedWindow?.TheDockGroup?.TheChild;

                currentWindowToDropInto?.SetCannotClose();

                DockKind ? currentDock = CurrentLeafObjToInsertWithRespectTo?.CurrentGroupDock;
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
                                groupToInsertItemsInto = TabbedGroupFactory.Create(null);

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

                currentWindowToDropInto?.ResetCanClose();

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
