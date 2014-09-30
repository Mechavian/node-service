using System.Collections.Generic;
using System.ComponentModel.Design;
using Mechavian.NodeService;
using Mechavian.NodeService.Services;
using Mechavian.NodeService.Stubs;
using Mechavian.NodeService.UI;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace NodeService.Test
{
    [TestFixture]
    public class UINodeServiceRunnerTests
    {
        [Test]
        public void UINodeServiceRunner_RunWithUI_ShowsWindow()
        {
            var mockService = new Mock<NodeServiceBase>();
            var service = mockService.Object;
            var args = new[] { "a1", "a2", "a3" };

            var envService = Mock.Of<IEnvironmentService>(s => s.GetCommandLineArgs() == args);
            var mockUiController = new Mock<IUIController>();
            mockUiController.Setup(s => s.ShowWindow(It.IsAny<IEnumerable<NodeServiceBase>>(), It.IsAny<string[]>()));
            var uiControllerFactory = Mock.Of<IUIControllerFactory>(f => f.GetUIController() == mockUiController.Object);

            var container = new ServiceContainer();
            container.AddService(envService);
            container.AddService(uiControllerFactory);

            var uat = new UINodeServiceRunner(container);
            uat.RunWithUI(new[] { service });

            mockUiController.Verify(s => s.ShowWindow(new[] { service }, args));
            mockService.Protected().Verify("Dispose", Times.Once(), true);
        }

        [Test]
        public void UINodeServiceRunner_RunWithUI_ShutsDownStartedServices()
        {
            var mockService = new Mock<NodeServiceBase>();
            var service = mockService.Object;
            service.Initialize();
            service.StartImpl(new string[0]);

            var envService = Mock.Of<IEnvironmentService>(s => s.GetCommandLineArgs() == new string[0]);
            var mockUiController = new Mock<IUIController>();
            mockUiController.Setup(s => s.ShowWindow(It.IsAny<IEnumerable<NodeServiceBase>>(), It.IsAny<string[]>()));
            var uiControllerFactory = Mock.Of<IUIControllerFactory>(f => f.GetUIController() == mockUiController.Object);

            var container = new ServiceContainer();
            container.AddService(envService);
            container.AddService(uiControllerFactory);

            var uat = new UINodeServiceRunner(container);
            uat.RunWithUI(new[] { service });

            Assert.AreEqual(ServiceStatus.Stopped, service.Status);
            mockService.Protected().Verify("Dispose", Times.Once(), true);
        }
    }
}
