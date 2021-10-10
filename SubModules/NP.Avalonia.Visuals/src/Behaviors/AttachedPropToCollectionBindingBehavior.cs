using Avalonia;
using System;
using System.Collections.Generic;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class AttachedPropToCollectionBindingBehavior<TProp, TTarget> : 
        ToCollectionBindingBehavior<TProp, TTarget>
    {
        public AttachedPropToCollectionBindingBehavior
        (
            IAvaloniaObject source, 
            AvaloniaProperty<TProp> attachedProp, 
            IEnumerable<TTarget> collection,
            Action<TTarget, TProp> setter) 
            : 
            base(new AttachedPropValueGetter<TProp>(source, attachedProp), collection, setter)
        {

        }
    }
}
