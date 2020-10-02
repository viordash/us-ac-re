using System.Threading.Tasks;

namespace UsAcRe.Core.Actions {
	public interface ITestAction {
		string FailMessage { get; set; }
		string ExecuteAsScriptSource();
		Task ExecuteAsync();
	}
}
