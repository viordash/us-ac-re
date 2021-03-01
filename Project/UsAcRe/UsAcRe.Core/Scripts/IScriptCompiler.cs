using System.Threading.Tasks;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Scripts {
	public interface IScriptCompiler {
		Task RunTest(string sourceCode);
	}
}
