using System.Windows;

namespace LTSASharp.Graphical
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            const string prog = "SWITCH = OFF, OFF = (on -> ON), ON = (off -> OFF).";
            //const string prog = "INPUTSPEED = (engineOn -> CHECKSPEED), CHECKSPEED = (speed -> CHECKSPEED |engineOff -> INPUTSPEED).";
            //const string prog = "BUFF = (in[i:0..3] -> out[i] -> BUFF).";
            //const string prog = "SUM = (in[a:0..2][b:0..2] -> TOTAL[a+b]), TOTAL[s:0..4] = (out[s] -> SUM).";
            //const string prog = "INPUTSPEED = (engineOn -> CHECKSPEED), CHECKSPEED = (speed -> CHECKSPEED |engineOff -> INPUTSPEED)." +
            //                    "TEST = (a -> b -> TEST)." +
            //                    "||COMP = (INPUTSPEED || TEST).).";
            //const string prog = "MAKE_A   = (makeA->ready->used->MAKE_A)." +
            //                    "MAKE_B   = (makeB->ready->used->MAKE_B)." +
            //                    "ASSEMBLE = (ready->assemble->used->ASSEMBLE)." +
            //                    "||FACTORY = (MAKE_A || MAKE_B || ASSEMBLE).";

            Machine.FspDescription = prog;
        }
    }
}
