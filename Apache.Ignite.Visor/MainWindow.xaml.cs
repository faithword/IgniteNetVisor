using System.Windows;
using Apache.Ignite.Visor.Model;

namespace Apache.Ignite.Visor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainWindow(Cluster cluster)
        {
            InitializeComponent();

            DataContext = cluster;
        }
    }
}
