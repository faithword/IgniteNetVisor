using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cluster;
using Apache.Ignite.Core.Discovery;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
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

            Nodes = new ObservableCollection<IClusterNode>(ignite.GetCluster().ForRemotes().GetNodes());

            ignite.GetEvents().EnableLocal(EventType.DiscoveryAll);
            ignite.GetEvents().LocalListen(this, EventType.DiscoveryAll);
        }

        public static async Task<Cluster> Connect()
        {
            var tasks = GetDiscos().SelectMany(disco => new[] {null, "127.0.0.1"}.Select(lh => new {disco, lh}))
                .Select(pair => new IgniteConfiguration
                {
                    ClientMode = true,
                    DiscoverySpi = pair.disco,
                    Localhost = pair.lh
                }).Select(Connect);

            // TODO: Try one by one with timeout instead of WhenAny. Ignite blocks internally.
            var completeTask = await Task.WhenAny(tasks);

            return completeTask.Result;
        }

        public static Task<Cluster> Connect(IgniteConfiguration cfg)
        {
            cfg.ClientMode = true;
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        return new Cluster(Ignition.Start(cfg));
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }
            });
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

        /// <summary>
        /// Gets anticipated discovery modes.
        /// </summary>
        private static IEnumerable<IDiscoverySpi> GetDiscos()
        {
            // default multicast
            yield return null;

            // static
            yield return new TcpDiscoverySpi
            {
                IpFinder = new TcpDiscoveryStaticIpFinder
                {
                    Endpoints = new[] {"127.0.0.1:47500..47509"}
                },
            };
        }
    }
}
