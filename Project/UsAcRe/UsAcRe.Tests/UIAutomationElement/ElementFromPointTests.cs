using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Moq;
using NUnit.Framework;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using static UsAcRe.UIAutomationElement.ElementFromPoint;

namespace UsAcRe.Tests.UIAutomationElement {
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

			public TreeElement PublicSortElementsByPointProximity(List<TreeElement> elements) {
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


		[Test]
		public void SortingElements_For_Similar_ZOrder_Test() {
			var element0 = new UiElement(1, null, "Button0", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element1 = new UiElement(1, null, "Button1", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 55, 100));
			var element2 = new UiElement(1, null, "Button2", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 54));
			var element3 = new UiElement(10, null, "Button3", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element4 = new UiElement(10, null, "Button4", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element5 = new UiElement(20, null, "Button5", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element6 = new UiElement(20, null, "Button6", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 100, 100));
			var element7 = new UiElement(1, null, "Button7", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect(0, 0, 10, 10));

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
			var elements = new List<TreeElement>();

			var tree = new Dictionary<UiElement, List<UiElement>>();
			elements.Add(new TreeElement(element0, new List<UiElement>() {
				new UiElement(-1, null, "Panel0", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel00", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element1, new List<UiElement>() {
				new UiElement(-1, null, "Panel1", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel10", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element2, new List<UiElement>() {
				new UiElement(-1, null, "Panel2", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel20", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element3, new List<UiElement>() {
				new UiElement(-1, null, "Panel3", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel30", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element4, new List<UiElement>() {
				new UiElement(-1, null, "Panel4", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel40", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element5, new List<UiElement>() {
				new UiElement(-1, null, "Panel5", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel50", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element6, new List<UiElement>() {
				new UiElement(-1, null, "Panel6", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel60", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect())
			}));
			elements.Add(new TreeElement(element7, new List<UiElement>() {
				new UiElement(-1, null, "Panel7", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel70", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel700", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
				new UiElement(-1, null, "Panel7000", "SomeClass", "AutomationId", ControlType.Tree.Id, new System.Windows.Rect()),
			}));

			var sortedElements = testable.PublicSortElementsByPointProximity(elements);
			Assert.That(sortedElements.Element.Name, Is.EqualTo("Button2"));
		}
	}
}
