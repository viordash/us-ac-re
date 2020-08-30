﻿
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
		protected readonly IWinApiService winApiService;
		protected readonly ISettingsService settingsService;
		protected readonly CancellationToken cancellationToken;
		protected readonly BaseAction prevAction;

		public BaseAction(BaseAction prevAction)
			: this(prevAction, ServiceLocator.Current.GetInstance<IAutomationElementService>(),
				  ServiceLocator.Current.GetInstance<ITestsLaunchingService>(),
				  ServiceLocator.Current.GetInstance<IWinApiService>(),
				  ServiceLocator.Current.GetInstance<ISettingsService>()) { }

		public BaseAction(
			BaseAction prevAction,
			IAutomationElementService automationElementService,
			ITestsLaunchingService testsLaunchingService,
			IWinApiService winApiService,
			ISettingsService settingsService) {
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
