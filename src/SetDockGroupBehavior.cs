using System;
using System.Collections.Generic;
using NP.Concepts.Behaviors;

namespace NP.AvaloniaDock
{
    public class SetDockGroupBehavior<T> : IDisposable
        where T : IDockGroup
    {
        private RemoveItemBehavior<T>? _removeItemBehavior;
        private SetParentBehavior<T>? _setParentBehavior;
        private SetAttachedPropertyFromParentBehavior<T, DockManager>? _setDockManagerBehavior;
        public SetDockGroupBehavior(IDockGroup parent, IList<T> items)
        {
            _removeItemBehavior = new RemoveItemBehavior<T>(items);
            _setParentBehavior = new SetParentBehavior<T>(parent, items);
            _setDockManagerBehavior = new SetAttachedPropertyFromParentBehavior<T, DockManager>(parent, items, DockAttachedProperties.TheDockManagerProperty);
        }

        public void Dispose()
        {
            _setDockManagerBehavior?.Dispose();
            _setDockManagerBehavior = null;
            _setParentBehavior?.Dispose();
            _setParentBehavior = null;
            _removeItemBehavior?.Dispose();
            _removeItemBehavior = null;
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
