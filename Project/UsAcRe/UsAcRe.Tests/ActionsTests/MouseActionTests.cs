using NUnit.Framework;
using UsAcRe.Actions;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class MouseActionTests : BaseActionTestable {

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
			var action = new MouseAction(MouseProcess.MouseActionType.LeftClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "await new MouseAction(MouseActionType.LeftClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4)).ExecuteAsync()");
		}

		[Test]
		public void ExecuteAsScriptSource_ForSinglePointClick_Test() {
			var action = new MouseAction(MouseProcess.MouseActionType.LeftClick, new System.Drawing.Point(1, 2));
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "await new MouseAction(MouseActionType.LeftClick, new System.Drawing.Point(1, 2)).ExecuteAsync()");
		}
	}
}
