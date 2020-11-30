using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class ElementMatchActionTests : Testable {

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}


		[Test]
		public void ToString_Test() {
			var action = ElementMatchAction.Record(new ElementProgram(0, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, Rect.Empty),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, Rect.Empty),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, Rect.Empty),
			});
			var str = action.ToString();
			Assert.IsNotEmpty(str);
		}

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = ElementMatchAction.Record(new ElementProgram(19, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, new Rect()),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, new Rect(9, 10, 11, 12)),
			});
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.IsNotEmpty(sourcePresentation);
		}

		[Test]
		public void ExecuteAsScriptSource_When_SearchPath_Is_Inherited_Test() {
			var action = ElementMatchAction.Record(new ElementProgram(19, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, new Rect()),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, new Rect(9, 10, 11, 12)),
			});
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.IsNotEmpty(sourcePresentation);
		}

		[Test]
		public void ExecuteAsync_Return_By_Timeout_Test() {
			var cancellationToken = new CancellationToken(false);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			var task = ElementMatchAction.Play(new ElementProgram(19, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
			}, 100);

			var stopwatch = Stopwatch.StartNew();
			Assert.ThrowsAsync<TestFailedException>(async () => await task);
			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.GreaterThan(100));
		}

		[Test]
		public void ExecuteAsync_CancellationToken_Test() {
			var cancelTokenSource = new CancellationTokenSource();
			var cancellationToken = cancelTokenSource.Token;

			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			var task = ElementMatchAction.Play(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
			}, 500);

			var stopwatch = Stopwatch.StartNew();

			Parallel.Invoke(() => {
				Thread.Sleep(150);
				cancelTokenSource.Cancel();
			},
			async () => await task
			);

			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.LessThan(500 - 1));
		}

		[Test]
		public async Task Execute_With_DryMode_Test() {
			var cancellationToken = new CancellationToken(false);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			testsLaunchingServiceMock
				.SetupGet(x => x.IsDryRunMode)
				.Returns(() => {
					return true;
				});

			var stopwatch = Stopwatch.StartNew();
			await ElementMatchAction.Play(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
			}, 1000);


			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.LessThan(1000 - 1));
			testsLaunchingServiceMock.VerifyAll();
		}

	}
}
