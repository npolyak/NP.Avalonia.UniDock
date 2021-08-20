using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class DockStackGroup : StackGroup<IDockGroup>, IDockGroup, IDisposable
    {
        public bool ShowChildHeaders { get; } = true;

        public DockManager TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value);
        }

        public IDockGroup? DockParent { get; set; }

        public IList<IDockGroup>? DockChildren => Items;

        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        public DockStackGroup()
        {
        }

        public void Dispose()
        {
            BeforeItemsSet();
        }

        IDisposable? _behavior;
        protected override void BeforeItemsSet()
        {
            _behavior?.Dispose();
            _behavior = null;
        }

        protected override void AfterItemsSet()
        {
            _behavior = new SetDockGroupBehavior(this, DockChildren!);
        }
    }
}
