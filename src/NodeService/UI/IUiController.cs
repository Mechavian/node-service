using System.Collections.Generic;

namespace Mechavian.NodeService.UI
{
    public interface IUIController
    {
        void ShowWindow(IEnumerable<NodeServiceBase> services, string[] args);
    }
}