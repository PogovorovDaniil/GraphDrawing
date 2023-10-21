using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace GraphDrawing.Helpers
{
    internal static class FrameworkElementHelper
    {
        internal static void SetX(this FrameworkElement element, double x) => Canvas.SetLeft(element, x - element.Width / 2);
        internal static void SetY(this FrameworkElement element, double y) => Canvas.SetTop(element, y - element.Height / 2);
        internal static void SetPosition(this FrameworkElement element, Vector2 position)
        {
            element.SetX(position.X);
            element.SetY(position.Y);
        }
    }
}
