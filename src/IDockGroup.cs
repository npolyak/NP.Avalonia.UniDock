using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public interface IDockGroup : IControl, IRemovable
    {
        DockManager? TheDockManager { get; }

        IDockGroup? DockParent { get; set; }

        IList<IDockGroup>? DockChildren { get; }

        bool ShowChildHeader => true;

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
    }
}
