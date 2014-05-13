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
            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            // take last one for now
            var system = lts.Systems.Last().Value;

            var stateMap = new Dictionary<LtsState, int>();

            int i = 1;
            foreach (var state in system.States)
            {
                var orb = new EllipseDecorator();

                orb.Width = orb.Height = size;

                orb.Background = new SolidColorBrush(Colors.Cyan);
                orb.BorderThickness = new Thickness(2);
                orb.BorderBrush = new SolidColorBrush(Colors.Black);

                //TODO use state number
                orb.Child = new TextBlock
                {
                    Text = (i - 1).ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };

                orb.Margin = new Thickness(i * spacingH, spacingV, 0, 0);

                LtsCanvas.Children.Add(orb);

                stateMap.Add(state, i);
                i++;
            }

            //TODO merge parallel edges (i.e. 2->a->3, 2->b->3 == 2->{a,b}->3)
            foreach (var transition in system.Transitions)
            {
                var arc = new Path();
                var text = new TextOnPathControl();

                text.Text = transition.Action.ToString();

                arc.Stroke = new SolidColorBrush(Colors.Black);
                arc.StrokeThickness = 2;

                var from = stateMap[transition.Source];
                var to = stateMap[transition.Destination];

                if (from == to)
                    //TODO draw loop
                    continue;

                //TODO position either side (not at top/bottom)

                var offsetV = size / 2;
                var offsetH = from < to ? 0 : size;

                var start = new Point(from * spacingV + offsetV, spacingH + offsetH);
                var segments = new List<PathSegment>
                {
                    new ArcSegment(new Point(to * spacingV +  offsetV, spacingH + offsetH), new Size(5, Math.Log(10d * Math.Abs(from - to))), 0, false, SweepDirection.Clockwise, true)
                };

                var figure = new PathFigure(start, segments, false);

                arc.Data = new PathGeometry(new[] { figure });
                text.PathFigure = figure;

                if (from < to)
                {
                    text.Margin = new Thickness(0, -10, 0, 0);
                }
                else
                {
                    text.Margin = new Thickness(0, 10, 0, 0);
                }

                //1arc.Margin = new Thickness(Math.Min(from, to) * spacingH + (size / 2), spacingV + (size / 2), 0, 0);

                LtsCanvas.Children.Add(arc);
                LtsCanvas.Children.Add(text);

                var arrow = new Polygon();

                arrow.Fill = new SolidColorBrush(Colors.Black);

                arrow.Points.Add(new Point(0, 0));
                arrow.Points.Add(new Point(5, 5));
                arrow.Points.Add(new Point(0, 10));

                if (from > to)
                {
                    arrow.RenderTransform = new ScaleTransform(-1, 1);
                }

                var arrowL = ((from + to) * spacingV + size - 5) / 2;
                var arcHeight = arc.Data.Bounds.Height;
                var arrowT = spacingH - (from > to ? -arcHeight - size : arcHeight) - 5;

                // h = 

                arrow.Margin = new Thickness(arrowL, arrowT, 0, 0);

                LtsCanvas.Children.Add(arrow);
            }
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
