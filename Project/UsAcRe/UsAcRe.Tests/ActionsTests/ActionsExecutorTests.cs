using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.Core.WindowsSystem;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class ActionsExecutorTests : BaseActionTestable {
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		[Test]
		public async Task Execution_Order_Test() {
			var executedActions = new List<string>();

			var cancellationToken = new CancellationToken(true);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			testsLaunchingServiceMock
			.Setup(x => x.Log(It.IsAny<BaseAction>()))
			.Callback<BaseAction>((baseAction) => {
				if(baseAction is ElementMatchAction elementMatchAction) {
					executedActions.Add(nameof(ElementMatchAction) + elementMatchAction.Program.FileName);
				} else if(baseAction is MouseClickAction mouseClickAction) {
					executedActions.Add(nameof(MouseClickAction) + mouseClickAction.Button + mouseClickAction.ClickedPoint.X);
				} else if(baseAction is MouseDragAction mouseDragAction) {
					executedActions.Add(nameof(MouseDragAction) + mouseDragAction.Button + mouseDragAction.StartCoord.X);
				} else if(baseAction is KeybdAction keybdAction) {
					executedActions.Add(nameof(KeybdAction) + keybdAction.VKCode);
				}
			});

			await ActionsExecutor
				.Perform
				.ElementMatching(new ElementProgram(0, "Progr0"), new List<UiElement>())
				.ElementMatching(new ElementProgram(1, "Progr1"), new List<UiElement>())
				.MouseClick(MouseButtonType.Left, new System.Drawing.Point(0, 0), false)
				.ElementMatching(new ElementProgram(2, "Progr2"), new List<UiElement>())
				.MouseClick(MouseButtonType.Left, new System.Drawing.Point(1, 1), false)
				.MouseDrag(MouseButtonType.Right, new System.Drawing.Point(2, 2), new System.Drawing.Point(5, 5))
				.MouseClick(MouseButtonType.Left, new System.Drawing.Point(3, 3), false)
				.ElementMatching(new ElementProgram(3, "Progr3"), new List<UiElement>())
				.MouseClick(MouseButtonType.Right, new System.Drawing.Point(4, 4), false)
				.MouseClick(MouseButtonType.Left, new System.Drawing.Point(5, 5), false)
				.Keyboard(VirtualKeyCodes.K_H, false)
				.MouseClick(MouseButtonType.Middle, new System.Drawing.Point(6, 6), true);

			Assert.That(executedActions, Is.EquivalentTo(new string[] {
				"ElementMatchActionProgr0",
				"ElementMatchActionProgr1",
				"MouseClickActionLeft0",
				"ElementMatchActionProgr2",
				"MouseClickActionLeft1",
				"MouseDragActionRight2",
				"MouseClickActionLeft3",
				"ElementMatchActionProgr3",
				"MouseClickActionRight4",
				"MouseClickActionLeft5",
				"KeybdActionK_H",
				"MouseClickActionMiddle6"
			}));
		}
	}
}
