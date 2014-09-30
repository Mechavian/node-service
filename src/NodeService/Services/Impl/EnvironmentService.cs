using System;

namespace Mechavian.NodeService.Services.Impl
{
    internal class EnvironmentService : IEnvironmentService
    {
        public bool IsUserInteractiveMode()
        {
            return Environment.UserInteractive;
        }

        public string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }
    }
}