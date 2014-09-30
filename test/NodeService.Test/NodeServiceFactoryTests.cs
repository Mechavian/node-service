using System;
using Mechavian.NodeService;
using Mechavian.NodeService.Services.Impl;
using Moq;
using NUnit.Framework;

namespace NodeService.Test
{
    [TestFixture]
    public class NodeServiceFactoryTests
    {
        [Test]
        public void NodeServiceFactory_CreateInstances_0InstancesThrows()
        {
            var uat = new NodeServiceFactory(Mock.Of<NodeServiceBase>);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                uat.CreateInstances(0);
            });
        }

        [Test]
        public void NodeServiceFactory_CreateInstances_NullReturnedFromFactoryThrows()
        {
            var uat = new NodeServiceFactory(() => null);

            Assert.Throws<InvalidOperationException>(() =>
            {
                uat.CreateInstances(1);
            });
        }

        [Test]
        public void NodeServiceFactory_CreateInstances_CreatesAndInitializesService()
        {
            var service = new Mock<NodeServiceBase>();

            var uat = new NodeServiceFactory(() => service.Object);

            var result = uat.CreateInstances(1);
            CollectionAssert.AreEqual(new[] {service.Object}, result);
            Assert.IsNotNull(result[0].Log, "Log has been initialized");
        }

        [Test]
        public void NodeServiceFactory_CreateInstances_InitializesUnsetNameToType()
        {
            var uat = new NodeServiceFactory(() => new MockNodeService());

            var result = uat.CreateInstances(1);
            Assert.AreEqual("MockNodeService", result[0].ServiceName);
        }

        [Test]
        public void NodeServiceFactory_CreateInstances_DoesntChangeSetName()
        {
            var uat = new NodeServiceFactory(() => new MockNodeService()
            {
                ServiceName = "FOO"
            });

            var result = uat.CreateInstances(1);
            Assert.AreEqual("FOO", result[0].ServiceName);
        }

        [Test]
        public void NodeServiceFactory_CreateInstances_AppendsIndexWhenMultiple()
        {
            var uat = new NodeServiceFactory(() => new MockNodeService()
            {
                ServiceName = "FOO"
            });

            var result = uat.CreateInstances(3);
            Assert.AreEqual("FOO (1)", result[0].ServiceName);
            Assert.AreEqual("FOO (2)", result[1].ServiceName);
            Assert.AreEqual("FOO (3)", result[2].ServiceName);
        }


        class MockNodeService : NodeServiceBase
        {
            protected override void OnStarting(string[] args)
            {
                throw new NotImplementedException();
            }

            protected override void OnStopping()
            {
                throw new NotImplementedException();
            }
        }
    }
}
