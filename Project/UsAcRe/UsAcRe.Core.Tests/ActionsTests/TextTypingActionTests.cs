using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class TextTypingActionTests : Testable {

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
			var action = TextTypingAction.Record("\t\tHello world \r\n \"test\"");
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "TextTypingAction.Play(\"\\t\\tHello world \\r\\n \\\"test\\\"\")");
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

			await TextTypingAction.Play("1234");
			testsLaunchingServiceMock.VerifyAll();
		}
	}
}
