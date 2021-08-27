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

        public static double GetSizeCoeff(this IDockGroup group, int idx)
        {
            if (group is DockStackGroup dockStackGroup)
            {
                return dockStackGroup.GetSizeCoefficient(idx);
            }

            return -1;
        }

        public static void SetSizeCoeff(this IDockGroup group, int idx, double coeff)
        {
            if (coeff < 0)
            {
                return;
            }

            if (group is DockStackGroup dockStackGroup)
            {
                dockStackGroup.SetSizeCoefficient(idx, coeff);
            }
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

                double sizeCoeff = dockParent.GetSizeCoeff(idx);

                group.RemoveItselfFromParent();

                IDockGroup child = group.DockChildren.First();
                child.RemoveItselfFromParent();

                dockParent.DockChildren.Insert(idx, child);

                dockParent.SetSizeCoeff(idx, sizeCoeff);
            }
        }
    }
}
