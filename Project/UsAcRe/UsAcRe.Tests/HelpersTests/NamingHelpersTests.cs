using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework;
using UsAcRe.Helpers;
using UsAcRe.MouseProcess;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Tests.MouseProcessTests {
	[TestFixture]
	public class NamingHelpersTests {

		[Test]
		public void Escape_Test() {
			var escaped = NamingHelpers.Escape("\t\tHello world \r\n \"test\"", int.MaxValue);
			Assert.That(escaped, Is.EqualTo("\\t\\tHello world \\r\\n \\\"test\\\""));
		}
	}
}
