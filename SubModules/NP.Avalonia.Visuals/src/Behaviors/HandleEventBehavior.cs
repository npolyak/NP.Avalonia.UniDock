using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class HandleEventBehavior
    {
        #region TheEvent Attached Avalonia Property
        public static RoutedEvent GetTheEvent(AvaloniaObject obj)
        {
            return obj.GetValue(TheEventProperty);
        }

        public static void SetTheEvent(AvaloniaObject obj, RoutedEvent value)
        {
            obj.SetValue(TheEventProperty, value);
        }

        public static readonly AttachedProperty<RoutedEvent> TheEventProperty =
            AvaloniaProperty.RegisterAttached<object, Control, RoutedEvent>
            (
                "TheEvent"
            );
        #endregion TheEvent Attached Avalonia Property

        static HandleEventBehavior()
        {
            TheEventProperty.Changed.Subscribe(OnEventChanged);
        }

        private static void OnEventChanged(AvaloniaPropertyChangedEventArgs<RoutedEvent> eventArgs)
        {
            IControl sender = (IControl) eventArgs.Sender;

            var oldEvent = eventArgs.OldValue.Value;

            if (oldEvent != null)
            {
                sender.RemoveHandler(oldEvent, (EventHandler<RoutedEventArgs>)OnEventFired);
            }

            var newEvent = eventArgs.NewValue.Value;

            if (newEvent != null)
            {
                sender.AddHandler
                (
                    eventArgs.NewValue.Value,
                    (EventHandler<RoutedEventArgs>)OnEventFired,
                    RoutingStrategies.Direct | RoutingStrategies.Bubble | RoutingStrategies.Tunnel);
            }
        }

        private static void OnEventFired(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
