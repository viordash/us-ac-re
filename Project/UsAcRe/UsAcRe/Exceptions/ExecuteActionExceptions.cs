using System;
using UsAcRe.Actions;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Exceptions {
	public class ExecuteBaseActionException : Exception {
		public ExecuteBaseActionException(string message) : base(message) { }
		protected ExecuteBaseActionException(BaseAction baseAction, string targetName) : this(BuildMessage(baseAction, targetName)) { }
		protected ExecuteBaseActionException(BaseAction baseAction) : this(BuildMessage(baseAction)) { }

		static string BuildMessage(BaseAction baseAction) {
			return baseAction.ToString();
		}

		static string BuildMessage(BaseAction baseAction, string targetName) {
			return string.Format("{0} ({1})", baseAction.ToString(), targetName);
		}
	}

	public class MinorException : ExecuteBaseActionException {
		public MinorException(MouseAction mouseAction, string targetName) : base(mouseAction, targetName) { }
		public MinorException(BaseAction baseAction) : base(baseAction) { }
	}

	public class SevereException : ExecuteBaseActionException {
		public SevereException(MouseAction mouseAction, string targetName) : base(mouseAction, targetName) { }
		public SevereException(BaseAction baseAction) : base(baseAction) { }
	}

	public class ScriptComposeException : ExecuteBaseActionException {
		public ScriptComposeException() : this("Script composing") { }
		public ScriptComposeException(string message) : base(message) { }
	}

	public class TargetProgramNotFoundExeption : ExecuteBaseActionException {
		public TargetProgramNotFoundExeption(ElementProgram targetProgram) : base(string.Format("Target program {0} not found", targetProgram.ToString())) { }
	}
}
