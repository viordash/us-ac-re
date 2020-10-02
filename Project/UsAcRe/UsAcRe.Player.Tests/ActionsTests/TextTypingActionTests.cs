using NUnit.Framework;
using UsAcRe.Player.Actions;

namespace UsAcRe.Player.Tests.ActionsTests {
	[TestFixture]
	public class TextTypingActionTests : BaseActionTestable {

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
	}
}
