using System.ServiceProcess;

namespace Mechavian.NodeService.Services
{
    internal interface IServiceBaseService
    {
        void Run(ServiceBase[] services);
    }
}