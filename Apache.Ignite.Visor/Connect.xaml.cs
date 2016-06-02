using System.Windows;
using Apache.Ignite.Visor.Model;

namespace Apache.Ignite.Visor
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        public Connect()
        {
            InitializeComponent();

            DataContext = new ConnectViewModel(OnConnected);
        }

        private void OnConnected(Cluster cluster)
        {
            new MainWindow(cluster).Show();
            Close();
        }
    }
}
