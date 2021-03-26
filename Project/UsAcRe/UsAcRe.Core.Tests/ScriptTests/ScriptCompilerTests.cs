using System.IO;
using System.Linq;
using NUnit.Framework;
using UsAcRe.Core.Scripts;

namespace UsAcRe.Core.Tests.ScriptTests {
	[TestFixture]
	public class ScriptCompilerTests {

		[Test]
		public void GetMandatoryAssemblies_Test() {
			var assembliesNames = ScriptCompiler.GetMandatoryAssemblies()
				.Select(x => Path.GetFileNameWithoutExtension(x));

			Assert.That(assembliesNames, Does.Contain("System.Windows.Forms"));
			Assert.That(assembliesNames, Does.Contain("System.Drawing.Primitives"));
			Assert.That(assembliesNames, Does.Contain("UsAcRe.Core"));
		}

	}
}
