
using System;

namespace UsAcRe.Actions {

	public abstract class BaseAction {
		public NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		public event EventHandler OnModify;

		public abstract int ExecuteTimeoutMs { get; }
		public virtual int DelayMsBeforeRun {
			get { return 100; }
		}

		public virtual void Modified() {
			if(OnModify != null) {
				OnModify(this, EventArgs.Empty);
			}
		}

		public abstract void Execute();
	}
}
