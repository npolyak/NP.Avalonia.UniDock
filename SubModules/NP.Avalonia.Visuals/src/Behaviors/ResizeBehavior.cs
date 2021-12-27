using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NP.Avalonia.Visuals.Controls;
using NP.Utilities;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class ResizeBehavior
    {
        private CustomWindow _window;
        private WindowEdge? _edge;
        private Point? _startPointerPoint;
        private Point? _originalWindowPosition;
        private Size? _originalWindowSize;
        public double RenderScaling => _window.PlatformImpl.RenderScaling;

        public ResizeBehavior(CustomWindow window)
        {
            _window = window;
        }
        public Point GetCurrentPointerPositionInScreen(PointerEventArgs e)
        {
            Point currentPointerPositionInScreen =
                _window.PointToScreen(e.GetPosition(_window)).ToPoint(RenderScaling);
            return currentPointerPositionInScreen;
        }

        private void SetPositionAndSize(PointerEventArgs e)
        {

            Point currentPointerPositionInScreen = GetCurrentPointerPositionInScreen(e);

            Point diff = currentPointerPositionInScreen - _startPointerPoint!.Value;

            var rc = new Rect(_originalWindowPosition!.Value, _originalWindowSize!.Value);

            if (_edge == WindowEdge.West || _edge == WindowEdge.NorthWest || _edge == WindowEdge.SouthWest)
            {
                rc = rc.WithX(rc.X + diff.X).WithWidth(rc.Width - diff.X); ;
            }
            if (_edge == WindowEdge.North || _edge == WindowEdge.NorthWest || _edge == WindowEdge.NorthEast)
            {
                rc = rc.WithY(rc.Y + diff.Y).WithHeight(rc.Height - diff.Y);
            }
            if (_edge == WindowEdge.East || _edge == WindowEdge.NorthEast || _edge == WindowEdge.SouthEast)
            {
                rc = rc.WithWidth(rc.Width + diff.X);
            }
            if (_edge == WindowEdge.South || _edge == WindowEdge.SouthWest || _edge == WindowEdge.SouthEast)
            {
                rc = rc.WithHeight(rc.Height + diff.Y);
            }

            PixelPoint rcPosition = rc.Position.ToPixelPoint(RenderScaling);

            if (_window.Position != rcPosition)
            {
                _window.Position = rcPosition;
            }
            if (_window.ClientSize != rc.Size)
            {
                _window.Resize(rc.Size);
            }
        }


        public void StartDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            if (e.MouseButton != MouseButton.Left)
                return;

            _edge = edge;
            _startPointerPoint = GetCurrentPointerPositionInScreen(e);

            e.Pointer.Capture(_window);
            _originalWindowPosition = _window.Position.ToPoint(RenderScaling);
            _originalWindowSize = _window.ClientSize;

            _window.PointerReleased += _capturedControl_PointerReleased;
            _window.PointerMoved += _capturedControl_PointerMoved;
        }

        private void _capturedControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _window!.PointerReleased -= _capturedControl_PointerReleased;
            _window.PointerMoved -= _capturedControl_PointerMoved;

            SetPositionAndSize(e);

            e.Pointer.Capture(null);

            _edge = null;
        }

        private void _capturedControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            SetPositionAndSize(e);
        }
    }
}
