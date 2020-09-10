using NUnit.Framework;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Tests.ExtensionsTests {
	[TestFixture]
	public class VirtualKeyCodesExtensionsTests {

		[Test]
		public void IsPrintable_Test() {
			Assert.IsTrue(VirtualKeyCodes.K_3.IsPrintable());
			Assert.IsTrue(VirtualKeyCodes.K_Z.IsPrintable());
			Assert.IsFalse(VirtualKeyCodes.VK_RETURN.IsPrintable());
			Assert.IsFalse(VirtualKeyCodes.VK_SHIFT.IsPrintable());
			Assert.IsFalse(VirtualKeyCodes.VK_F10.IsPrintable());
		}

		[Test]
		public void TryGetKeyValue_Test() {
			char value;
			Assert.IsTrue(VirtualKeyCodes.K_3.TryGetKeyValue(out value));
			Assert.That(value, Is.EqualTo('3'));
			Assert.IsTrue(VirtualKeyCodes.K_A.TryGetKeyValue(out value));
			Assert.That(value, Is.EqualTo('a'));
			Assert.IsFalse(VirtualKeyCodes.VK_BACK.TryGetKeyValue(out value));
		}
	}
}
