using NP.Utilities;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public static class DockGroupTreeUtils
    {
        public static IDockGroup? ToParent(IDockGroup g) => g?.DockParent;

        public static IEnumerable<IDockGroup>? ToChildren(IDockGroup g) =>
            g.DockChildren;

        public static IEnumerable<IDockGroup> DockGroupAncestors(this IDockGroup g)
        {
            return g.Ancestors(ToParent!);
        }

        public static IEnumerable<IDockGroup> DockGroupSelfAndAncestors(this IDockGroup g)
        {
            return g.SelfAndAncestors(ToParent!);
        }

        public static IDockGroup GetDockGroupRootNote(this IDockGroup g)
        {
            return g.GetRootNode(ToParent!);
        }

        public static IEnumerable<IDockGroup> DockGroupSelfAndDescendants(this IDockGroup g) =>
            g.SelfAndDescendants(ToChildren!);

        public static IEnumerable<IDockGroup> DockGroupDescendants(this IDockGroup g) =>
           g.Descendants(ToChildren!);
    }
}
