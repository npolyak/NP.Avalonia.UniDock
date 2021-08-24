using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.AvaloniaDock
{
    public interface IDockGroup : IControl, IRemovable
    {
        public string DockId { get; set; }

        event Action<IDockGroup> DockIdChanged;

        DockManager? TheDockManager { get; }

        IDockGroup? DockParent { get; set; }

        IList<IDockGroup> DockChildren { get; }

        bool ShowChildHeader => true;

        bool IsPermanent => false;

        void CleanSelfOnRemove()
        {

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

        public static bool HasLeafAncestor(this ILeafDockObj item)
        {
            return 
                item.GetDockGroupAncestors()
                    .Any(ancestor => ancestor is ILeafDockObj);
        }
    }
}
