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
            const string prog = "SP(I=0) = (a[I] -> END)." +
                                "P123 = (start -> SP(1);SP(2);SP(3);END)." +
                                "LOOP = P123;LOOP.";

            Machine.FspDescription = prog;
        }
    }
}
