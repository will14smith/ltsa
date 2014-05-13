using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LTSASharp.Graphical.Controls
{
    public class EllipseDecorator : Border
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            var a = ActualWidth / 2;
            var b = ActualHeight / 2;

            var centerPoint = new Point(a, b);
            var thickness = BorderThickness.Left;

            var ellipse = new EllipseGeometry(centerPoint, a, b);

            drawingContext.PushClip(ellipse);
            drawingContext.DrawGeometry(Background, new Pen(BorderBrush, thickness), ellipse);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var a = finalSize.Width / 2;
            var b = finalSize.Height / 2;

            var x = a * Math.Cos(45 * Math.PI / 180);
            var y = b * Math.Sin(45 * Math.PI / 180);

            var rect = new Rect(new Point(a - x, b - y), new Point(a + x, b + y));

            if (Child != null)
                Child.Arrange(rect);

            return finalSize;
        }
    }

}
