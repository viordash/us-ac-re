
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using NGuard;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public abstract class BaseAction {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		protected static T CreateInstance<T>() where T : BaseAction {
			return ServiceLocator.Current.GetInstance<T>();
		}

		protected readonly ITestsLaunchingService testsLaunchingService;
		protected readonly CancellationToken cancellationToken;

		protected BaseAction(
			ITestsLaunchingService testsLaunchingService) {
			Guard.Requires(testsLaunchingService, nameof(testsLaunchingService));
			this.testsLaunchingService = testsLaunchingService;
			cancellationToken = testsLaunchingService.GetCurrentCancellationToken();
		}

		public abstract string ExecuteAsScriptSource();

		public async Task<BaseAction> ExecuteAsync() {
			await SafeActionAsync(ExecuteCoreAsync);
			return this;
		}

		protected abstract ValueTask ExecuteCoreAsync();

		protected async Task SafeActionAsync(Func<ValueTask> action) {
			try {
				testsLaunchingService.Log(this);
				if(cancellationToken.IsCancellationRequested) {
					return;
				}
				await action();
			} catch(TestFailedExeption) {
				throw;
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
