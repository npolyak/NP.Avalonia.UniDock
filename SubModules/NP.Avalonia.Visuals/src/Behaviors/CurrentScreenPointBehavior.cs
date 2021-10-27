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
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.VisualTree;
using NP.Utilities;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class CurrentScreenPointBehavior
    {
        private static Subject<Point2D> _currentScreenPoint = new Subject<Point2D>();
        public static IObservable<Point2D> CurrentScreenPoint => _currentScreenPoint;

        public static Point2D CurrentScreenPointValue { get; private set; } = new Point2D();

        public static event Action PointerReleasedEvent;

        static CurrentScreenPointBehavior()
        {
            InputManager.Instance.Process.Subscribe(OnInputReceived);
        }

        private static void OnInputReceived(RawInputEventArgs e)
        {
            if (!e.Handled && e is RawPointerEventArgs margs)
                ProcessRawEvent(margs);
        }

        private static void ProcessRawEvent(RawPointerEventArgs e)
        {
            if (_capturedWindow == null)
                return;

            var position = _capturedWindow.PointToScreen(e.Position);

            // var rootPoint = _capturedWindow.PointToClient(position);
            // var transform = _capturedWindow.TransformToVisual(_capturedWindow);
            // CurrentScreenPoint = _capturedWindow.PointToScreen(rootPoint * transform!.Value);

            CurrentScreenPointValue = position.ToPoint2D();
            _currentScreenPoint.OnNext(CurrentScreenPointValue);
        }

        public static Window _capturedWindow;

        public static Window? CapturedWindow => _capturedWindow;
        public static IInputElement CapturedControl =>
            Mouse?.Captured;

        private static IMouseDevice Mouse =>
            (_capturedWindow as IInputRoot)?.MouseDevice;

        public static void Capture(Control control)
        { 
            _capturedWindow = 
                control.GetSelfAndVisualAncestors()
                       .OfType<Window>()
                       .FirstOrDefault()!;

            Mouse?.Capture(control);

            control.PointerReleased -= Control_PointerReleased;
            control.PointerReleased += Control_PointerReleased;
        }

        public static void ReleaseCapture()
        {
            if (CapturedControl != null)
            {
                CapturedControl.PointerReleased -= Control_PointerReleased;
                CapturedControl.PointerReleased -= Control_PointerReleased;
            }

            Mouse?.Capture(null);
            _capturedWindow = null;
        }

        private static void Control_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            ReleaseCapture();

            PointerReleasedEvent?.Invoke();
        }
    }
}
