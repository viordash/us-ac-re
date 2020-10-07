using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.MouseProcess;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class MouseDragActionTests : BaseActionTestable {

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
	}
}
