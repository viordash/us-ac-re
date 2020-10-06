using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using NGuard;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Highlighter;

namespace UsAcRe.Services {
	public class TestsLaunchingService : ITestsLaunchingService {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly IWindowsFormsService windowsFormsService;
		CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
		ElementHighlighter elementHighlighter = null;

		public List<ITestAction> executedActions = new List<ITestAction>();
		public ITestAction LastAction {
			get {
				return executedActions
					.Reverse<ITestAction>()
					.Skip(1)
					.FirstOrDefault();
			}
		}

		public TestsLaunchingService(IWindowsFormsService windowsFormsService) {
			Guard.Requires(windowsFormsService, nameof(windowsFormsService));
			this.windowsFormsService = windowsFormsService;
			cancelTokenSource = null;
		}

		public CancellationToken GetCurrentCancellationToken() {
			if(cancelTokenSource == null) {
				return new CancellationToken(false);
			}
			return cancelTokenSource.Token;
		}

		public void Start() {
			if(cancelTokenSource != null) {
				cancelTokenSource.Cancel();
				cancelTokenSource.Dispose();
			}
			cancelTokenSource = new CancellationTokenSource();
		}

		public void Stop() {
			if(cancelTokenSource != null) {
				cancelTokenSource.Cancel();
			}
			CloseHighlighter();
		}

		public void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip) {
			windowsFormsService.GetMainForm().BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
				if(!boundingRectangle.IsEmpty) {
					elementHighlighter = new ElementHighlighter(boundingRectangle, toolTip);
					elementHighlighter.StartHighlighting();
				}
			}));
		}

		public void HighlightElement(System.Windows.Rect boundingRectangle) {
			windowsFormsService.GetMainForm().BeginInvoke((Action)(() => {
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
			windowsFormsService.GetMainForm()?.BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
			}));
		}

		public void Log(ITestAction testAction) {
			logger.Info("\r\n {0}", testAction.ExecuteAsScriptSource());
			executedActions.Add(testAction);
		}
	}
}
