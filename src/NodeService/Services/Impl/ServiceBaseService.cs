using System.ServiceProcess;

namespace Mechavian.NodeService.Services.Impl
{
    internal class ServiceBaseService : IServiceBaseService
    {
        public void Run(ServiceBase[] services)
        {
            ServiceBase.Run(services);
        }
    }
}