using System.ComponentModel.Composition.Hosting;
using Mechavian.NodeService.UI;

namespace Mechavian.NodeService.Services.Impl
{
    internal class UIControllerFactory : IUIControllerFactory
    {
        public IUIController GetUIController()
        {
            var catalog = new AssemblyCatalog("Mechavian.NodeService.UI.dll");
            var container = new CompositionContainer(catalog);
            return container.GetExportedValue<IUIController>();
        }
    }
}