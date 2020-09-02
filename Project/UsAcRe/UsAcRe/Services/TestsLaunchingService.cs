using System;
using System.Threading;
using NGuard;
using UsAcRe.Actions;
using UsAcRe.Highlighter;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		void Start();
		void Stop();
		void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip);
		void OpenHighlighter(ElementFromPoint elementFromPoint);
		void CloseHighlighter();
		void Log(BaseAction baseAction);
	}

	public class TestsLaunchingService : ITestsLaunchingService {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly IWindowsFormsService windowsFormsService;
		CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
		ElementHighlighter elementHighlighter = null;

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

		public void OpenHighlighter(ElementFromPoint elementFromPoint) {
			windowsFormsService.GetMainForm().BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
				if(!elementFromPoint.TreeOfSpecificUiElement.BoundingRectangle.IsEmpty) {
					elementHighlighter = new ElementHighlighter(elementFromPoint);
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

		public void Log(BaseAction baseAction) {
			logger.Info("\r\n {0}", baseAction.ExecuteAsScriptSource());
		}
	}
}
