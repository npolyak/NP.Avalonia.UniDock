using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.VisualTree;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class CurrentScreenPointBehavior
    {
        private static Subject<PixelPoint> _currentScreenPoint = new Subject<PixelPoint>();
        public static IObservable<PixelPoint> CurrentScreenPoint => _currentScreenPoint;

        public static PixelPoint CurrentScreenPointValue { get; private set; } = new PixelPoint();

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

            CurrentScreenPointValue = position;
            _currentScreenPoint.OnNext(position);
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

        private static void Control_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (CapturedControl != null)
            {
                CapturedControl.PointerReleased -= Control_PointerReleased;
            }

            Mouse?.Capture(null);
            _capturedWindow = null;
        }
    }
}
