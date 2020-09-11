
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using NGuard;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {

	public abstract class BaseAction {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		protected readonly IAutomationElementService automationElementService;
		protected readonly ITestsLaunchingService testsLaunchingService;
		protected readonly IWinApiService winApiService;
		protected readonly ISettingsService settingsService;
		protected readonly CancellationToken cancellationToken;
		protected readonly BaseAction prevAction;
		protected IServiceProvider ServiceProvider { get; set; }

		public BaseAction(IServiceProvider serviceProvider) {
			ServiceProvider = serviceProvider;
		}

		public BaseAction(BaseAction prevAction)
			: this(prevAction, prevAction.ServiceProvider.GetInstance<IAutomationElementService>(),
				  prevAction.ServiceProvider.GetInstance<ITestsLaunchingService>(),
				  prevAction.ServiceProvider.GetInstance<IWinApiService>(),
				  prevAction.ServiceProvider.GetInstance<ISettingsService>()) { }

		public BaseAction(
			BaseAction prevAction,
			IAutomationElementService automationElementService,
			ITestsLaunchingService testsLaunchingService,
			IWinApiService winApiService,
			ISettingsService settingsService) {
			Guard.Requires(automationElementService, nameof(automationElementService));
			Guard.Requires(testsLaunchingService, nameof(testsLaunchingService));
			Guard.Requires(winApiService, nameof(winApiService));
			Guard.Requires(settingsService, nameof(settingsService));
			this.prevAction = prevAction;
			this.automationElementService = automationElementService;
			this.testsLaunchingService = testsLaunchingService;
			this.winApiService = winApiService;
			this.settingsService = settingsService;
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
