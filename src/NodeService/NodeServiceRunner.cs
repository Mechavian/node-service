using System;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Mechavian.NodeService.Services;
using Mechavian.NodeService.Services.Impl;

namespace Mechavian.NodeService
{
    public static class NodeServiceRunner
    {
        private static IServiceProvider _services;

        internal static IServiceProvider Services
        {
            get
            {
                return _services ?? (_services = CreateServices());
            }
        }

        public static void Run<T>(int instances = 1)
            where T : NodeServiceBase, new()
        {
            Run(() => new T(), instances);
        }

        public static void Run<T>(Func<T> factory, int instances = 1)
            where T : NodeServiceBase
        {
            var serviceContainer = new ServiceContainer(Services);
            serviceContainer.AddService<INodeServiceFactory>(sp => new NodeServiceFactory(factory));

            RunImpl(serviceContainer, instances);
        }

        internal static void RunImpl(IServiceProvider serviceProvider, int instances)
        {
            var environmentService = serviceProvider.GetService<IEnvironmentService>();
            var nodeServiceFactory = serviceProvider.GetService<INodeServiceFactory>();

            if (environmentService.IsUserInteractiveMode())
            {
                var tcs = new TaskCompletionSource<int>();
                var thread = new Thread(() =>
                {
                    try
                    {
                        var runner = new UINodeServiceRunner(serviceProvider);
                        runner.RunWithUI(nodeServiceFactory.CreateInstances(instances));
                        tcs.SetResult(0);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                
                thread.Start();
                tcs.Task.Wait();
            }
            else
            {
                ServiceBase[] services = nodeServiceFactory.CreateInstances(instances).Cast<ServiceBase>().ToArray();

                var serviceBaseService = serviceProvider.GetService<IServiceBaseService>();
                serviceBaseService.Run(services);
            }
        }

        private static IServiceProvider CreateServices()
        {
            var serviceContainer = new ServiceContainer();
            serviceContainer.AddService<IEnvironmentService>(sp => new EnvironmentService());
            serviceContainer.AddService<IServiceBaseService>(sp => new ServiceBaseService());
            serviceContainer.AddService<IUIControllerFactory>(sp => new UIControllerFactory());
            return serviceContainer;
        }
    }
}