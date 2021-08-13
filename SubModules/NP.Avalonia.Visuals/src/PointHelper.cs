using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NP.Utilities;

namespace NP.Avalonia.Visuals
{
    public static class PointHelper
    {
        public static Point2D ToPoint2D(this Point point)
        {
            return new Point2D(point.X, point.Y);
        }


        public static Point2D ToPoint2D(this PixelPoint point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static Point ToPoint(this Point2D pt)
        {
            return new Point(pt.X, pt.Y);
        }

        public static PixelPoint ToPixelPoint(this Point2D pt)
        {
            return new PixelPoint((int) pt.X, (int) pt.Y);
        }

        public static Point2D ToPoint2D(this Size size)
        {
            return new Point2D(size.Width, size.Height);
        }

        public static Size ToSize(this Point2D pt)
        {
            return new Size(pt.X, pt.Y);
        }

        public static Point2D MinimumDragDistance =>
            new Point2D(3.2, 3.2);

        public static Rect ToRect(this Rect2D rect)
        {
            return new Rect(rect.StartPoint.ToPoint(), rect.EndPoint.ToPoint());
        }

        public static Rect2D ToRect2D(this Rect rect)
        {
            return new Rect2D(rect.TopLeft.ToPoint2D(), rect.BottomRight.ToPoint2D());
        }

        public static Point2D GetSize(this Control c)
        {
            return new Point2D(c.Bounds.Width, c.Bounds.Height);
        }

        public static bool IsPointWithinControl(this Control c, Point p)
        {
            Rect2D bounds = new Rect2D(new Point2D(), c.GetSize());

            return bounds.ContainsPoint(p.ToPoint2D());
        }

        public static Rect2D GetScreenBounds(this IInputElement c)
        {
            PixelPoint startPoint = c.PointToScreen(new Point(0, 0));
            PixelPoint endPoint = c.PointToScreen(new Point(c.Bounds.Width, c.Bounds.Height));

            return new Rect2D(startPoint.ToPoint2D(), endPoint.ToPoint2D());
        }

        public static bool IsPointerWithinControl(this Control c, PointerEventArgs e)
        {
            return c.IsPointWithinControl(e.GetPosition(c));
        }
    }
}
