using Meter.View;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Testing.DB;

namespace Testing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainDB.SetEvents();
            await MainDB.Fetch();
            MainTab maintab = new();
            Hide();
            maintab.Show();
            Close();

        }
    }
}
