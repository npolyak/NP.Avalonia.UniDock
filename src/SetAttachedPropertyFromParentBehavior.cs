// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using Avalonia;
using NP.Concepts.Behaviors;
using System.Collections.Generic;
using System;
using NP.Utilities;
using System.Linq;

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
            IDockGroup? topGroup = Parent?.GetDockGroupSelfAndAncestors().LastOrDefault();

            SetItemValue(item);

            if (topGroup != null)
            {
                DockStaticEvents.FirePossibleDockChangeHappenedInsideEvent(topGroup);
            }
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
