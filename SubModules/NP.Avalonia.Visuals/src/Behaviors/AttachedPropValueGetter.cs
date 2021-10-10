using Avalonia;
using NP.Concepts.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class AttachedPropValueGetter<TProp> : IValueGetter<TProp>
    {
        public IAvaloniaObject Source { get; }

        private AvaloniaProperty<TProp> _attachedProperty;

        public TProp GetValue() => Source.GetValue(_attachedProperty);

        public IObservable<TProp> ValueObservable { get; }

        public AttachedPropValueGetter(IAvaloniaObject source, AvaloniaProperty<TProp> attachedProperty)
        {
            Source = source;
            _attachedProperty = attachedProperty;

            ValueObservable = source.GetObservable(_attachedProperty);
        }
    }
}
