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
		public void ToString_Test() {
			var action = new ElementMatchAction(new ElementProgram(0, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, Rect.Empty),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, Rect.Empty),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, Rect.Empty),
			}, 1000);
			var str = action.ToString();
			Assert.IsNotEmpty(str);
		}

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = new ElementMatchAction(new ElementProgram(19, "notepad.exe"), new List<UiElement>() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, new Rect()),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, new Rect(9, 10, 11, 12)),
			}, 1000);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.IsNotEmpty(sourcePresentation);
		}

		[Test]
		public void ExecuteAsScriptSource_When_SearchPath_Is_Inherited_Test() {
			var action = new ElementMatchAction(new ElementProgram(19, "notepad.exe"), new TreeOfSpecificUiElement() {
				new UiElement(4, "value1", "name1", "className1", "automationId1", ControlType.Button.Id, new Rect(1, 2, 3, 4)),
				new UiElement(3, "value2", "name2", "className2", "automationId2", ControlType.CheckBox.Id, new Rect()),
				new UiElement(1, "value3", "name3", "className3", "automationId3", ControlType.ComboBox.Id, new Rect(9, 10, 11, 12)),
			}, 1000);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.IsNotEmpty(sourcePresentation);
		}


	}
}
