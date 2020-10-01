using UsAcRe.Player.Actions;

namespace UsAcRe.Player.Scripts {
	public interface IScriptBuilder {
		string Generate(ActionsList actions);
	}
}
