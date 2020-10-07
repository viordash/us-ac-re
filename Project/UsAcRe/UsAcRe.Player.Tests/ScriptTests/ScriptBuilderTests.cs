using System.Collections.Generic;
using System.Windows.Automation;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.Tests.ActionsTests;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Core.WindowsSystem;
using UsAcRe.Player.Scripts;

namespace UsAcRe.Player.Tests.ScriptTests {
	[TestFixture]
	public class ScriptBuilderTests : Testable {
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		[Test]
		public void CreateUsingsSection_Test() {
			var elementMatchAction = ElementMatchAction.Record(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new System.Windows.Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, new System.Windows.Rect()),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, new System.Windows.Rect(9, 10, 11, 12)),
			});

			var mouseAction = MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), false);
			var keybdActionDown = KeybdAction.Record(VirtualKeyCodes.K_G, false);
			var keybdActionUp = KeybdAction.Record(VirtualKeyCodes.K_G, true);

			var actions = new ActionsList() {
				elementMatchAction,
				mouseAction,
				keybdActionDown,
				keybdActionUp
			};
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			var usings = scriptBuilder.CreateUsingsSection(actions);
			Assert.IsNotEmpty(usings);
			Assert.That(usings, Does.Contain("using System;"));
			Assert.That(usings, Does.Contain("using System.Collections.Generic;"));
			Assert.That(usings, Does.Contain("using System.Drawing;"));
			Assert.That(usings, Does.Contain("using System.Text;"));
			Assert.That(usings, Does.Contain("using System.Threading.Tasks;"));
			Assert.That(usings, Does.Contain("using System.Windows;"));
			Assert.That(usings, Does.Contain("using UsAcRe.Core.Actions;"));
			Assert.That(usings, Does.Contain("using UsAcRe.Core.MouseProcess;"));
			Assert.That(usings, Does.Contain("using UsAcRe.Core.Services;"));
			Assert.That(usings, Does.Contain("using UsAcRe.Core.UIAutomationElement;"));
			Assert.That(usings, Does.Contain("using UsAcRe.Core.WindowsSystem;"));
		}

		[Test]
		public void CreateNamespaceSection_Test() {
			var actions = new ActionsList() { };
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			var code = scriptBuilder.CreateNamespaceSection("//something");
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith("namespace UsAcRe.TestsScripts {"));
			Assert.That(code, Does.EndWith("}"));
		}

		[Test]
		public void CreateClassSection_Test() {
			var actions = new ActionsList() { };
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			var code = scriptBuilder.CreateClassSection("//something");
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith("\tpublic class TestsScript {"));
			Assert.That(code, Does.EndWith("\t}"));
		}

		[Test]
		public void CreateExecuteMethodSection_Test() {
			var actions = new ActionsList() { };
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			var code = scriptBuilder.CreateExecuteMethodSection("//something");
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith("\t\tpublic async Task ExecuteAsync() {"));
			Assert.That(code, Does.EndWith("\t\t}"));
		}

		[Test]
		public void CreateExecuteMethodBody_Test() {
			var elementMatchAction = ElementMatchAction.Record(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "", "Calculator", "", "automationId1", ControlType.Table.Id, new System.Windows.Rect(10, 20, 30, 40)),
				new UiElement(3, "value2", "7", "137", "automationId2", ControlType.CheckBox.Id, new System.Windows.Rect(11, 22, 33, 44)),
			});

			var mouseAction = MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), false);
			var keybdActionDown = KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_1, false);
			var keybdActionUp = KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_1, true);

			var actions = new ActionsList() {
				elementMatchAction,
				mouseAction,
				keybdActionDown,
				keybdActionUp
			};
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			var code = scriptBuilder.CreateExecuteMethodBody(actions);
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.StartWith(
				"\t\t\tawait ElementMatchAction.Play(new ElementProgram(42, \"notepad.exe\"), new List<UiElement>() {\r\n"
				+ "\t\t\t\tnew UiElement(4, \"\", \"Calculator\", \"\", \"automationId1\", 50036, new System.Windows.Rect(10, 20, 30, 40)),\r\n"
				+ "\t\t\t\tnew UiElement(3,"));

			Assert.That(code, Does.Contain("await MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), false)"));
			Assert.That(code, Does.Contain("await KeybdAction.Play(VirtualKeyCodes.K_1, false)"));
			Assert.That(code, Does.Contain("await KeybdAction.Play(VirtualKeyCodes.K_1, true);"));
		}

		[Test]
		public void Build_Test() {
			var mouseAction = MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), true);
			var keybdActionDown = KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_1, false);
			var elementMatchAction = ElementMatchAction.Record(new ElementProgram(42, "notepad.exe"), new List<UiElement>() {
				new UiElement(0, "", "Decimal", "Button", "314", 50013, new System.Windows.Rect(2017, 289, 59, 15)),
				new UiElement(0, "", "", "CalcFrame", "", 50033, new System.Windows.Rect(1998, 130, 407, 330)),
				new UiElement(0, "", "Calculator", "CalcFrame", "", 50032, new System.Windows.Rect(1990, 79, 423, 389)),
			});
			elementMatchAction.TimeoutMs = 10000;

			var actions = new ActionsList() {
				mouseAction,
				keybdActionDown,
				elementMatchAction
			};
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			var code = scriptBuilder.Generate(actions);
			Assert.IsNotEmpty(code);
			Assert.That(code, Does.Contain("using System.Drawing;"));
			Assert.That(code, Does.Contain("namespace UsAcRe.TestsScripts {"));
			Assert.That(code, Does.Contain("await MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), true);"));
			Assert.That(code, Does.Contain("}, 10000);"));
		}

		[Test]
		public void CombineTextTypingActions_Test() {
			var actions = new ActionsList() {
				MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), true),
				MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.VK_RSHIFT, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.VK_RSHIFT, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_S, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_S, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, true),
				MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.VK_RSHIFT, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_H, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_H, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.VK_RSHIFT, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_O, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_O, true),
				MouseClickAction.Record(MouseButtonType.Right, new System.Drawing.Point(1, 2), false),
			};
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			scriptBuilder.CombineTextTypingActions(actions);

			Assert.That(actions.Count, Is.EqualTo(14));
			Assert.IsInstanceOf<TextTypingAction>(actions[6]);
			Assert.That((actions[6] as TextTypingAction).Text, Is.EqualTo("est"));

			Assert.IsInstanceOf<TextTypingAction>(actions[12]);
			Assert.That((actions[12] as TextTypingAction).Text, Is.EqualTo("ello"));
		}

		[Test]
		public void CombineTextTypingActions_With_Error_In_Seq_Test() {
			var actions = new ActionsList() {
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_S, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_S, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_T, true),
				MouseClickAction.Record(MouseButtonType.Right, new System.Drawing.Point(1, 2), false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_H, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_H, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_E, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_L, true),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_O, false),
				//KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_O, true),
			};
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			scriptBuilder.CombineTextTypingActions(actions);

			Assert.That(actions.Count, Is.EqualTo(11));
			Assert.IsInstanceOf<TextTypingAction>(actions[0]);
			Assert.That((actions[0] as TextTypingAction).Text, Is.EqualTo("test"));
		}

		[Test]
		public void CombineTextTypingActions_Seq_For_One_Key_Is_Skipped_Test() {
			var actions = new ActionsList() {
				MouseClickAction.Record(MouseButtonType.Right, new System.Drawing.Point(1, 2), false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_H, false),
				KeybdAction.Record(Core.WindowsSystem.VirtualKeyCodes.K_H, true),
			};
			var scriptBuilder = new ScriptBuilder(settingsServiceMock.Object);
			scriptBuilder.CombineTextTypingActions(actions);

			Assert.That(actions.Count, Is.EqualTo(3));
		}
	}
}
