using System.Windows.Automation;
using NUnit.Framework;
using UsAcRe.Core.Services;
using UsAcRe.Core.Tests.ActionsTests;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Tests.UIAutomationElement {
	[TestFixture]
	public class UiElementTests : Testable {
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		[Test]
		public void Difference_In_Value_Test() {
			var element = new UiElement(-1, "1234", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));
			var otherElement = new UiElement(-1, "12345", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForSimilars(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(6));
			Assert.That(difference.Difference(), Does.Contain("Value").And.Contain("(1234)").And.Contain("(12345)"));
		}

		[Test]
		public void Difference_In_Name_Test() {
			var element = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));
			var otherElement = new UiElement(-1, "123", "Name1", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForSimilars(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(2));
			Assert.That(difference.Difference(), Does.Contain("Name").And.Contain("(Name)").And.Contain("(Name1)"));
		}

		[Test]
		public void Difference_In_ClassName_Test() {
			var element = new UiElement(-1, "123", "Name", "SomeClass1", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));
			var otherElement = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForSimilars(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(3));
			Assert.That(difference.Difference(), Does.Contain("ClassName").And.Contain("(SomeClass1)").And.Contain("(SomeClass)"));
		}

		[Test]
		public void Difference_In_AutomationId_Test() {
			var element = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));
			var otherElement = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId1", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForSimilars(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(4));
			Assert.That(difference.Difference(), Does.Contain("AutomationId").And.Contain("(AutomationId)").And.Contain("(AutomationId1)"));
		}

		[Test]
		public void Difference_In_BoundingRectangle_Location_Test() {
			var element = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4));
			var otherElement = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(11, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForExact(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(5));
			Assert.That(difference.Difference(), Does.Contain("Location").And.Contain("(1,2)").And.Contain("(11,2)"));
		}

		[Test]
		public void Difference_In_BoundingRectangle_Size_Test() {
			var element = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(10, 2, 3, 40));
			var otherElement = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(10, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForExact(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(5));
			Assert.That(difference.Difference(), Does.Contain("Size").And.Contain("(3,40)").And.Contain("(3,4)"));
		}

		[Test]
		public void Difference_In_Few_Fields_Return_Most_Top() {
			var element = new UiElement(-1, "123", "Name", "SomeClass2", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(10, 2, 3, 40));
			var otherElement = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(10, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForExact(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Not.Null);
			Assert.That(difference.Weight, Is.EqualTo(3));
			Assert.That(difference.Difference(), Does.Contain("ClassName").And.Contain("(SomeClass2)").And.Contain("(SomeClass)"));
		}

		[Test]
		public void No_Differences_Return_Null_Test() {
			var element = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(10, 2, 3, 4));
			var otherElement = new UiElement(-1, "123", "Name", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(10, 2, 3, 4));

			var difference = element.Differences(otherElement, ElementCompareParameters.ForExact(), automationElementServiceMock.Object);
			Assert.That(difference, Is.Null);
		}


	}
}
