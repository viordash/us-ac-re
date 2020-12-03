using System;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;

namespace UsAcRe.Player.Services {
	public class ExecuteActionEventArgs : EventArgs {
		public BaseAction Action { get; private set; }
		public ExecuteActionEventArgs(BaseAction action) {
			Action = action;
		}
	}

	public class PlayerLaunchingService : TestsLaunchingService {

		public delegate void BeforeExecuteActionEventHandler(object sender, ExecuteActionEventArgs e);
		public event BeforeExecuteActionEventHandler OnBeforeExecuteAction;

		public PlayerLaunchingService(IWindowsFormsService windowsFormsService) : base(windowsFormsService) {
		}

		public override void Log(BaseAction testAction) {
			executedActions.Add(testAction);
			OnBeforeExecuteAction?.Invoke(this, new ExecuteActionEventArgs(testAction));
		}
	}
}
