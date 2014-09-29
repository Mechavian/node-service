using System.Diagnostics;
using Mechavian.NodeService;
using log4net;
using System.Threading;
using System;

namespace NodeService.TestService
{
    public partial class MyTestService : NodeServiceBase
    {
        static Random rng = new Random();

        public MyTestService()
        {
        }

        protected override void OnStarting(string[] args)
        {
            Log.Info("Starting");
            var wait = rng.NextDouble();
            wait = 1 - (wait * wait);
            Thread.Sleep((int)(5000 * wait));
            Log.Info("Done Starting");
        }

        protected override void OnStopping()
        {
            Log.Info("Stopping");
            var wait = rng.NextDouble();
            wait = 1 - (wait * wait);
            Thread.Sleep((int)(5000 * wait));
            Log.Info("Done Stopping");
        }
    }
}
