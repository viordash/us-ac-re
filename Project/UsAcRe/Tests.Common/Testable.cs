using System.Threading;
using CommonServiceLocator;
using Moq;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Tests.ActionsTests {
	public class Testable {
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

			serviceLocatorMock
				.Setup(x => x.GetInstance<KeybdAction>())
				.Returns(() => {
					return new KeybdAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object);
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<MouseClickAction>())
				.Returns(() => {
					return new MouseClickAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object);
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<MouseDragAction>())
				.Returns(() => {
					return new MouseDragAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object);
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<TextTypingAction>())
				.Returns(() => {
					return new TextTypingAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object);
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<ElementMatchAction>())
				.Returns(() => {
					return new ElementMatchAction(automationElementServiceMock.Object, settingsServiceMock.Object, testsLaunchingServiceMock.Object);
				});

			serviceLocatorMock
				.Setup(x => x.GetInstance<DelayAction>())
				.Returns(() => {
					return new DelayAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object);
				});

			ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
		}

		public virtual void TearDown() {
		}


	}
}
