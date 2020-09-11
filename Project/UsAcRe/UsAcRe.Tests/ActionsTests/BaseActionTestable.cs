using System.Threading;
using CommonServiceLocator;
using Moq;
using UsAcRe.Core.Services;

namespace UsAcRe.Tests.ActionsTests {
	public class BaseActionTestable {
		protected Mock<IServiceLocator> serviceLocatorMock;
		protected Mock<IAutomationElementService> automationElementServiceMock;
		protected Mock<IWinApiService> winApiServiceMock;
		protected Mock<ITestsLaunchingService> testsLaunchingServiceMock;
		protected Mock<ISettingsService> settingsServiceMock;

		public virtual void Setup() {

			serviceLocatorMock = new Mock<IServiceLocator>();
			automationElementServiceMock = new Mock<IAutomationElementService>();
			winApiServiceMock = new Mock<IWinApiService>();
			testsLaunchingServiceMock = new Mock<ITestsLaunchingService>();
			settingsServiceMock = new Mock<ISettingsService>();

			serviceLocatorMock
				.Setup(x => x.GetInstance<IAutomationElementService>())
				.Returns(() => {
					return automationElementServiceMock.Object;
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<IWinApiService>())
				.Returns(() => {
					return winApiServiceMock.Object;
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<ITestsLaunchingService>())
				.Returns(() => {
					return testsLaunchingServiceMock.Object;
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<ISettingsService>())
				.Returns(() => {
					return settingsServiceMock.Object;
				});


			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return new CancellationToken(true);
				});

			ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
		}

		public virtual void TearDown() {
		}


	}
}
