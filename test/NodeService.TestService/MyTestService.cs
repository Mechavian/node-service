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
            Thread.Sleep((int)(2000 * wait));
            Log.Info("Done Starting");

            Log.Error("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");

            var logCount = rng.Next(0, 50);
            while (logCount-- > 0)
            {
                Log.DebugFormat("Message #{0}", logCount);
            }
        }

        protected override void OnStopping()
        {
            Log.Info("Stopping");
            var wait = rng.NextDouble();
            wait = 1 - (wait * wait);
            Thread.Sleep((int)(2000 * wait));
            Log.Info("Done Stopping");
        }
    }
}
