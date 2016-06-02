using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cluster;
using Apache.Ignite.Core.Events;

namespace Apache.Ignite.Visor.Model
{
    internal class Cluster : IEventListener<DiscoveryEvent>
    {
        // TODO: Unmanaged console output

        private readonly IIgnite _ignite;

        private Cluster(IIgnite ignite)
        {
            _ignite = ignite;

            Nodes = new ObservableCollection<IClusterNode>(ignite.GetCluster().GetNodes());

            ignite.GetEvents().EnableLocal(EventType.DiscoveryAll);
            ignite.GetEvents().LocalListen(this, EventType.DiscoveryAll);
        }

        public static Task<Cluster> Connect(IgniteConfiguration cfg)
        {
            cfg.ClientMode = true;
            return Task.Factory.StartNew(() => new Cluster(Ignition.Start(cfg)));
        }

        public ObservableCollection<IClusterNode> Nodes { get; }

        public bool Invoke(DiscoveryEvent evt)
        {
            if (evt.Type == EventType.NodeJoined)
            {
                Nodes.Add(evt.EventNode);
            }
            else if (evt.Type == EventType.NodeLeft)
            {
                Nodes.Remove(evt.EventNode);
            }

            return true;
        }
    }
}
