using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ServiceProcess;
using Mechavian.NodeService;
using Mechavian.NodeService.Services;
using Mechavian.NodeService.Services.Impl;
using Mechavian.NodeService.UI;
using Moq;
using NUnit.Framework;

namespace NodeService.Test
{
    [TestFixture]
    public class NodeServiceRunnerTests
    {
        [Test]
        public void NodeServiceRunner_Services_CreatesDefaultServices()
        {
            var services = NodeServiceRunner.Services;

            Assert.IsNotNull(services);
            Assert.IsInstanceOf<EnvironmentService>(services.GetService<IEnvironmentService>());
            Assert.IsInstanceOf<ServiceBaseService>(services.GetService<IServiceBaseService>());
            Assert.IsInstanceOf<UIControllerFactory>(services.GetService<IUIControllerFactory>());
        }

        [Test]
        public void NodeServiceRunner_Run_NotInteractiveMode()
        {
            var services = new[] { Mock.Of<NodeServiceBase>(), Mock.Of<NodeServiceBase>(), Mock.Of<NodeServiceBase>() };
            var mockServiceBaseService = new Mock<IServiceBaseService>();
            mockServiceBaseService.Setup(s => s.Run(It.IsAny<ServiceBase[]>()));

            var envService = Mock.Of<IEnvironmentService>(s => s.IsUserInteractiveMode() == false);

            var mockNodeFactory = new Mock<INodeServiceFactory>();
            mockNodeFactory.Setup(f => f.CreateInstances(It.IsAny<int>())).Returns(services);

            var container = new ServiceContainer();
            container.AddService(mockServiceBaseService.Object);
            container.AddService(envService);
            container.AddService(mockNodeFactory.Object);

            NodeServiceRunner.RunImpl(container, 3);

            Assert.AreEqual(3, services.Length);
            mockServiceBaseService.Verify(s => s.Run(services));
            mockNodeFactory.Verify(f => f.CreateInstances(3));
        }

        [Test]
        public void NodeServiceRunner_Run_InteractiveMode()
        {
            var services = new[] { Mock.Of<NodeServiceBase>(), Mock.Of<NodeServiceBase>(), Mock.Of<NodeServiceBase>() };
            var args = new[] { "a1", "a2", "a3" };

            var envService = Mock.Of<IEnvironmentService>(s => s.IsUserInteractiveMode() == true
                                                               && s.GetCommandLineArgs() == args);
            var mockUiController = new Mock<IUIController>();
            mockUiController.Setup(s => s.ShowWindow(It.IsAny<IEnumerable<NodeServiceBase>>(), It.IsAny<string[]>()));
            var uiControllerFactory = Mock.Of<IUIControllerFactory>(f => f.GetUIController() == mockUiController.Object);

            var mockNodeFactory = new Mock<INodeServiceFactory>();
            mockNodeFactory.Setup(f => f.CreateInstances(It.IsAny<int>())).Returns(services);

            var container = new ServiceContainer();
            container.AddService(envService);
            container.AddService(uiControllerFactory);
            container.AddService(mockNodeFactory.Object);

            NodeServiceRunner.RunImpl(container, 3);

            mockUiController.Verify(s => s.ShowWindow(services, args));
            mockNodeFactory.Verify(f => f.CreateInstances(3));
        }

        [Test]
        public void NodeServiceRunner_Run_InteractiveModeCatchesUnhandledExceptions()
        {
            var services = new[] { Mock.Of<NodeServiceBase>(), Mock.Of<NodeServiceBase>(), Mock.Of<NodeServiceBase>() };
            var args = new[] { "a1", "a2", "a3" };

            var envService = Mock.Of<IEnvironmentService>(s => s.IsUserInteractiveMode() == true
                                                               && s.GetCommandLineArgs() == args);
            var mockUiController = new Mock<IUIController>();
            mockUiController.Setup(s => s.ShowWindow(It.IsAny<IEnumerable<NodeServiceBase>>(), It.IsAny<string[]>()))
                .Callback<IEnumerable<NodeServiceBase>, string[]>((s, a) =>
                {
                    throw new InvalidOperationException("AAAGGGHHH!");
                });
            var uiControllerFactory = Mock.Of<IUIControllerFactory>(f => f.GetUIController() == mockUiController.Object);

            var mockNodeFactory = new Mock<INodeServiceFactory>();
            mockNodeFactory.Setup(f => f.CreateInstances(It.IsAny<int>())).Returns(services);

            var container = new ServiceContainer();
            container.AddService(envService);
            container.AddService(uiControllerFactory);
            container.AddService(mockNodeFactory.Object);

            var exception = Assert.Throws<AggregateException>(() =>
            {
                NodeServiceRunner.RunImpl(container, 3);
            });
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerExceptions[0]);
            Assert.AreEqual("AAAGGGHHH!", exception.InnerExceptions[0].Message);
        }
    }
}
