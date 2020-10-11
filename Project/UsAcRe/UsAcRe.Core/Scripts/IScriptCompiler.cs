using System.Threading.Tasks;

namespace UsAcRe.Core.Scripts {
	public interface IScriptCompiler {
		Task RunTest(string sourceCode);
	}
}
