using System;
using Moq;
using NUnit.Framework;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class UsersManagementServiceTests {
		protected Mock<IServiceProvider> serviceProviderMock;

		[SetUp]
		public void Setup() {
		}

		[Test]
		public void Get_User_Test() {

			Assert.Pass();
		}
	}
}
