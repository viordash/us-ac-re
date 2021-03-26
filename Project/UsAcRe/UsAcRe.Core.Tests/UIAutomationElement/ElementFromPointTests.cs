using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Moq;
using NUnit.Framework;
using UsAcRe.Core.Services;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Core.WindowsSystem;
using static UsAcRe.Core.UIAutomationElement.ElementFromPoint;

namespace UsAcRe.Core.Tests.UIAutomationElement {
	[TestFixture]
	public class ElementFromPointTests {
		#region inner classes
		class TestableElementFromPoint : ElementFromPoint {
			List<UiElement> predefinedElements = null;

			public TestableElementFromPoint(
			IAutomationElementService automationElementService,
			IWinApiService winApiService,
			WinAPI.POINT elementCoord,
			bool detailedRetrieve,
			List<UiElement> predefinedElements) : base(automationElementService, winApiService, elementCoord, detailedRetrieve) {
				this.predefinedElements = predefinedElements;
			}

			public UiElement PublicSortElementsByPointProximity(List<ElementFromPoint.TreeItem> elements) {
				return SortElementsByPointProximity(elements, IntPtr.Zero);
			}

			protected override int GetZOrder(UiElement element, IntPtr rootWindow) {
				return element.Index;
			}

		}
		#endregion

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
				.Returns<UiElement, TreeScope>((e, s) => {
					switch(e.Name.Value) {
						case "Desktop":
							return new List<UiElement>() {
								new UiElement(-1, null, "Button2", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(1, 2, 3, 4)) {
									AutomationElementObj = new object()
								}
							};
						default:
							return new List<UiElement>();
					}
				});

			var testable = new ElementFromPoint(automationElementServiceMock.Object, winApiServiceMock.Object, new WinAPI.POINT(100, 200), true);

			winApiServiceMock.VerifyAll();
			automationElementServiceMock.VerifyAll();
		}


		[Test]
		public void SortingElements_For_Similar_ZOrder_Use_BoundingRectangle_For_Sort_Test() {
			var element0 = new UiElement(1, null, "Button0", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element1 = new UiElement(1, null, "Button1", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 55, 100));
			var element2 = new UiElement(1, null, "Button2", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 54));
			var element3 = new UiElement(10, null, "Button3", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element4 = new UiElement(10, null, "Button4", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element5 = new UiElement(20, null, "Button5", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element6 = new UiElement(20, null, "Button6", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element7 = new UiElement(1, null, "Button7", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 90, 90));

			var testable = new TestableElementFromPoint(automationElementServiceMock.Object, winApiServiceMock.Object, new WinAPI.POINT(100, 200), true,
				new List<UiElement>() {
					element0,
					element1,
					element2,
					element3,
					element4,
					element5,
					element6,
					element7
				});
			var elements = new List<ElementFromPoint.TreeItem>();

			elements.Add(new ElementFromPoint.TreeItem(element0, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element1, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element2, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element3, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element4, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element5, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element6, null, new List<UiElement>()));
			elements.Add(new ElementFromPoint.TreeItem(element7, null, new List<UiElement>()));

			var sortedElements = testable.PublicSortElementsByPointProximity(elements);
			Assert.That(sortedElements.Name.Value, Is.EqualTo("Button2"));
		}
	}
}
