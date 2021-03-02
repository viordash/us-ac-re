using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class DelayActionTests : Testable {

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
			var action = DelayAction.Record(19);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "DelayAction.Play(19)");
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
				.SetupGet(x => x.Examination)
				.Returns(() => {
					return true;
				});

			await DelayAction.Play(42);
			testsLaunchingServiceMock.VerifyAll();
		}
	}
}
