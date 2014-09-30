using System.Collections.Generic;

namespace Mechavian.NodeService.UI
{
    internal interface IUIController
    {
        void ShowWindow(IEnumerable<NodeServiceBase> services, string[] args);
    }
}