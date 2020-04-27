using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.Scripts;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Tests.ScriptTests {
	[TestFixture]
	public class ScriptBuilderTests {
		[Test]
		public void CreateUsingsSection_Test() {
			var elementMatchAction = new ElementMatchAction(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, new Rect()),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, new Rect(9, 10, 11, 12)),
			}, 1000);

			var mouseAction = new MouseAction(MouseProcess.MouseActionType.LeftClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));

			var actions = new ActionsList() {
				elementMatchAction,
				mouseAction
			};
			var scriptBuilder = new ScriptBuilder(actions);
			var usings = scriptBuilder.CreateUsingsSection();
			Assert.IsNotEmpty(usings);
			Assert.That(usings, Does.Contain("using System;"));
			Assert.That(usings, Does.Contain("using System.Collections.Generic;"));
			Assert.That(usings, Does.Contain("using System.Drawing;"));
			Assert.That(usings, Does.Contain("using UsAcRe.MouseProcess;"));
			Assert.That(usings, Does.Contain("using UsAcRe.UIAutomationElement;"));
		}
	}
}
