using System;

namespace UsAcRe.Actions {
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
}
