using System;

namespace UsAcRe.Actions {
	public class ExecuteBaseActionException : Exception {
		public ExecuteBaseActionException(string message) : base(message) {

		}
	}
	public class ExecuteMouseActionException : ExecuteBaseActionException {
		public ExecuteMouseActionException(MouseAction mouseAction, string targetName) : base(BuildMessage(mouseAction, targetName)) {

		}

		static string BuildMessage(MouseAction mouseAction, string targetName) {
			return string.Format("{0} ActionType:{1}, DownClickedPoint:{2}", targetName, mouseAction.ActionType, mouseAction.DownClickedPoint);
		}
	}
}
