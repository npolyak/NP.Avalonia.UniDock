using Avalonia.Controls;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.AvaloniaDock
{
    public class SimpleDockGroup : Control, IDockGroup, IDisposable
    {
        public event Action<IRemovable>? RemoveEvent;

        public void Remove()
        {
            RemoveEvent?.Invoke(this);
        }

        public DockManager TheDockManager
        {
            get => DockAttachedProperties.GetTheDockManager(this);
            set => DockAttachedProperties.SetTheDockManager(this, value);
        }

        public IDockGroup? DockParent
        {
            get => null;
            set => throw new NotImplementedException();
        }

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
        }

        private SetDockGroupBehavior _setBehavior;
        IDisposable? _behavior;
        public SimpleDockGroup()
        {
            _behavior = DockChildren?.AddBehavior(OnItemAdded, OnItemRemoved);
            _setBehavior = new SetDockGroupBehavior(this, DockChildren!);
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
