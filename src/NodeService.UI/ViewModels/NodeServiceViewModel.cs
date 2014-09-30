using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Mechavian.NodeService.UI.Annotations;

namespace Mechavian.NodeService.UI.ViewModels
{
    public class NodeServiceViewModel : INotifyPropertyChanged, IDisposable
    {
        private static readonly Dictionary<ServiceStatus, Color> StatusIcon = new Dictionary<ServiceStatus, Color>
        {
            {ServiceStatus.Running, Colors.Green},
            {ServiceStatus.Starting, Colors.LightGreen},
            {ServiceStatus.Stopped, Colors.Blue},
            {ServiceStatus.Stopping, Colors.LightBlue}
        };

        private readonly string[] _args;
        private readonly ObservableCollection<LoggingEventViewModel> _loggingEvents = new ObservableCollection<LoggingEventViewModel>();
        private readonly NodeServiceBase _service;
        private readonly Dispatcher _serviceDispatcher;
        private readonly DelegateCommand _startCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly Dispatcher _uiDispatcher;
        private string _displayName;
        private Color _statusColor = Colors.DarkGray;
        private string _statusText = "UNKNOWN";


        public NodeServiceViewModel([NotNull] NodeServiceBase service, string[] args)
        {
            if (service == null) throw new ArgumentNullException("service");

            _service = service;
            _args = args;
            DisplayName = _service.ServiceName;
            _startCommand = new DelegateCommand(o => Start(), o => CanStart());
            _stopCommand = new DelegateCommand(o => Stop(), o => CanStop());

            _service.StatusChanged += (o, s) => _uiDispatcher.Invoke(UpdateStatusProperties);

            _serviceDispatcher = CreateDispatcher();
            _uiDispatcher = Dispatcher.CurrentDispatcher;

            UpdateStatusProperties();
            AttachAppender(service.Log);
        }

        public NodeServiceViewModel()
        {
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public Color StatusColor
        {
            get { return _statusColor; }
            set
            {
                if (value == _statusColor) return;
                _statusColor = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LoggingEventViewModel> LoggingEvents
        {
            get { return _loggingEvents; }
        }

        public ICommand StartCommand
        {
            get { return _startCommand; }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand; }
        }

        public void Dispose()
        {
            if (_serviceDispatcher != null)
            {
                _serviceDispatcher.InvokeShutdown();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AttachAppender(ILog log)
        {
            var logger = log.Logger as Logger;
            if (logger != null)
            {
                var appender = new NodeServiceLogAppender();
                appender.LogAppended += (o, e) => _uiDispatcher.BeginInvoke(new Action(() => LoggingEvents.Add(new LoggingEventViewModel(e))));

                logger.AddAppender(appender);
            }
        }

        private Dispatcher CreateDispatcher()
        {
            Dispatcher dispatcher = null;

            var dispatcherReadyEvent = new ManualResetEvent(false);

            new Thread(() =>
            {
                dispatcher = Dispatcher.CurrentDispatcher;
                dispatcherReadyEvent.Set();
                Dispatcher.Run();
            }).Start();

            dispatcherReadyEvent.WaitOne();

            return dispatcher;
        }

        private void UpdateStatusProperties()
        {
            StatusColor = StatusIcon[_service.Status];
            StatusText = _service.Status.ToString("G");
            _startCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
        }

        public Task Start()
        {
            if (CanStart())
            {
                var tcs = new TaskCompletionSource<int>();
                _serviceDispatcher.BeginInvoke(new Action(() =>
                {
                    _service.StartImpl(_args);
                    tcs.SetResult(0);
                }));

                return tcs.Task;
            }

            return Task.FromResult(0);
        }

        public bool CanStart()
        {
            return _service.Status == ServiceStatus.Stopped;
        }

        public Task Stop()
        {
            var tcs = new TaskCompletionSource<int>();
            _serviceDispatcher.BeginInvoke(new Action(() =>
            {
                _service.StopImpl();
                tcs.SetResult(0);
            }));

            return tcs.Task;
        }

        public bool CanStop()
        {
            return _service.Status == ServiceStatus.Running;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NodeServiceLogAppender : AppenderSkeleton
    {
        public event Action<NodeServiceLogAppender, LoggingEvent> LogAppended;

        protected override void Append(LoggingEvent loggingEvent)
        {
            var handler = LogAppended;
            if (handler != null)
            {
                handler(this, loggingEvent);
            }
        }
    }
}