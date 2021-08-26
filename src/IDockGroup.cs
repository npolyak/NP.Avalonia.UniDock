using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.UniDock
{
    public interface IDockGroup : IControl, IRemovable
    {
        public string DockId { get; set; }

        event Action<IDockGroup> DockIdChanged;

        DockManager? TheDockManager { get; set; }

        IDockGroup? DockParent { get; set; }

        IList<IDockGroup> DockChildren { get; }

        bool ShowChildHeader => true;

        bool AutoDestroy { get; set; }

        bool IsRoot => DockParent == null;

        void CleanSelfOnRemove()
        {
            this.TheDockManager = null;
        }

        protected void SimplifySelf();

        void Simplify() 
        {
            IDockGroup? dockParent = DockParent;
            SimplifySelf();
            dockParent?.Simplify();
        }
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
            IDockGroup? parent = item.DockParent;

            if (parent != null)
            {
                parent.DockChildren!.Remove(item);
                item.DockParent = null;
            }

            item.CleanSelfOnRemove();
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

        public static void SimplifySelfImpl(this IDockGroup group)
        {
            if (!group.AutoDestroy)
            {
                return;
            }

            if (group.GetNumberChildren() == 0)
            {
                group.RemoveItselfFromParent();
            }

            IDockGroup? dockParent = group.DockParent;
            if (dockParent == null)
            {
                return;
            }

            if (group.GetNumberChildren() == 1)
            {
                int idx = dockParent.DockChildren.IndexOf(group);
                group.RemoveItselfFromParent();

                IDockGroup child = group.DockChildren.First();
                child.RemoveItselfFromParent();

                dockParent.DockChildren.Insert(idx, child);
            }
        }
    }
}
