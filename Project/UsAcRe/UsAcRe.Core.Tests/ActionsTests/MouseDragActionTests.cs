using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.MouseProcess;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class MouseDragActionTests : Testable {

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = MouseDragAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "MouseDragAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4))");
		}

		[Test]
		public async Task Execute_With_DryMode_Test() {
			var cancellationToken = new CancellationToken(false);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			testsLaunchingServiceMock
				.SetupGet(x => x.IsDryRunMode)
				.Returns(() => {
					return true;
				});

			await MouseDragAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));
			testsLaunchingServiceMock.VerifyAll();
		}
	}
}
