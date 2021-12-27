using Avalonia;
using Avalonia.Data;
using NP.Utilities;
using System;
using System.Reactive.Subjects;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class ValueBinding<T> : IBinding
    {
        private T? _value;
        public T? Value
        {
            get => _value;
            set
            {
                if (_value.ObjEquals(value))
                    return;

                _value = value;

                if (_value != null)
                {
                    _subject = new BehaviorSubject<object>(_value);
                }
            }
        }

        private BehaviorSubject<object>? _subject;

        public InstancedBinding Initiate
        (
            IAvaloniaObject target, 
            AvaloniaProperty targetProperty, 
            object? anchor = null, 
            bool enableDataValidation = false)
        {
            return new InstancedBinding(_subject, BindingMode.OneWay, BindingPriority.LocalValue);
        }
    }
}
