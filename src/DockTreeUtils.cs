using NP.Utilities;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public static class DockGroupTreeUtils
    {
        public static IDockGroup? ToParent(IDockGroup g) => g?.DockParent;

        public static IEnumerable<IDockGroup>? ToChildren(IDockGroup g) =>
            g.DockChildren;

        public static IEnumerable<IDockGroup> GetDockGroupAncestors(this IDockGroup g)
        {
            return g.Ancestors(ToParent!);
        }

        public static IEnumerable<IDockGroup> GetDockGroupSelfAndAncestors(this IDockGroup g)
        {
            return g.SelfAndAncestors(ToParent!);
        }

        public static IDockGroup GetDockGroupRootNote(this IDockGroup g)
        {
            return g.GetRootNode(ToParent!);
        }

        public static IEnumerable<IDockGroup> GetDockGroupSelfAndDescendants(this IDockGroup g) =>
            g.SelfAndDescendants(ToChildren!);

        public static IEnumerable<IDockGroup> GetDockGroupDescendants(this IDockGroup g) =>
           g.Descendants(ToChildren!);
    }
}
