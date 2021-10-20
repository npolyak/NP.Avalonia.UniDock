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
using Avalonia.Controls;
using NP.Avalonia.UniDockService;
using NP.Avalonia.Visuals.Behaviors;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public interface IDockGroup : IDockManagerContainer, IControl, IRemovable
    {
        public string DockId { get; set; }

        event Action<IDockGroup> DockIdChanged;

        event Action<IDockGroup> IsDockVisibleChangedEvent;

        internal void FireIsDockVisibleChangedEvent();

        IDockGroup? DockParent { get; set; }

        bool IsDockVisible
        {
            get => DockAttachedProperties.GetIsDockVisible(this);
            set => DockAttachedProperties.SetIsDockVisible(this, value);
        }

        public GroupKind TheGroupKind { get; }

        IList<IDockGroup> DockChildren { get; }

        bool ShowChildHeader => true;

        bool AutoDestroy { get; set; }

        bool CanFloat
        {
            get => true;
            set
            {

            }
        }

        bool CanClose
        {
            get => true;
            set
            {

            }
        }

        /// stable groups become invisible when they do not have any items.
        /// They are not removed, when empty or have only one item. 
        /// They are used to set the default locations of predefined DockItems
        bool IsStableGroup
        {
            get => false;
            set
            {

            }
        }

        // IsPredefined == true can only be for DockItems
        // so that
        //      They can be restored from dock id (without any extra data) - for DockItems
        // IsPredefined == false means
        //      If dock item - it does not have a default location and needs a full parameter list with values to
        //          be restored - or rather - recreated
        bool IsPredefined 
        {
            get => false;
            set { }
        }

        bool IsRoot => DockParent == null;

        double DefaultDockOrderInGroup
        {
            get => 0;
            set { }
        }

        void CleanSelfOnRemove()
        {
            //this.TheDockManager = null;
        }

        protected void SimplifySelf();

        void Simplify() 
        {
            IDockGroup? dockParent = DockParent;
            SimplifySelf();
            dockParent?.Simplify();
        }

        IDockGroup CloneIfStable();
    }

    public interface ILeafDockObj : IDockGroup
    {
        bool ShowCompass { get; set; }

        DockKind? CurrentGroupDock { get; }

        IEnumerable<DockItem> LeafItems { get; }

        IDockGroup? GetContainingGroup() => DockParent;

        IControl GetVisual() => this;
    }

    public static class DockGroupHelper
    {
        public static void RemoveItselfFromParent(this IDockGroup item)
        {
            if (!item.IsStableGroup)
            {
                IDockGroup? parent = item.DockParent;

                if (parent != null)
                {
                    parent.DockChildren!.Remove(item);
                    item.DockParent = null;
                }

                item.CleanSelfOnRemove();
            }
        }

        public static int GetNumberChildren(this IDockGroup item)
        {
            return item?.DockChildren?.Count ?? 0;
        }

        public static bool HasLeafAncestor(this ILeafDockObj item)
        {
            return 
                item.GetDockGroupAncestors()
                    .Any(ancestor => ancestor is ILeafDockObj);
        }

        public static GridLength GetSizeCoeff(this IDockGroup group, int idx)
        {
            if (group is StackDockGroup dockStackGroup)
            {
                return dockStackGroup.GetSizeCoefficient(idx);
            }

            return default;
        }

        public static void SetSizeCoeff(this IDockGroup group, int idx, GridLength coeff)
        {
            if (group is StackDockGroup dockStackGroup)
            {
                dockStackGroup.SetSizeCoefficient(idx, coeff);
            }
        }

        public static void SimplifySelfImpl(this IDockGroup group)
        {
            if (!group.AutoDestroy || group.IsStableGroup)
            {
                return;
            }

            if (group.GetNumberChildren() == 0)
            {
                group.RemoveItselfFromParent();
                group.TheDockManager = null;
            }

            IDockGroup? dockParent = group.DockParent;
            if (dockParent == null)
            {
                return;
            }

            if (group.GetNumberChildren() == 1)
            {
                FloatingWindow? window = group.GetGroupWindow();

                try
                {
                    window?.SetCannotClose();
                    int idx = dockParent.DockChildren.IndexOf(group);

                    GridLength sizeCoeff = dockParent.GetSizeCoeff(idx);

                    group.RemoveItselfFromParent();

                    IDockGroup child = group.DockChildren.First();
                    child.RemoveItselfFromParent();

                    group.TheDockManager = null;

                    dockParent.DockChildren.Insert(idx, child);

                    dockParent.SetSizeCoeff(idx, sizeCoeff);
                }
                finally
                {
                    window?.ResetCanClose();
                }
            }
        }

        public static void SetIsDockVisible(this IDockGroup group)
        {
            bool isDockVisible = group.DockChildren.Any(child => child.IsDockVisible);

            group.IsDockVisible = isDockVisible;
        }

        private static IDisposable? _isDockVisibleChangeSubscription = null;
        internal static void SetIsDockVisibleChangeSubscription()
        {
            if (_isDockVisibleChangeSubscription == null)
            {
                _isDockVisibleChangeSubscription =
                    DockAttachedProperties
                        .IsDockVisibleProperty
                        .Changed
                        .Subscribe(OnIsDockVisbleChanged);
            }
        }

        public static IEnumerable<ILeafDockObj> GetLeafGroups(this IDockGroup dockGroup)
        {
            var leafGroups = 
                dockGroup.GetDockGroupSelfAndDescendants(stopCondition: item => item is ILeafDockObj)
                         .OfType<ILeafDockObj>()
                         .Distinct();

            return leafGroups;
        }

        public static IEnumerable<DockItem> GetLeafItems(this IDockGroup dockGroup)
        {
            return dockGroup.GetLeafGroups().SelectMany(g => g.LeafItems);
        }

        private static void OnIsDockVisbleChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            IDockGroup? dockGroup = args.Sender as IDockGroup;

            dockGroup?.FireIsDockVisibleChangedEvent();
        }

        public static FloatingWindow? GetGroupWindow(this IDockGroup group)
        {
            return group.GetDockGroupRoot()?.GetControlsWindow<FloatingWindow>();
        }

        public static DockObjectInfo ToDockObjectInfo(this IDockGroup group)
        {
            return new DockObjectInfo(group.DockId, group.TheGroupKind);
        }

        public static void SetStableParent(this IDockGroup group)
        {
            if (group.IsStableGroup)
            {
                if (group.DockParent != null)
                {
                    group.DockParent.IsStableGroup = true;
                }
            }
        }

        public static T? GetResource<T>(this IDockGroup? dockGroup, object resourceKey)
            where T : class
        {
            if (dockGroup == null)
                return null;

            T? result = dockGroup.FindResource(resourceKey) as T;

            if (result != null)
            {
                return result;
            }

            SimpleDockGroup? rootGroup = dockGroup.GetDockGroupRoot() as SimpleDockGroup;

            if (dockGroup != rootGroup)
            {
                result = rootGroup?.FindResource(resourceKey) as T;

                if (result != null)
                {
                    return result;
                }
            }

            SimpleDockGroup? parentDockGroup = rootGroup?.ParentWindowGroup;

            return parentDockGroup?.GetResource<T>(resourceKey);
        }
    }
}
