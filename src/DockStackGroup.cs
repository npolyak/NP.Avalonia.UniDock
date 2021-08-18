using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class DockStackGroup : StackGroup<IDockGroup>, IDockGroupDockManagerContainer, IDisposable
    {
        DockManagerContainer IDockGroupDockManagerContainer.TheDockManagerContainer { get; } = 
            new DockManagerContainer();

        public IDockGroup? DockParent { get; set; }

        public IList<IDockGroup>? DockChildren => Items;

        SetDockManagerBehavior? _behavior;
        public DockStackGroup()
        {
            _behavior = new SetDockManagerBehavior(this);
        }

        public void Dispose()
        {
            _behavior?.Dispose();
            _behavior = null;
        }
    }
}
