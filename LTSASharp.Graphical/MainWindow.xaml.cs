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
            const string prog = "SERVERv2 = (accept.request->service->accept.reply->SERVERv2)." +
                                "CLIENTv2 = (call.request->call.reply->continue->CLIENTv2)." +
                                "||CLIENT_SERVERv2 = (CLIENTv2 || SERVERv2)/{call/accept}.";

            Machine.FspDescription = prog;
        }
    }
}
