using Avalonia.Controls;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class SimpleDockGroup : Control, IDockGroupDockManagerContainer, IDisposable
    {
        DockManagerContainer IDockGroupDockManagerContainer.TheDockManagerContainer { get; } =
            new DockManagerContainer();

        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        public IDockGroup? DockParent => null;

        public IList<IDockGroup> DockChildren { get; } = 
            new ObservableCollection<IDockGroup>();

        public IDockGroup? DockChild 
        {
            get => DockChildren.FirstOrDefault();

            set
            {
                EmptyChildren();

                if (value != null)
                {
                    DockChildren.Add(value);
                }
            }
        }

        private void EmptyChildren()
        {
            DockChildren.RemoveAllOneByOne();
        }

        public void Dispose()
        {
            _behavior?.Dispose();
            _behavior = null;
            _setManagerBehavior?.Dispose();
            _setManagerBehavior = null;
        }

        IDisposable? _behavior;
        SetDockManagerBehavior? _setManagerBehavior;
        public SimpleDockGroup()
        {
            _setManagerBehavior = new SetDockManagerBehavior(this);
            _behavior = DockChildren?.AddBehavior(OnItemAdded, OnItemRemoved);
        }

        private void OnItemRemoved(IDockGroup item)
        {
            this.VisualChildren.Remove(item);
            this.LogicalChildren.Remove(item);
        }

        private void OnItemAdded(IDockGroup item)
        {
            this.VisualChildren.Add(item);
            this.LogicalChildren.Add(item);
        }
    }
}
