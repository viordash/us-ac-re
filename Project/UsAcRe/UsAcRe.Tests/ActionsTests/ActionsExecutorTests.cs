using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

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
				} else if(baseAction is MouseAction mouseAction) {
					executedActions.Add(nameof(MouseAction) + mouseAction.ActionType + mouseAction.DownClickedPoint.X);
				} else if(baseAction is KeybdAction keybdAction) {
					executedActions.Add(nameof(KeybdAction) + keybdAction.VKCode);
				}
			});

			await ActionsExecutor
				.Perform
				.ElementMatching(new ElementProgram(0, "Progr0"), new List<UiElement>())
				.ElementMatching(new ElementProgram(1, "Progr1"), new List<UiElement>())
				.Mouse(MouseActionType.LeftClick, new System.Drawing.Point(0, 0))
				.ElementMatching(new ElementProgram(2, "Progr2"), new List<UiElement>())
				.Mouse(MouseActionType.LeftClick, new System.Drawing.Point(1, 1), new System.Drawing.Point(0, 0))
				.Mouse(MouseActionType.RightDrag, new System.Drawing.Point(2, 2))
				.Mouse(MouseActionType.LeftClick, new System.Drawing.Point(3, 3), new System.Drawing.Point(0, 0))
				.ElementMatching(new ElementProgram(3, "Progr3"), new List<UiElement>())
				.Mouse(MouseActionType.RightClick, new System.Drawing.Point(4, 4))
				.Mouse(MouseActionType.LeftClick, new System.Drawing.Point(5, 5), new System.Drawing.Point(0, 0))
				.Keyboard(VirtualKeyCodes.K_H, false)
				.Mouse(MouseActionType.MiddleDoubleClick, new System.Drawing.Point(6, 6));

			Assert.That(executedActions, Is.EquivalentTo(new string[] {
				"ElementMatchActionProgr0",
				"ElementMatchActionProgr1",
				"MouseActionLeftClick0",
				"ElementMatchActionProgr2",
				"MouseActionLeftClick1",
				"MouseActionRightDrag2",
				"MouseActionLeftClick3",
				"ElementMatchActionProgr3",
				"MouseActionRightClick4",
				"MouseActionLeftClick5",
				"KeybdActionK_H",
				"MouseActionMiddleDoubleClick6"
			}));
		}
	}
}
