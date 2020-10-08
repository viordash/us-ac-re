﻿
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
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		protected static T CreateInstance<T>() where T : BaseAction {
			return ServiceLocator.Current.GetInstance<T>();
		}


		public string FailMessage { get; set; } = null;

		protected readonly ISettingsService settingsService;
		protected readonly ITestsLaunchingService testsLaunchingService;
		protected readonly CancellationToken cancellationToken;

		protected BaseAction(
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService) {
			Guard.Requires(settingsService, nameof(settingsService));
			Guard.Requires(testsLaunchingService, nameof(testsLaunchingService));
			this.settingsService = settingsService;
			this.testsLaunchingService = testsLaunchingService;
			cancellationToken = testsLaunchingService.GetCurrentCancellationToken();
		}

		public abstract string ExecuteAsScriptSource();

		public async Task ExecuteAsync() {
			await SafeActionAsync(ExecuteCoreAsync);
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
				if(ex is Win32Exception exception && (uint)exception.ErrorCode == 0x80004005) {
					throw new MinorException(this);
				} else {
					throw new SevereException(this);
				}
			}

		}
	}
}