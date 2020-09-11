using NUnit.Framework;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.Tests.MouseProcessTests {
	[TestFixture]
	public class NamingHelpersTests {

		[Test]
		public void Escape_Test() {
			var escaped = NamingHelpers.Escape("\t\tHello world \r\n \"test\"", int.MaxValue);
			Assert.That(escaped, Is.EqualTo("\\t\\tHello world \\r\\n \\\"test\\\""));
		}
	}
}
