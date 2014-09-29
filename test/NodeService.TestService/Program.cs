using System;
using log4net.Config;
using Mechavian.NodeService;

namespace NodeService.TestService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            XmlConfigurator.Configure();
            NodeServiceRunner.Run<MyTestService>(10);
        }
    }
}
