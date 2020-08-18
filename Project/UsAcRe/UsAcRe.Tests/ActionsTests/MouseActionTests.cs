using NUnit.Framework;
using UsAcRe.Actions;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class MouseActionTests {

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = new MouseAction(MouseProcess.MouseActionType.LeftClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4));
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "new MouseAction(MouseActionType.LeftClick, new System.Drawing.Point(1, 2), new System.Drawing.Point(3, 4)).Execute()");
		}
	}
}
