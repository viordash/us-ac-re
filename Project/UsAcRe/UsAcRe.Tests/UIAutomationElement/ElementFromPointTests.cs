using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Moq;
using NUnit.Framework;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Tests.UIAutomationElement {
	[TestFixture]
	public class ElementFromPointTests {
		Mock<IAutomationElementService> automationElementServiceMock;
		Mock<IWinApiService> winApiServiceMock;

		[SetUp]
		public virtual void SetUp() {
			automationElementServiceMock = new Mock<IAutomationElementService>();
			winApiServiceMock = new Mock<IWinApiService>();
		}

		[TearDown]
		public virtual void TearDown() {
		}

		[Test]
		public void DetermineElementUnderPoint_Test() {
			winApiServiceMock
				.Setup(x => x.GetRootWindowForElementUnderPoint(It.IsAny<WinAPI.POINT>()))
				.Returns(() => {
					return new IntPtr(42);
				});


			automationElementServiceMock
				.Setup(x => x.GetDesktop())
				.Returns(() => {
					return new UiElement(-1, null, "Desktop", "", "", ControlType.Window.Id, new System.Windows.Rect(1, 2, 300, 400)) {
						AutomationElementObj = new object()
					};
				});

			automationElementServiceMock
				.Setup(x => x.FromHandle(It.Is<IntPtr>(h => h == new IntPtr(42))))
				.Returns(() => {
					return new UiElement(-1, null, "Button1", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4)) {
						AutomationElementObj = new object()
					};
				});

			automationElementServiceMock
				.Setup(x => x.FindAllValidElements(It.IsAny<UiElement>(), It.IsAny<TreeScope>()))
				.Returns(() => {
					return new List<UiElement>() {
						new UiElement(-1, null, "Button2", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4)) {
							AutomationElementObj = new object()
						}
					};
				});

			var testable = new ElementFromPoint(automationElementServiceMock.Object, winApiServiceMock.Object, new WinAPI.POINT(100, 200), true);

			winApiServiceMock.VerifyAll();
			automationElementServiceMock.VerifyAll();
		}
	}
}
