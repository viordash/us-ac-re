using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class ElementMatchActionTests {

		[Test]
		public void UsingsForScriptSource_Test() {
			var action = new ElementMatchAction("notepad.exe", new List<UiElement>() {
				new UiElement(0, "value1", "name1", "automationId1", ControlType.Button.Id, Rect.Empty),
				new UiElement(0, "value2", "name2", "automationId2", ControlType.CheckBox.Id, Rect.Empty),
				new UiElement(0, "value3", "name3", "automationId3", ControlType.ComboBox.Id, Rect.Empty),
			}, 1000);
			var source = action.UsingsForScriptSource();
			Assert.That(source, Contains.Item("using System.Collections.Generic;"));
			Assert.That(source, Contains.Item("using System;"));
		}

		[Test]
		public void ToString_Test() {
			var action = new ElementMatchAction("notepad.exe", new List<UiElement>() {
				new UiElement(4, "value1", "name1", "automationId1", ControlType.Button.Id, Rect.Empty),
				new UiElement(3, "value2", "name2", "automationId2", ControlType.CheckBox.Id, Rect.Empty),
				new UiElement(1, "value3", "name3", "automationId3", ControlType.ComboBox.Id, Rect.Empty),
			}, 1000);
			var str = action.ToString();
			Assert.IsNotEmpty(str);
		}

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = new ElementMatchAction("notepad.exe", new List<UiElement>() {
				new UiElement(4, "value1", "name1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "automationId2", ControlType.CheckBox.Id, new Rect()),
				new UiElement(1, "value3", "name3", "automationId3", ControlType.ComboBox.Id, new Rect(9, 10, 11, 12)),
			}, 1000);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.IsNotEmpty(sourcePresentation);
		}


	}
}
