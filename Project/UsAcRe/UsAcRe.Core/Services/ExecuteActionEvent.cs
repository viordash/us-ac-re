using System;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Services {
	public class ExecuteActionEventArgs : EventArgs {
		public BaseAction Action { get; private set; }
		public ExecuteActionEventArgs(BaseAction action) {
			Action = action;
		}
	}
	public delegate void ExecuteActionEventHandler(object sender, ExecuteActionEventArgs e);
}
