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

		public event ExecuteActionEventHandler OnBeforeExecuteAction;
		public event ExecuteActionEventHandler OnAfterExecuteAction;

		public List<BaseAction> ExecutedActions { get; private set; } = null;
		public BaseAction LastAction {
			get {
				return ExecutedActions?
					.Reverse<BaseAction>()
					.Skip(1)
					.FirstOrDefault();
			}
		}

		public bool Examination { get; private set; } = false;

		public TestsLaunchingService(IWindowsFormsService windowsFormsService) {
			Guard.Requires(windowsFormsService, nameof(windowsFormsService));
			this.windowsFormsService = windowsFormsService;
		}

		CancellationTokenSource CurrentCancellation {
			get {
				if(cancelTokenSource == null) {
					throw new InvalidOperationException("CancellationToken not ready");
				}
				return cancelTokenSource;
			}
		}

		public CancellationToken CurrentCancellationToken {
			get { return CurrentCancellation.Token; }
		}

		#region inner classes
		class TestLaunchScope : ITestLaunchScope {
			readonly Action closeAction;
			readonly TestsLaunchingService service;
			readonly bool oldExamination;
			readonly List<BaseAction> oldExecutedActions;
			readonly CancellationTokenSource oldCancelTokenSource;
			readonly bool nestedScope;

			public TestLaunchScope(TestsLaunchingService service, Action closeAction) {
				this.service = service;
				this.closeAction = closeAction;
				oldExamination = service.Examination;
				oldExecutedActions = service.ExecutedActions;
				service.ExecutedActions = new List<BaseAction>();
				oldCancelTokenSource = service.cancelTokenSource;
				nestedScope = oldCancelTokenSource != null;
				if(!nestedScope) {
					service.cancelTokenSource = new CancellationTokenSource();
				}
			}

			public void Dispose() {
				if(!nestedScope) {
					closeAction();
				}
				service.Examination = oldExamination;
				service.ExecutedActions = oldExecutedActions;
				service.cancelTokenSource = oldCancelTokenSource;
			}
		}
		class TestsRunningScope : TestLaunchScope {
			public TestsRunningScope(TestsLaunchingService service, Action closeAction) : base(service, closeAction) {
				service.Examination = false;
			}
		}
		class TestsExamineScope : TestLaunchScope {
			public TestsExamineScope(TestsLaunchingService service, Action closeAction) : base(service, closeAction) {
				service.Examination = true;
			}
		}
		#endregion

		public ITestLaunchScope Start() {
			return new TestsRunningScope(this, Stop);
		}

		public ITestLaunchScope Examine() {
			return new TestsExamineScope(this, Stop);
		}

		public void Record() {
			//CreatCancelTokenSource();
		}

		public void Stop() {
			CurrentCancellation.Cancel();
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

		public void BeforeExecuteAction(BaseAction testAction) {
			logger.Info("\r\n {0}", testAction.ExecuteAsScriptSource());
			ExecutedActions.Add(testAction);
			OnBeforeExecuteAction?.Invoke(this, new ExecuteActionEventArgs(testAction));
		}

		public void AfterExecuteAction(BaseAction testAction) {
			OnAfterExecuteAction?.Invoke(this, new ExecuteActionEventArgs(testAction));
		}
	}
}
