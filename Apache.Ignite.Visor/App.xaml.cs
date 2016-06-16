using System;
using System.Threading;
using System.Windows;
using Apache.Ignite.Visor.Model;

namespace Apache.Ignite.Visor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                int idx = int.Parse(e.Args[0]);

                var thread = new Thread(() => Cluster.StartIgnite(idx));

                thread.Start();

                var exitCode = thread.Join(TimeSpan.FromSeconds(10)) ? 0 : 1;

                Environment.Exit(exitCode);
            }

            base.OnStartup(e);
        }
    }
}
