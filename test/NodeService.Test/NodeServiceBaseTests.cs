using System;
using System.Collections.Generic;
using log4net;
using Mechavian.NodeService;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace NodeService.Test
{
    [TestFixture]
    public class NodeServiceBaseTests
    {
        [Test]
        public void NodeServiceBase_CtorDefaults()
        {
            var uat = Mock.Of<NodeServiceBase>();

            Assert.AreEqual(ServiceStatus.Stopped, uat.Status);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var log = uat.Log;
            });
        }

        [Test]
        public void NodeServiceBase_Initialize_LogAvailable()
        {
            var uat = Mock.Of<NodeServiceBase>();

            uat.Initialize();
            Assert.IsInstanceOf<ILog>(uat.Log);
        }

        [Test]
        public void NodeServiceBase_StartImpl_CallsOnStarting()
        {
            var statusChangedArgs = new List<ServiceStatus>();
            var mock = new Mock<NodeServiceBase>();
            var uat = mock.Object;
            uat.StatusChanged += (o, s) => statusChangedArgs.Add(s);

            uat.Initialize();
            var args = new[] {"foo"};
            uat.StartImpl(args);

            mock.Protected().Verify("OnStarting", Times.Once(), new object[] {args});

            Assert.AreEqual(ServiceStatus.Running, uat.Status);
            CollectionAssert.AreEqual(new[] {ServiceStatus.Starting, ServiceStatus.Running}, statusChangedArgs);
        }

        [Test]
        public void NodeServiceBase_StopImpl_NotStartedDoesntCallOnStopping()
        {
            var statusChangedArgs = new List<ServiceStatus>();
            var mock = new Mock<NodeServiceBase>();
            var uat = mock.Object;
            uat.StatusChanged += (o, s) => statusChangedArgs.Add(s);
            uat.Initialize();

            uat.StopImpl();

            mock.Protected().Verify("OnStopping", Times.Never());
            Assert.AreEqual(ServiceStatus.Stopped, uat.Status);
            CollectionAssert.AreEqual(new ServiceStatus[0], statusChangedArgs);
        }

        [Test]
        public void NodeServiceBase_StopImpl_CallsOnStopping()
        {
            var mock = new Mock<NodeServiceBase>();
            var uat = mock.Object;

            uat.Initialize();
            uat.StartImpl(new string[0]);
            var statusChangedArgs = new List<ServiceStatus>();
            uat.StatusChanged += (o, s) => statusChangedArgs.Add(s);

            uat.StopImpl();

            mock.Protected().Verify("OnStopping", Times.Once());
            Assert.AreEqual(ServiceStatus.Stopped, uat.Status);
            CollectionAssert.AreEqual(new[] { ServiceStatus.Stopping, ServiceStatus.Stopped }, statusChangedArgs);
        }
    }
}
