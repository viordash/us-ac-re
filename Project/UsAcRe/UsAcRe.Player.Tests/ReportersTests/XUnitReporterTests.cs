using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Tests.ActionsTests;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Player.Reporters;

namespace UsAcRe.Player.Tests.ReportersTests {
	[TestFixture]
	public class XUnitReporterTests : Testable {

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}


		[Test]
		public void Generate_Test() {
			var action = ElementMatchAction.Record(new ElementProgram(0, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, Rect.Empty),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, Rect.Empty),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, Rect.Empty),
			});
			action.FailMessage = "FailMessage";
			var reporter = new XUnitReporter();
			reporter.Success("success0", TimeSpan.FromMilliseconds(100));
			reporter.Success("success1", TimeSpan.FromMilliseconds(101));
			reporter.Success("success2", TimeSpan.FromMilliseconds(102));
			reporter.Fail("fail0", TimeSpan.FromMilliseconds(199), new TestFailedException(action));
			var report = reporter.Generate("suiteName");
			Assert.That(report, Contains.Substring("tests=\"4\""));
			Assert.That(report, Contains.Substring("failures=\"1\""));
			Assert.That(report, Contains.Substring("name=\"success0\""));
		}
	}
}
