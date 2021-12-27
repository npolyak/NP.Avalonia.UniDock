using Avalonia;
using Avalonia.Metadata;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class PropContainer : KeyedDisposable<AvaloniaProperty?>
    {
        public void Subscribe(IAvaloniaObject avaloniaObject, Action<object> subscribeAction)
        {
            Disposable = avaloniaObject.GetObservable(Key).Subscribe(subscribeAction);
        }
    }

    public class PropertiesChangeObserver : IDisposable
    {
        ReplaySubject<IAvaloniaObject>? _resultObservable = new ReplaySubject<IAvaloniaObject>();

        public IObservable<IAvaloniaObject>? ResultObservable => _resultObservable;

        private ObservableCollection<PropContainer> _props = new ObservableCollection<PropContainer>();

        [Content]
        public IEnumerable<PropContainer> PropContainers
        {
            get => _props;

            set
            {
                if (_props == value)
                {
                    return;
                }

                _props.RemoveAll();

                if (value != null)
                {
                    _props.AddRangeIfNotThere(value);
                }
            }
        }

        public IEnumerable<AvaloniaProperty>? Props
        {
            set
            {
                PropContainers = 
                    value?.Select(p => new PropContainer { Key = p })!;
            }
        }

        private IAvaloniaObject? _avaloniaObject;
        public IAvaloniaObject? TheAvaloniaObject
        {
            get => _avaloniaObject;
            set
            {
                if (_avaloniaObject == value)
                    return;

                if (_avaloniaObject != null)
                {
                    PropContainers.DoForEach(p => p.Dispose());
                }

                _avaloniaObject = value;

                if (_avaloniaObject != null)
                {
                    PropContainers.DoForEach(p => SubscribeProp(p));
                }

                Fire();
            }
        }


        IDisposable _behaviorSubscription;
        public PropertiesChangeObserver()
        {
            _behaviorSubscription = PropContainers.AddBehavior(OnPropAdded, OnPropRemoved);

            Fire();
        }

        private void Fire()
        {
            if (_avaloniaObject != null)
            {
                _resultObservable?.OnNext(_avaloniaObject);
            }
        }

        private void OnPropRemoved(PropContainer prop)
        {
            prop?.Dispose();
        }


        private void OnPropAdded(PropContainer prop)
        {
            if (_avaloniaObject != null)
            {
                SubscribeProp(prop);
                Fire();
            }
        }

        private void SubscribeProp(PropContainer prop)
        {
            prop.Subscribe(_avaloniaObject!, obj => Fire());
        }

        public void Dispose()
        {
            _resultObservable = null;
            _props.RemoveAll();
        }


        #region PropChangeObserver Attached Avalonia Property
        public static PropertiesChangeObserver? GetPropChangeObserver(IAvaloniaObject obj)
        {
            return obj.GetValue(PropChangeObserverProperty);
        }

        public static void SetPropChangeObserver(IAvaloniaObject obj, PropertiesChangeObserver? value)
        {
            obj.SetValue(PropChangeObserverProperty!, value);
        }

        public static readonly AttachedProperty<PropertiesChangeObserver> PropChangeObserverProperty =
            AvaloniaProperty.RegisterAttached<object, IAvaloniaObject, PropertiesChangeObserver>
            (
                "PropChangeObserver"
            );
        #endregion PropChangeObserver Attached Avalonia Property

        static PropertiesChangeObserver()
        {
            PropChangeObserverProperty.Changed.Subscribe(OnPropChangeObserverPropChanged);
        }

        private static void OnPropChangeObserverPropChanged(AvaloniaPropertyChangedEventArgs<PropertiesChangeObserver> args)
        {
            IAvaloniaObject sender = args.Sender;

            var oldObserver = args.OldValue.Value;

            if (oldObserver != null)
            {
                oldObserver.Dispose();
            }

            var observer = args.NewValue.Value;

            if (observer != null)
            {
                observer.TheAvaloniaObject = sender;
            }
        }
    }
}
