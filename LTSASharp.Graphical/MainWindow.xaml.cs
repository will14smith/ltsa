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
            const string prog = "ROTATOR = PAUSED," +
                                "PAUSED  = (run->RUN | pause->PAUSED | interrupt->STOP)," +
                                "RUN     = (pause->PAUSED |{run,rotate}->RUN | interrupt->STOP)." +
                                "||THREAD_DEMO = (a:ROTATOR || b:ROTATOR)" +
                                "/{stop/{a,b}.interrupt}.";

            Machine.FspDescription = prog;
        }
    }
}
