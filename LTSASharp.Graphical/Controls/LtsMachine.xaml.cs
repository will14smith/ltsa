using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Antlr4.Runtime;
using LTSASharp.Fsp;
using LTSASharp.Fsp.Conversion;
using LTSASharp.Lts;
using LTSASharp.Lts.Conversion;
using LTSASharp.Parsing;

namespace LTSASharp.Graphical.Controls
{
    /// <summary>
    /// Interaction logic for LtsMachine.xaml
    /// </summary>
    public partial class LtsMachine : UserControl
    {
        private string prog;
        private double spacingH = 100;
        private double spacingV = 100;
        private double size = 30;

        private LtsSystem system;

        public LtsMachine()
        {
            InitializeComponent();
        }

        public string FspDescription
        {
            get { return prog; }
            set { prog = value; Update(); }
        }

        private void Update()
        {
            CompileSystem();

            var stateMap = new Dictionary<LtsState, int>();
            var i = 1;

            foreach (var state in system.States.Where(x => x.Number >= 0).OrderBy(x => x.Number))
            {
                LtsCanvas.Children.Add(DrawState(state, i));

                stateMap.Add(state, i++);
            }

            // handle special cases
            if (system.States.Contains(LtsState.End))
            {
                LtsCanvas.Children.Add(DrawState(LtsState.End, i));

                stateMap.Add(LtsState.End, i++);
            }

            //TODO merge parallel edges (i.e. 2->a->3, 2->b->3 == 2->{a,b}->3)
            foreach (var transition in system.Transitions)
            {
                var from = stateMap[transition.Source];
                var to = stateMap[transition.Destination];

                LtsCanvas.Children.Add(DrawTransition(transition, from, to));
            }
        }

        private FrameworkElement DrawTransition(LtsAction transition, int sourcePos, int destPos)
        {
            var canvas = new Canvas();

            if (sourcePos == destPos)
            {
                //TODO draw loop
                return canvas;
            }

            var arc = new Path
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2
            };

            //TODO position either side (not at top/bottom)
            var offsetV = size / 2;
            var offsetH = sourcePos < destPos ? 0 : size;

            var startX = sourcePos * spacingV + offsetV;
            var startY = spacingH + offsetH;

            var endY = spacingH + offsetH;
            var endX = destPos * spacingV + offsetV;

            var startPoint = new Point(startX, startY);
            var endPoint = new Point(endX, endY);

            // Linear W/H looks a bit silly 5W/log(10H) looks better
            var arcSize = new Size(5, Math.Log(10d * Math.Abs(sourcePos - destPos)));

            var segments = new[] { new ArcSegment(endPoint, arcSize, 0, false, SweepDirection.Clockwise, true) };

            arc.Data = new PathGeometry(new[] { new PathFigure(startPoint, segments, false) });

            var text = new TextOnPathControl
            {
                Text = transition.Action.ToString(),
                PathFigure = new PathFigure(startPoint, segments, false),
                Margin = new Thickness(0, sourcePos < destPos ? -10 : 10, 0, 0)
            };

            var arrow = new Polygon
            {
                Fill = new SolidColorBrush(Colors.Black)
            };

            arrow.Points.Add(new Point(0, 0));
            arrow.Points.Add(new Point(5, 5));
            arrow.Points.Add(new Point(0, 10));

            if (sourcePos > destPos)
            {
                arrow.RenderTransform = new ScaleTransform(-1, 1);
            }

            var arrowLeft = ((sourcePos + destPos) * spacingV + size - 5) / 2;
            var arcHeight = arc.Data.Bounds.Height;
            var arrowTop = spacingH + (sourcePos > destPos ? arcHeight + size : -arcHeight) - 5;

            arrow.Margin = new Thickness(arrowLeft, arrowTop, 0, 0);

            canvas.Children.Add(arc);
            canvas.Children.Add(text);
            canvas.Children.Add(arrow);

            return canvas;
        }
        private FrameworkElement DrawState(LtsState state, int xPos)
        {
            var orb = new EllipseDecorator();

            orb.Width = orb.Height = size;

            orb.Background = new SolidColorBrush(state == system.InitialState ? Colors.Red : Colors.Cyan);
            orb.BorderThickness = new Thickness(2);
            orb.BorderBrush = new SolidColorBrush(Colors.Black);

            orb.Child = new TextBlock
            {
                Text = GetStateText(state),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold
            };

            orb.Margin = new Thickness(xPos * spacingH, spacingV, 0, 0);

            return orb;
        }
        private static string GetStateText(LtsState state)
        {
            if (state.Number >= 0)
                return state.Number.ToString();

            if (state == LtsState.End)
                return "E";

            throw new InvalidOperationException();
        }

        private void CompileSystem()
        {
            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            // take last one for now
            system = lts.Systems.Last().Value;
        }


        protected FspDescription CompileFsp(string input)
        {
            return CompileFsp(new AntlrInputStream(input));
        }

        protected FspDescription CompileFsp(AntlrInputStream input)
        {
            var lexer = new FSPActualLexer(input);
            var parser = new FSPActualParser(new BufferedTokenStream(lexer));

            var fspConverter = new FspConveter();
            parser.fsp_description().Accept(fspConverter);

            return fspConverter.Description;
        }

        protected LtsDescription CompileLts(FspDescription fsp)
        {
            var ltsConverter = new LtsConverter(fsp);

            return ltsConverter.Convert();
        }

    }
}
