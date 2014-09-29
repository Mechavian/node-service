using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Mechavian.NodeService.UI;

namespace Mechavian.NodeService
{
    public static class NodeServiceRunner
    {
        public static void Run<T>(int instances = 1)
            where T : NodeServiceBase, new()
        {
            Run(() => new T(), instances);
        }

        public static void Run<T>(Func<T> factory, int instances = 1)
            where T : NodeServiceBase
        {
            if (Environment.UserInteractive)
            {
                var thread = new Thread(() =>
                {
                    var runner = new NodeServiceRunner<T>(factory, instances);
                    runner.RunWithUI();
                });
                thread.SetApartmentState(ApartmentState.STA);

                thread.Start();
                thread.Join();
            }
            else
            {
                var runner = new NodeServiceRunner<T>(factory, instances);
                ServiceBase[] services = runner.CreateInstances();
                ServiceBase.Run(services);
            }
        }
    }

    internal class NodeServiceRunner<T>
        where T : NodeServiceBase
    {
        private readonly int _instances;
        private readonly Func<T> _factory;

        internal NodeServiceRunner(Func<T> factory, int instances)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (instances <= 0) throw new ArgumentOutOfRangeException("instances", instances, "Instances must be greater than 0");

            _factory = factory;
            _instances = instances;
        }


        internal void RunWithUI()
        {
            var services = CreateInstances().Cast<T>().ToArray();
            var toShutdown = new Queue<T>(services);

            var catalog = new AssemblyCatalog("Mechavian.NodeService.UI.dll");
            var container = new CompositionContainer(catalog);
            var controller = container.GetExportedValue<IUIController>();

            try
            {
                controller.ShowWindow(services, Environment.GetCommandLineArgs());
            }
            finally
            {
                ShutdownServices(toShutdown);
            }
        }

        private static void ShutdownServices(Queue<T> toShutdown)
        {
            while (toShutdown.Count != 0)
            {
                var service = toShutdown.Dequeue();

                service.StopImpl();
                service.Dispose();
            }
        }

        internal ServiceBase[] CreateInstances() 
        {
            var services = new List<ServiceBase>();
            for (int i = 0; i < _instances; i++)
            {
                var service = _factory();
                if (service == null)
                {
                    throw new InvalidOperationException("Service Factory cannot return null");
                }

                UpdateServiceName(service, i);

                service.Initialize();
                services.Add(service);
            }

            return services.ToArray();
        }

        private void UpdateServiceName(T service, int index)
        {
            var serviceName = service.ServiceName;
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = service.GetType().Name;
            }

            if (_instances > 1)
            {
                serviceName = string.Format("{0} ({1})", serviceName, index + 1);
            }

            service.ServiceName = serviceName;
        }
    }
}
