using Avalonia;
using NP.Concepts.Behaviors;
using System.Collections.Generic;
using System;
using NP.Utilities;

namespace NP.Avalonia.UniDock
{
    public class SetAttachedPropertyFromParentBehavior<T, TProp> : ForEachItemOverrideBehavior<T>
        where T : IDockGroup
    {
        private IDockGroup Parent { get; }
        private AttachedProperty<TProp> _attachedProp;

        private IDisposable? _subscription;

        private IEnumerable<T>? _items;
        public SetAttachedPropertyFromParentBehavior(IDockGroup parent, IEnumerable<T>? items, AttachedProperty<TProp> attachedProperty) : 
            base(items)
        {
            _items = items;
            Parent = parent;
            _attachedProp = attachedProperty;

            _subscription = _attachedProp.Changed.Subscribe(OnAttachedPropChanged);
        }

        private void OnAttachedPropChanged(AvaloniaPropertyChangedEventArgs<TProp> change)
        {
            if (!ReferenceEquals(change.Sender, Parent))
            {
                return;
            }

            _items?.DoForEach(item => SetItemValue(item));
        }

        private void SetItemValue(T item)
        {
            item.SetValue(_attachedProp, Parent.GetValue(_attachedProp));
        }

        protected override void OnItemAdded(T item)
        {
            SetItemValue(item);
        }

        protected override void OnItemRemoved(T item)
        {
            
        }

        public override void Dispose()
        {
            base.Dispose();

            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
