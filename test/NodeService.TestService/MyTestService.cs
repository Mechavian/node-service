using Mechavian.NodeService;
using System.Threading;
using System;

namespace NodeService.TestService
{
    public class MyTestService : NodeServiceBase
    {
        static Random rng = new Random();

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
