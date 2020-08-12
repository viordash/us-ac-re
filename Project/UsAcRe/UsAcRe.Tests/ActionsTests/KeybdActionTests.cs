using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class KeybdActionTests {

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = new KeybdAction(VirtualKeyCodes.VK_CONTROL, false);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "new KeybdAction(VirtualKeyCodes.VK_CONTROL, false).Execute()");
		}
	}
}
