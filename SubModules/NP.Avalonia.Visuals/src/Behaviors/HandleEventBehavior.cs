using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class HandleEventBehavior
    {
        #region TheEvent Attached Avalonia Property
        public static RoutedEvent GetTheEvent(IControl obj)
        {
            return obj.GetValue(TheEventProperty);
        }

        public static void SetTheEvent(IControl obj, RoutedEvent value)
        {
            obj.SetValue(TheEventProperty, value);
        }

        public static readonly AttachedProperty<RoutedEvent> TheEventProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, RoutedEvent>
            (
                "TheEvent"
            );
        #endregion TheEvent Attached Avalonia Property

        #region TheRoutingStrategies Attached Avalonia Property
        public static RoutingStrategies GetTheRoutingStrategies(IControl obj)
        {
            return obj.GetValue(TheRoutingStrategiesProperty);
        }

        public static void SetTheRoutingStrategies(IControl obj, RoutingStrategies value)
        {
            obj.SetValue(TheRoutingStrategiesProperty, value);
        }

        public static readonly AttachedProperty<RoutingStrategies> TheRoutingStrategiesProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, RoutingStrategies>
            (
                "TheRoutingStrategies",
                RoutingStrategies.Bubble
            );
        #endregion TheRoutingStrategies Attached Avalonia Property


        private static void OnEventChanged(AvaloniaPropertyChangedEventArgs<RoutedEvent> eventArgs)
        {
            IControl sender = (IControl) eventArgs.Sender;

            var oldEvent = eventArgs.OldValue.Value;

            sender.ResetEvent(oldEvent);
        }

        private static void ResetEvent(this IControl sender, RoutedEvent oldEvent)
        {
            if (oldEvent != null)
            {
                sender.RemoveHandler(oldEvent, (EventHandler<RoutedEventArgs>)OnEventFired);
            }

            var newEvent = GetTheEvent(sender);

            if (newEvent != null)
            {
                RoutingStrategies routingStrategies = GetTheRoutingStrategies(sender);

                sender.AddHandler
                (
                    newEvent,
                    (EventHandler<RoutedEventArgs>)OnEventFired,
                    routingStrategies);
            }
        }

        private static void OnRoutingStrategiesChanged(AvaloniaPropertyChangedEventArgs<RoutingStrategies> eventArgs)
        {
            IControl sender = (IControl)eventArgs.Sender;

            RoutedEvent oldEvent = GetTheEvent(sender);

            sender.ResetEvent(oldEvent);
        }

        private static void OnEventFired(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }


        static HandleEventBehavior()
        {
            TheEventProperty.Changed.Subscribe(OnEventChanged);
            TheRoutingStrategiesProperty.Changed.Subscribe(OnRoutingStrategiesChanged);
        }

    }
}
