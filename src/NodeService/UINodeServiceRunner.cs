using System;
using System.Collections.Generic;
using Mechavian.NodeService.Services;

namespace Mechavian.NodeService
{
    internal class UINodeServiceRunner
    {
        private readonly IServiceProvider _serviceProvider;

        internal UINodeServiceRunner(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
        }

        internal void RunWithUI(NodeServiceBase[] services)
        {
            var uiControllerFactory = _serviceProvider.GetService<IUIControllerFactory>();
            var environmentService = _serviceProvider.GetService<IEnvironmentService>();

            var controller = uiControllerFactory.GetUIController();

            try
            {
                controller.ShowWindow(services, environmentService.GetCommandLineArgs());
            }
            finally
            {
                ShutdownServices(services);
            }
        }

        private static void ShutdownServices(IEnumerable<NodeServiceBase> services)
        {
            foreach (var service in services)
            {
                service.StopImpl();
                service.Dispose();
            }
        }
    }
}
