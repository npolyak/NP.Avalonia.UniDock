using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class ToCollectionBindingBehavior<TProp, TTarget>
    {
        SetItemsBehavior<TTarget, TProp> _setItemsBehavior;

        IValueGetter<TProp> _valueGetter;

        public ToCollectionBindingBehavior
        (
            IValueGetter<TProp> valueGetter, 
            IEnumerable<TTarget> targets, 
            Action<TTarget, TProp> setter)
        {
            _setItemsBehavior = new SetItemsBehavior<TTarget, TProp>(setter);
         
            _valueGetter = valueGetter;

            _valueGetter.ValueObservable.Subscribe(OnSourcePropChanged);

            OnSourcePropChanged(_valueGetter.GetValue());

            _setItemsBehavior.Items = targets;
        }

        private void OnSourcePropChanged(TProp val)
        {
            _setItemsBehavior.Val = val;
        }
    }
}
