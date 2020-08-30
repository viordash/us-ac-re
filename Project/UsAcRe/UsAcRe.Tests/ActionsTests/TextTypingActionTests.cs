using NUnit.Framework;
using UsAcRe.Actions;

namespace UsAcRe.Tests.ActionsTests {
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
			var action = new TextTypingAction("\t\tHello world \r\n \"test\"");
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "TextType(\"\\t\\tHello world \\r\\n \\\"test\\\"\")");
		}
	}
}
