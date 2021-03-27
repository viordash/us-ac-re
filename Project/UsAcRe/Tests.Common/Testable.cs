using System;
using System.Threading;
using Moq;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Tests.ActionsTests {
	public class Testable {
		protected Mock<IServiceProvider> serviceProviderMock;
		protected Mock<IAutomationElementService> automationElementServiceMock;
		protected Mock<IWinApiService> winApiServiceMock;
		protected Mock<ITestsLaunchingService> testsLaunchingServiceMock;
		protected Mock<ISettingsService> settingsServiceMock;
		protected Mock<IFileService> fileServiceMock;
		protected Mock<IScriptBuilder> scriptBuilderMock;
		protected Mock<IScriptCompiler> scriptCompilerMock;

		public virtual void Setup() {
			serviceProviderMock = new Mock<IServiceProvider>();
			automationElementServiceMock = new Mock<IAutomationElementService>();
			winApiServiceMock = new Mock<IWinApiService>();
			testsLaunchingServiceMock = new Mock<ITestsLaunchingService>();
			settingsServiceMock = new Mock<ISettingsService>();
			fileServiceMock = new Mock<IFileService>();
			scriptBuilderMock = new Mock<IScriptBuilder>();
			scriptCompilerMock = new Mock<IScriptCompiler>();

			serviceProviderMock
				.Setup(x => x.GetService(typeof(IAutomationElementService)))
				.Returns(() => {
					return automationElementServiceMock.Object;
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(IWinApiService)))
				.Returns(() => {
					return winApiServiceMock.Object;
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(ITestsLaunchingService)))
				.Returns(() => {
					return testsLaunchingServiceMock.Object;
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(ISettingsService)))
				.Returns(() => {
					return settingsServiceMock.Object;
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(IFileService)))
				.Returns(() => {
					return fileServiceMock.Object;
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(IScriptBuilder)))
				.Returns(() => {
					return scriptBuilderMock.Object;
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(IScriptCompiler)))
				.Returns(() => {
					return scriptCompilerMock.Object;
				});


			testsLaunchingServiceMock
				.SetupGet(x => x.CurrentCancellationToken)
				.Returns(() => {
					return new CancellationToken(true);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(KeybdAction)))
				.Returns(() => {
					return new KeybdAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(MouseClickAction)))
				.Returns(() => {
					return new MouseClickAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(MouseDragAction)))
				.Returns(() => {
					return new MouseDragAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(TextTypingAction)))
				.Returns(() => {
					return new TextTypingAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(ElementMatchAction)))
				.Returns(() => {
					return new ElementMatchAction(automationElementServiceMock.Object, settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(DelayAction)))
				.Returns(() => {
					return new DelayAction(settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			serviceProviderMock
				.Setup(x => x.GetService(typeof(ActionSet)))
				.Returns(() => {
					return new ActionSet(scriptCompilerMock.Object, settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object);
				});

			ServiceLocator.SetLocatorProvider(serviceProviderMock.Object);
		}

		public virtual void TearDown() {
		}


	}
}
