using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class DockStackGroup : StackGroup<IDockGroup>, IDockGroup
    {
        public IDockGroup? DockParent { get; set; }

        public IList<IDockGroup>? DockChildren => Items;
    }
}
