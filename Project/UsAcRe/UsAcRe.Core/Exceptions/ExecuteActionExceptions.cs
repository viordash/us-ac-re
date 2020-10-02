using System;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Exceptions {
	public class ExecuteBaseActionException : Exception {
		public ExecuteBaseActionException(string message) : base(message) { }
		protected ExecuteBaseActionException(ITestAction testAction, string targetName) : this(BuildMessage(testAction, targetName)) { }
		protected ExecuteBaseActionException(ITestAction testAction) : this(BuildMessage(testAction)) { }

		static string BuildMessage(ITestAction testAction) {
			return string.Format("{0} \r\n {1}", testAction.FailMessage, testAction.ToString());
		}

		static string BuildMessage(ITestAction testAction, string targetName) {
			return string.Format("{0} \r\n {1} ({2})", testAction.FailMessage, testAction.ToString(), targetName);
		}
	}

	public class MinorException : ExecuteBaseActionException {
		public MinorException(ITestAction testAction, string targetName) : base(testAction, targetName) { }
		public MinorException(ITestAction testAction) : base(testAction) { }
	}

	public class SevereException : ExecuteBaseActionException {
		public SevereException(ITestAction testAction, string targetName) : base(testAction, targetName) { }
		public SevereException(ITestAction testAction) : base(testAction) { }
	}

	public class ScriptComposeException : ExecuteBaseActionException {
		public ScriptComposeException() : this("Script composing") { }
		public ScriptComposeException(string message) : base(message) { }
	}

	public class TargetProgramNotFoundExeption : ExecuteBaseActionException {
		public TargetProgramNotFoundExeption(ElementProgram targetProgram) : base(string.Format("Target program {0} not found", targetProgram.ToString())) { }
	}

	public class TestFailedExeption : ExecuteBaseActionException {
		public TestFailedExeption(ITestAction testAction) : base(testAction) { }
	}
}
