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
			var keybdActionDown = new KeybdAction(WindowsSystem.VirtualKeyCodes.K_G, false);
			var keybdActionUp = new KeybdAction(WindowsSystem.VirtualKeyCodes.K_G, true);

			var actions = new ActionsList() {
				elementMatchAction,
				mouseAction,
				keybdActionDown,
				keybdActionUp
			};
			var scriptBuilder = new ScriptBuilder(actions);
			var usings = scriptBuilder.CreateUsingsSection();
			Assert.IsNotEmpty(usings);
			Assert.That(usings, Does.Contain("using System;"));
			Assert.That(usings, Does.Contain("using System.Collections.Generic;"));
			Assert.That(usings, Does.Contain("using System.Drawing;"));
			Assert.That(usings, Does.Contain("using UsAcRe.MouseProcess;"));
			Assert.That(usings, Does.Contain("using UsAcRe.UIAutomationElement;"));
			Assert.That(usings, Does.Contain("using UsAcRe.WindowsSystem;"));
		}

		[Test]
		public void CreateNamespaceSection_Test() {
			var actions = new ActionsList() { };
			var scriptBuilder = new ScriptBuilder(actions);
			var code = scriptBuilder.CreateNamespaceSection("//something");
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith("namespace UsAcRe.TestsScripts {"));
			Assert.That(code, Does.EndWith("}"));
		}

		[Test]
		public void CreateClassSection_Test() {
			var actions = new ActionsList() { };
			var scriptBuilder = new ScriptBuilder(actions);
			var code = scriptBuilder.CreateClassSection("//something");
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith("\tpublic class TestsScript {"));
			Assert.That(code, Does.EndWith("\t}"));
		}

		[Test]
		public void CreateExecuteMethodSection_Test() {
			var actions = new ActionsList() { };
			var scriptBuilder = new ScriptBuilder(actions);
			var code = scriptBuilder.CreateExecuteMethodSection("//something");
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith("\t\tpublic void Execute() {"));
			Assert.That(code, Does.EndWith("\t\t}"));
		}

		[Test]
		public void CreateExecuteMethodBody_Test() {
			var elementMatchAction = new ElementMatchAction(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "", "Calculator", "", "automationId1", ControlType.Table.Id, new Rect(10, 20, 30, 40)),
				new UiElement(3, "value2", "7", "137", "automationId2", ControlType.CheckBox.Id, new Rect(11, 22, 33, 44)),
			}, 1000);

			var mouseAction = new MouseAction(MouseProcess.MouseActionType.LeftClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));
			var keybdActionDown = new KeybdAction(WindowsSystem.VirtualKeyCodes.K_1, false);
			var keybdActionUp = new KeybdAction(WindowsSystem.VirtualKeyCodes.K_1, true);

			var actions = new ActionsList() {
				elementMatchAction,
				mouseAction,
				keybdActionDown,
				keybdActionUp
			};
			var scriptBuilder = new ScriptBuilder(actions);
			var code = scriptBuilder.CreateExecuteMethodBody();
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith(
				"\t\t\tnew ElementMatchAction(new ElementProgram(42, \"notepad.exe\"), new List<UiElement>() {\r\n"
				+ "\t\t\t\tnew UiElement(4, \"\", \"Calculator\", \"\", \"automationId1\", 50036, new Rect(10, 20, 30, 40)),\r\n"
				+ "\t\t\t\tnew UiElement(3, \"value2\", \"7\", \"137\", \"automationId2\", 50002, new Rect(11, 22, 33, 44)),\r\n"
				+ "\t\t\t}, 1000).Execute();"));

			Assert.That(code, Does.Contain("new MouseAction(MouseActionType.LeftClick, new Point(1, 2), new Point(3, 4)).Execute();"));
			Assert.That(code, Does.Contain("new KeybdAction(VirtualKeyCodes.K_1, false).Execute();"));
			Assert.That(code, Does.Contain("new KeybdAction(VirtualKeyCodes.K_1, true).Execute();"));
		}

		[Test]
		public void Build_Test() {
			var mouseAction = new MouseAction(MouseProcess.MouseActionType.LeftDoubleClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));
			var keybdActionDown = new KeybdAction(WindowsSystem.VirtualKeyCodes.K_1, false);

			var actions = new ActionsList() {
				mouseAction,
				keybdActionDown,
			};
			var scriptBuilder = new ScriptBuilder(actions);
			var code = scriptBuilder.Generate();
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.Contain("using System.Drawing;"));
			Assert.That(code, Does.Contain("namespace UsAcRe.TestsScripts {"));
			Assert.That(code, Does.Contain("new MouseAction(MouseActionType.LeftDoubleClick, new Point(1, 2), new Point(3, 4)).Execute();"));
		}
	}
}
