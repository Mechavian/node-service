using Mechavian.NodeService.UI;

namespace Mechavian.NodeService.Services
{
    internal interface IUIControllerFactory
    {
        IUIController GetUIController();
    }
}