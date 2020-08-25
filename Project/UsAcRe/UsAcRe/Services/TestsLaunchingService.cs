using System;
using System.Threading;
using NGuard;
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
	}

	public class TestsLaunchingService : ITestsLaunchingService {
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
				cancelTokenSource = null;
			}
			CloseHighlighter();
		}

		public void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip) {
			windowsFormsService.GetMainForm().BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
				elementHighlighter = new ElementHighlighter(boundingRectangle, toolTip);
				elementHighlighter.StartHighlighting();
			}));
		}

		public void OpenHighlighter(ElementFromPoint elementFromPoint) {
			windowsFormsService.GetMainForm().BeginInvoke((Action)(() => {
				CloseHighlighterInternal();
				elementHighlighter = new ElementHighlighter(elementFromPoint);
				elementHighlighter.StartHighlighting();
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
	}
}
