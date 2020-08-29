
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using UsAcRe.Exceptions;
using UsAcRe.Services;

namespace UsAcRe.Actions {

	public abstract class BaseAction {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		protected readonly IAutomationElementService automationElementService;
		protected readonly ITestsLaunchingService testsLaunchingService;
		protected readonly CancellationToken cancellationToken;
		protected readonly BaseAction prevAction;

		public BaseAction(BaseAction prevAction)
			: this(prevAction, ServiceLocator.Current.GetInstance<IAutomationElementService>(), ServiceLocator.Current.GetInstance<ITestsLaunchingService>()) {

		}

		public BaseAction(BaseAction prevAction, IAutomationElementService automationElementService, ITestsLaunchingService testsLaunchingService) {
			this.prevAction = prevAction;
			this.automationElementService = automationElementService;
			this.testsLaunchingService = testsLaunchingService;
			cancellationToken = testsLaunchingService.GetCurrentCancellationToken();
		}

		public abstract string ExecuteAsScriptSource();

		public async Task<BaseAction> ExecuteAsync() {
			await DelayBeforeExecute();
			await ExecuteCoreAsync();
			logger.Info("\r\n {0}", ExecuteAsScriptSource());
			return this;
		}

		protected abstract Task ExecuteCoreAsync();

		protected virtual async Task DelayBeforeExecute() {
			await Task.Delay(10);
		}

		protected async Task SafeActionAsync(Func<ValueTask> action) {
			try {
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

		protected int GetClickPositionToleranceInPercent() {
			return 50;//			Properties.Settings.Default.ClickPositionToleranceInPercent;
		}
	}
}
