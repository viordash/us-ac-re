using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Scripts {
	public interface IScriptBuilder {
		string Generate(ActionsList actions);
	}
}
