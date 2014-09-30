using System;
using System.Collections.Generic;

namespace Mechavian.NodeService.Services.Impl
{
    internal class NodeServiceFactory : INodeServiceFactory
    {
        private readonly Func<NodeServiceBase> _factory;

        public NodeServiceFactory(Func<NodeServiceBase> factory)
        {
            _factory = factory;
        }

        public NodeServiceBase[] CreateInstances(int instanceCount)
        {
            if (instanceCount <= 0) throw new ArgumentOutOfRangeException("instanceCount", instanceCount, "Instance count must be greater than 0");

            var services = new List<NodeServiceBase>();
            for (int i = 0; i < instanceCount; i++)
            {
                var service = _factory();
                if (service == null)
                {
                    throw new InvalidOperationException("Service Factory cannot return null");
                }

                UpdateServiceName(service, instanceCount > 1 ? i : -1);

                service.Initialize();
                services.Add(service);
            }

            return services.ToArray();
        }

        private void UpdateServiceName(NodeServiceBase service, int index)
        {
            var serviceName = service.ServiceName;
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = service.GetType().Name;
            }

            if (index != -1)
            {
                serviceName = string.Format("{0} ({1})", serviceName, index + 1);
            }

            service.ServiceName = serviceName;
        }
    }
}