using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.MouseProcess;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class MouseActionTests : Testable {

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
			var action = MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), false);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), false)");
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

			await MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), false);
			testsLaunchingServiceMock.VerifyAll();
		}
	}
}
