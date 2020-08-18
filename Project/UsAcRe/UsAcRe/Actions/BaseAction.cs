
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UsAcRe.Exceptions;

namespace UsAcRe.Actions {

	public abstract class BaseAction {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		public abstract string ExecuteAsScriptSource();

		public virtual async Task ExecuteAsync() {
			await ExecuteCoreAsync();
			logger.Info("\r\n {0}", ExecuteAsScriptSource());
			Thread.Sleep(200);
		}

		protected abstract Task ExecuteCoreAsync();

		protected async Task SafeActionAsync(Func<ValueTask> action) {
			try {
				await action();
			} catch(Exception ex) {
				if(ex is Win32Exception && (uint)((Win32Exception)ex).ErrorCode == 0x80004005) {
					throw new MinorException(this);
				} else {
					throw new SevereException(this);
				}
			}

		}
	}
}
