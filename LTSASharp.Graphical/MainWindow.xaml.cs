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
            const string prog = "SWITCH = (on->off->SWITCH)." +
                                "||SWITCHES(N=3) = (forall[i:1..N] s[i]:SWITCH).";

            Machine.FspDescription = prog;
        }
    }
}
