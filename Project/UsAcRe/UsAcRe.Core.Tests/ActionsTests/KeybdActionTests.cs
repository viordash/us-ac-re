using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class KeybdActionTests : BaseActionTestable {

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
			var action = new KeybdAction(VirtualKeyCodes.VK_CONTROL, false);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "Keyboard(VirtualKeyCodes.VK_CONTROL, false)");
		}
	}
}
