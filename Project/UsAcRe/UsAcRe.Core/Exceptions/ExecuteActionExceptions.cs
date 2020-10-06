using System;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Exceptions {
	public class ExecuteBaseActionException : Exception {
		public ExecuteBaseActionException(string message) : base(message) { }
		protected ExecuteBaseActionException(BaseAction baseAction, string targetName) : this(BuildMessage(baseAction, targetName)) { }
		protected ExecuteBaseActionException(BaseAction baseAction) : this(BuildMessage(baseAction)) { }

		static string BuildMessage(BaseAction baseAction) {
			return string.Format("{0} \r\n {1}", baseAction.FailMessage, baseAction.ToString());
		}

		static string BuildMessage(BaseAction baseAction, string targetName) {
			return string.Format("{0} \r\n {1} ({2})", baseAction.FailMessage, baseAction.ToString(), targetName);
		}
	}

	public class MinorException : ExecuteBaseActionException {
		public MinorException(BaseAction baseAction, string targetName) : base(baseAction, targetName) { }
		public MinorException(BaseAction baseAction) : base(baseAction) { }
	}

	public class SevereException : ExecuteBaseActionException {
		public SevereException(BaseAction baseAction, string targetName) : base(baseAction, targetName) { }
		public SevereException(BaseAction baseAction) : base(baseAction) { }
	}

	public class ScriptComposeException : ExecuteBaseActionException {
		public ScriptComposeException() : this("Script composing") { }
		public ScriptComposeException(string message) : base(message) { }
	}

	public class TargetProgramNotFoundExeption : ExecuteBaseActionException {
		public TargetProgramNotFoundExeption(ElementProgram targetProgram) : base(string.Format("Target program {0} not found", targetProgram.ToString())) { }
	}

	public class TestFailedExeption : ExecuteBaseActionException {
		public TestFailedExeption(BaseAction baseAction) : base(baseAction) { }
	}
}
