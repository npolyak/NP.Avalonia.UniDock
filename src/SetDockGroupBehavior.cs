using System;
using System.Collections.Generic;
using NP.Concepts.Behaviors;

namespace NP.AvaloniaDock
{
    public class SetDockGroupBehavior<T> : IDisposable
        where T : IDockGroup
    {
        private SetAttachedPropertyFromParentBehavior<T, DockManager>? _setDockManagerBehavior;
        private RemoveItemBehavior<T>? _removeItemBehavior;
        private SetParentBehavior<T>? _setParentBehavior;
        public SetDockGroupBehavior(IDockGroup parent, IList<T> items)
        {
            _setDockManagerBehavior = new SetAttachedPropertyFromParentBehavior<T, DockManager>(parent, items, DockAttachedProperties.TheDockManagerProperty);
            _removeItemBehavior = new RemoveItemBehavior<T>(items);
            _setParentBehavior = new SetParentBehavior<T>(parent, items);
        }

        public void Dispose()
        {
            _setParentBehavior?.Dispose();
            _setParentBehavior = null;
            _removeItemBehavior?.Dispose();
            _removeItemBehavior = null;
            _setDockManagerBehavior?.Dispose();
            _setDockManagerBehavior = null;
        }
    }

    public class SetDockGroupBehavior : SetDockGroupBehavior<IDockGroup>
    {
        public SetDockGroupBehavior(IDockGroup parent, IList<IDockGroup> items) : 
            base(parent, items)
        {

        }
    }
}
