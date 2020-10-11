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
			Assert.AreEqual(sourcePresentation, "ActionSet.Play(\"test.scrcs\")");
		}
	}
}
