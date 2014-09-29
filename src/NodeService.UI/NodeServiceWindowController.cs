using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Mechavian.NodeService.UI.ViewModels;
using Mechavian.NodeService.UI.Views;

namespace Mechavian.NodeService.UI
{
    [Export(typeof(IUIController))]
    internal class NodeServiceWindowController : IUIController
    {
        public void ShowWindow(IEnumerable<NodeServiceBase> services, string[] args)
        {
            bool shuttingDown = false;
            bool shutDown = false;

            using (var vm = new NodeServiceWindowViewModel()
            {
                Services = new ObservableCollection<NodeServiceViewModel>(services.Select((s) => new NodeServiceViewModel(s, args)))
            })
            {
                var window = new NodeServiceWindow()
                {
                    DataContext = vm
                };

                window.Closing += async (sender, e) =>
                {
                    if (shutDown) return;

                    e.Cancel = true;
                    if (shuttingDown) return;

                    shuttingDown = true;
                    await vm.StopAll();

                    shutDown = true;
                    window.Dispatcher.BeginInvoke(new Action(window.Close));
                };

                window.ShowDialog();
            }
        }
    }
}