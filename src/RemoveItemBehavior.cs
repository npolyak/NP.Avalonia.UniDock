using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public class RemoveItemBehavior<T> : IDisposable
        where T : IRemovable
    {
        IDisposable? _behavior;

        private IList<T>? Items { get; set; }

        public RemoveItemBehavior(IList<T>? items)
        {
            Items = items;
            _behavior = items?.AddBehavior(OnItemAdded, OnItemRemoved);
        }

        private void OnItemAdded(T item)
        {
            item.RemoveEvent += Item_RemoveEvent;
        }

        private void OnItemRemoved(T item)
        {
            item.RemoveEvent -= Item_RemoveEvent;
        }

        private void Item_RemoveEvent(IRemovable item)
        {
            this.Items?.Remove((T)item);
        }

        public void Dispose()
        {
            _behavior?.Dispose();
            _behavior = null;
        }
    }
}
