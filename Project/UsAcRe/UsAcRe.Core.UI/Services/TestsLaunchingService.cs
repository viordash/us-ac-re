using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using NGuard;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Highlighter;

namespace UsAcRe.Core.UI.Services {
	public class TestsLaunchingService : ITestsLaunchingService {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly IWindowsFormsService windowsFormsService;
		CancellationTokenSource cancelTokenSource = null;
		ElementHighlighter elementHighlighter = null;

		protected readonly List<BaseAction> executedActions;
		public IEnumerable<BaseAction> ExecutedActions { get { return executedActions; } }
		public BaseAction LastAction {
			get {
				return executedActions
					.Reverse<BaseAction>()
					.Skip(1)
					.FirstOrDefault();
			}
		}

		public bool IsDryRunMode { get; private set; } = false;

		public TestsLaunchingService(IWindowsFormsService windowsFormsService) {
			Guard.Requires(windowsFormsService, nameof(windowsFormsService));
			this.windowsFormsService = windowsFormsService;
			executedActions = new List<BaseAction>();
		}

		public CancellationToken GetCurrentCancellationToken() {
			if(cancelTokenSource == null) {
				throw new InvalidOperationException("CancellationToken not ready");
			}
			return cancelTokenSource.Token;
		}

		#region inner classes
		class TestsRunningScope : IDisposable {
			readonly Action closeAction;
			public TestsRunningScope(Action closeAction) {
				this.closeAction = closeAction;
			}
			public void Dispose() {
				closeAction();
			}
		}
		#endregion

		void CreatCancelTokenSource() {
			if(cancelTokenSource != null) {
				if(!cancelTokenSource.IsCancellationRequested) {
					throw new InvalidOperationException("Testing already runned");
				}
				cancelTokenSource.Cancel();
				cancelTokenSource.Dispose();
			}
			cancelTokenSource = new CancellationTokenSource();
		}

		public IDisposable Start(bool isDryRunMode) {
			IsDryRunMode = isDryRunMode;
			executedActions.Clear();
			CreatCancelTokenSource();
			return new TestsRunningScope(Stop);
		}

		public void Record() {
			CreatCancelTokenSource();
		}

		public void Stop() {
			if(cancelTokenSource != null) {
				cancelTokenSource.Cancel();
			}
			CloseHighlighter();
		}

		public void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip) {
			windowsFormsService.BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
				if(!boundingRectangle.IsEmpty) {
					elementHighlighter = new ElementHighlighter(boundingRectangle, toolTip);
					elementHighlighter.StartHighlighting();
				}
			}));
		}

		public void HighlightElement(System.Windows.Rect boundingRectangle) {
			windowsFormsService.BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
				if(!boundingRectangle.IsEmpty) {
					elementHighlighter = new ElementHighlighter(boundingRectangle, null, 1, 0.6, Colors.Yellow, Colors.Red);
					elementHighlighter.StartHighlighting();
				}
			}));
		}

		void CloseHighlighterInternal() {
			if(elementHighlighter != null) {
				var highlighter = elementHighlighter;
				highlighter.StopHighlighting();
				elementHighlighter = null;
			}
		}

		public void CloseHighlighter() {
			windowsFormsService.BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
			}));
		}

		public virtual void Log(BaseAction testAction) {
			logger.Info("\r\n {0}", testAction.ExecuteAsScriptSource());
			executedActions.Add(testAction);
		}
	}
}
