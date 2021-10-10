using Avalonia;
using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class AttachedToPlainBindingBehavior<TSource, TProp, TTarget>
        where TSource : IControl
    {
        SetItemsBehavior<TTarget, TProp> _setItemsBehavior;

        public AttachedToPlainBindingBehavior(TSource sourceObj, AttachedProperty<TProp> attachedPropToWatch, IList<TTarget> targets, Action<TTarget, TProp> setter)
        {
            _setItemsBehavior = new SetItemsBehavior<TTarget, TProp>(setter);
         
            sourceObj.GetObservable(attachedPropToWatch).Subscribe(OnAttachedPropChanged);

            OnAttachedPropChanged(sourceObj.GetValue(attachedPropToWatch));

            _setItemsBehavior.Items = targets;
        }

        private void OnAttachedPropChanged(TProp val)
        {
            _setItemsBehavior.Val = val;
        }
    }
}
