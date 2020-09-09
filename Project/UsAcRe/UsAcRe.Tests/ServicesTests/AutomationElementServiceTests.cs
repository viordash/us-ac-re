using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using Moq;
using NUnit.Framework;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Tests.ServicesTests {
	[TestFixture]
	public class AutomationElementServiceTests {

		AutomationElementService testable;
		Mock<IWinApiService> winApiServiceMock;
		Mock<ISettingsService> settingsServiceMock;

		[SetUp]
		public virtual void Setup() {
			winApiServiceMock = new Mock<IWinApiService>();
			settingsServiceMock = new Mock<ISettingsService>();
			testable = new AutomationElementService(winApiServiceMock.Object, settingsServiceMock.Object);
		}

		[TearDown]
		public virtual void TearDown() {
		}

		[Test]
		public void GetRootElement_Get_ByIndex_Test() {
			var process1 = Process.Start("notepad.exe");
			Thread.Sleep(100);
			var process2 = Process.Start("notepad.exe");
			Thread.Sleep(100);

			try {
				var elementProgram = new ElementProgram(1, "notepad.exe");
				var rootElement = testable.GetRootElement(elementProgram);
				Assert.NotNull(rootElement);
				var automationElement = rootElement.AutomationElementObj as AutomationElement;
				Assert.NotNull(automationElement);

				var selectedProcess = Process.GetProcessById(automationElement.Current.ProcessId);
				Assert.NotNull(selectedProcess);
				Assert.That(selectedProcess.Id, Is.EqualTo(process2.Id));
			} finally {
				process1.Kill();
				process2.Kill();
			}

		}
	}
}
