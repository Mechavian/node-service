namespace Mechavian.NodeService.Services
{
    internal interface INodeServiceFactory
    {
        NodeServiceBase[] CreateInstances(int instanceCount);
    }
}