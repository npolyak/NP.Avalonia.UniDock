using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class SetParentBehavior<T> : ForEachItemOverrideBehavior<T>
        where T : IDockGroup
    {
        public IDockGroup Parent { get; }

        public SetParentBehavior(IDockGroup parent, IEnumerable<T>? items) : base(items)
        {
            Parent = parent;
        }

        protected override void OnItemAdded(T item)
        {
            item.DockParent = Parent;
        }

        protected override void OnItemRemoved(T item)
        {
            item.DockParent = null;
        }
    }
}
