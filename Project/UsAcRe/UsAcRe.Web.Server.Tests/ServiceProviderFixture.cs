using System;
using Moq;
using NUnit.Framework;

namespace Tests.Common {
	public class ServiceProviderFixture {
		protected Mock<IServiceProvider> serviceProviderMock;

		[SetUp]
		public virtual void SetUp() {
			serviceProviderMock = new Mock<IServiceProvider>();
		}

		[TearDown]
		public virtual void TearDown() {

		}
	}
}
