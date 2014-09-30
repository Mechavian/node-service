using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mechavian.NodeService.UI.ViewModels
{
    internal class NodeServiceWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DelegateCommand _startAllCommand;
        private readonly DelegateCommand _stopAllCommand;

        public NodeServiceWindowViewModel()
        {
            Services = new ObservableCollection<NodeServiceViewModel>();

            _startAllCommand = new DelegateCommand(o => StartAll());
            _stopAllCommand = new DelegateCommand(o => StopAll());
        }

        public ObservableCollection<NodeServiceViewModel> Services { get; set; }

        public ICommand StartAllCommand
        {
            get { return _startAllCommand; }
        }

        public ICommand StopAllCommand
        {
            get { return _stopAllCommand; }
        }

        public void Dispose()
        {
            foreach (NodeServiceViewModel service in Services)
            {
                service.Dispose();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task StopAll()
        {
            List<Task> tasks = Services.Select(service => service.Stop()).ToList();
            return Task.WhenAll(tasks);
        }

        public Task StartAll()
        {
            List<Task> tasks = Services.Select(service => service.Start()).ToList();
            return Task.WhenAll(tasks);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}