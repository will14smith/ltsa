using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LTSASharp.Graphical.Controls
{
    class TextOnPathControl : UserControl
    {
        // Fields
        readonly Panel mainPanel;
        const double Fontsize = 10;

        // Dependency properties
        public static readonly DependencyProperty PathFigureProperty =
            DependencyProperty.Register("PathFigure",
                typeof(PathFigure),
                typeof(TextOnPathControl),
                new FrameworkPropertyMetadata(OnPathPropertyChanged));

        public static readonly DependencyProperty TextProperty =
            TextBlock.TextProperty.AddOwner(typeof(TextOnPathControl),
                new FrameworkPropertyMetadata(OnTextPropertyChanged));

        // Properties
        public PathFigure PathFigure
        {
            set { SetValue(PathFigureProperty, value); }
            get { return (PathFigure)GetValue(PathFigureProperty); }
        }

        public string Text
        {
            set { SetValue(TextProperty, value); }
            get { return (string)GetValue(TextProperty); }
        }

        // Constructors
        static TextOnPathControl()
        {
            FontFamilyProperty.OverrideMetadata(typeof(TextOnPathControl),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));
            FontStyleProperty.OverrideMetadata(typeof(TextOnPathControl),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));
            FontWeightProperty.OverrideMetadata(typeof(TextOnPathControl),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));
            FontStretchProperty.OverrideMetadata(typeof(TextOnPathControl),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));
        }

        public TextOnPathControl()
        {
            mainPanel = new Canvas();
            Content = mainPanel;
        }

        // Property-changed handlers
        static void OnFontPropertyChanged(DependencyObject obj,
                                DependencyPropertyChangedEventArgs args)
        {
            ((TextOnPathControl)obj).OrientTextOnPath();
        }

        static void OnPathPropertyChanged(DependencyObject obj,
                                DependencyPropertyChangedEventArgs args)
        {
            ((TextOnPathControl)obj).OrientTextOnPath();
        }

        static void OnTextPropertyChanged(DependencyObject obj,
                                DependencyPropertyChangedEventArgs args)
        {
            var ctrl = (TextOnPathControl)obj;
            ctrl.mainPanel.Children.Clear();

            if (String.IsNullOrEmpty(ctrl.Text))
                return;

            foreach (var textBlock in ctrl.Text.Select(ch => new TextBlock { Text = ch.ToString(), FontSize = Fontsize }))
            {
                ctrl.mainPanel.Children.Add(textBlock);
            }
            ctrl.OrientTextOnPath();
        }

        void OrientTextOnPath()
        {
            var pathLength = GetPathFigureLength(PathFigure);
            var flip = pathLength < 0;
            pathLength = flip ? -pathLength : pathLength;

            double textLength = 0;
            double maxHeight = 0;

            foreach (UIElement child in mainPanel.Children)
            {
                child.Measure(new Size(Double.PositiveInfinity,
                                       Double.PositiveInfinity));
                textLength += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }

            if (pathLength == 0 || textLength == 0)
                return;

            var pathGeometry = new PathGeometry(new[] { PathFigure });
            var baseline = Fontsize * FontFamily.Baseline;
            var progress = (pathLength - textLength) / 2 / pathLength;

            foreach (UIElement child in mainPanel.Children)
            {
                var width = child.DesiredSize.Width;
                var height = child.DesiredSize.Height;
                progress += width / 2 / pathLength;
                Point point, tangent;

                pathGeometry.GetPointAtFractionLength(flip ? 1 - progress : progress,
                                                out point, out tangent);

                TransformGroup transformGroup = new TransformGroup();

                transformGroup.Children.Add(
                    new RotateTransform((flip ? 180 : 0) + Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI, width / 2, baseline));
                transformGroup.Children.Add(
                    new TranslateTransform(point.X - width / 2,
                                           point.Y - (flip ? height - baseline : baseline)));

                child.RenderTransform = transformGroup;
                progress += width / 2 / pathLength;
            }
        }

        // Utility method
        public static double GetPathFigureLength(PathFigure pathFigure)
        {
            if (pathFigure == null)
                return 0;

            var isAlreadyFlattened = pathFigure.Segments.All(pathSegment => pathSegment is PolyLineSegment || pathSegment is LineSegment);

            var pathFigureFlattened = isAlreadyFlattened ? pathFigure : pathFigure.GetFlattenedPathFigure();
            double length = 0;
            double deltaX = 0;
            var pt1 = pathFigureFlattened.StartPoint;

            foreach (var pathSegment in pathFigureFlattened.Segments)
            {
                if (pathSegment is LineSegment)
                {
                    var pt2 = (pathSegment as LineSegment).Point;
                    length += (pt2 - pt1).Length;
                    deltaX += pt2.X - pt1.X;
                    pt1 = pt2;
                }
                else if (pathSegment is PolyLineSegment)
                {
                    var pointCollection = (pathSegment as PolyLineSegment).Points;
                    foreach (var pt2 in pointCollection)
                    {
                        length += (pt2 - pt1).Length;
                        deltaX += pt2.X - pt1.X;
                        pt1 = pt2;
                    }
                }
            }

            return deltaX < 0 ? -length : length;
        }

    }
}
