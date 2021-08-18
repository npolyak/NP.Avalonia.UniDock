using NP.Concepts.Behaviors;
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

        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        RemoveItemBehavior<IDockGroup>? _removeItemBehavior;
        SetDockManagerBehavior? _setDockManagerBehavior;
        public DockStackGroup()
        {
            _setDockManagerBehavior = new SetDockManagerBehavior(this);
        }

        public void Dispose()
        {
            _setDockManagerBehavior?.Dispose();
            _setDockManagerBehavior = null;
            BeforeItemsSet();
        }

        protected override void BeforeItemsSet()
        {
            _removeItemBehavior?.Dispose();
            _removeItemBehavior = null;
        }

        protected override void AfterItemsSet()
        {
            _removeItemBehavior = new RemoveItemBehavior<IDockGroup>(Items);
        }
    }
}
