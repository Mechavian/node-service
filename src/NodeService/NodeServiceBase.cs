using System;
using System.ComponentModel;
using System.ServiceProcess;
using log4net;

namespace Mechavian.NodeService
{
    public enum ServiceStatus
    {
        Starting,
        Running,
        Stopping,
        Stopped,
    }

    [DesignerCategory("")]
    public abstract class NodeServiceBase : ServiceBase
    {
        private readonly object _lockObj = new object();
        private bool _isInitialized;
        private ILog _log;
        private ServiceStatus _status = ServiceStatus.Stopped;

        public ServiceStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                    lock (_lockObj)
                        if (_status != value)
                        {
                            _status = value;
                            Action<NodeServiceBase, ServiceStatus> handler = StatusChanged;
                            if (handler != null)
                            {
                                handler(this, _status);
                            }
                        }
            }
        }

        public ILog Log
        {
            get
            {
                if (!_isInitialized) throw new InvalidOperationException("Cannot access Log until fully initialized");
                return _log;
            }
            private set { _log = value; }
        }

        public event Action<NodeServiceBase, ServiceStatus> StatusChanged;

        internal void Initialize()
        {
            Log = LogManager.GetLogger(ServiceName);
            _isInitialized = true;
        }

        internal void StartImpl(string[] args)
        {
            Status = ServiceStatus.Starting;
            OnStarting(args);
            Status = ServiceStatus.Running;
        }

        internal void StopImpl()
        {
            if (Status != ServiceStatus.Stopped)
            {
                Status = ServiceStatus.Stopping;
                OnStopping();
                Status = ServiceStatus.Stopped;
            }
        }

        protected abstract void OnStarting(string[] args);

        protected abstract void OnStopping();

        protected override sealed void OnStart(string[] args)
        {
            StartImpl(args);
            base.OnStart(args);
        }

        protected override sealed void OnStop()
        {
            StopImpl();
            base.OnStop();
        }
    }
}