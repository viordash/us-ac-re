using System;
using System.Reflection;
using Moq;
using NUnit.Framework;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Tests.ServicesTests {
	[TestFixture]
	public class ServiceLocatorTests {
		Mock<IServiceProvider> serviceProviderMock;

		[SetUp]
		public void Setup() {
			serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(x => x.GetService(typeof(string)))
				.Returns(() => {
					return "test";
				});
			ServiceLocator.SetLocatorProvider(serviceProviderMock.Object);
		}

		[Test]
		public void Not_Setted_LocatorProvider_Throws_InvalidOperationException() {
			var fiServiceProvider = typeof(ServiceLocator).GetField("gServiceProvider", BindingFlags.NonPublic | BindingFlags.Static);
			fiServiceProvider.SetValue(null, null);

			Assert.Throws<InvalidOperationException>(() => ServiceLocator.Current.GetInstance<string>());
		}

		[Test]
		public void Set_Null_Tol_LocatorProvider_Throws_InvalidOperationException() {
			Assert.Throws<ArgumentNullException>(() => ServiceLocator.SetLocatorProvider(null));
		}

		[Test]
		public void GetInstance_Test() {
			Assert.That(ServiceLocator.Current.GetInstance<string>(), Is.EqualTo("test"));
		}
	}
}
