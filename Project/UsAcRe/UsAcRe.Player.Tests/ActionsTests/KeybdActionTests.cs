using NUnit.Framework;
using UsAcRe.Core.WindowsSystem;
using UsAcRe.Player.Actions;

namespace UsAcRe.Player.Tests.ActionsTests {
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
			var action = KeybdAction.Record(VirtualKeyCodes.VK_CONTROL, false);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "KeybdAction.Play(VirtualKeyCodes.VK_CONTROL, false)");
		}
	}
}
