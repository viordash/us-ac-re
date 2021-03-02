using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class ActionSetTests : Testable {

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
			var action = ActionSet.Record("test.scrcs");
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "ActionSet.Play(@\"test.scrcs\")");
		}

		[Test]
		public async Task Play_Test() {
			var cancellationToken = new CancellationToken(false);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			testsLaunchingServiceMock
				.SetupGet(x => x.Examination)
				.Returns(() => {
					return false;
				});

			fileServiceMock
				.Setup(x => x.ReadAllText(It.Is<string>(fn => fn == "\"test.scrcs\"")))
				.Returns(() => {
					return "using System;";
				});

			scriptCompilerMock
				.Setup(x => x.RunTest(It.Is<string>(s => s == "using System;")))
				.Returns(() => {
					return Task.CompletedTask;
				});

			await ActionSet.Play("\"test.scrcs\"");
			fileServiceMock.VerifyAll();
			scriptCompilerMock.VerifyAll();
		}
	}
}
