using System.Threading;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		void Start();
		void Stop();
		void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip);
		void HighlightElement(System.Windows.Rect boundingRectangle);
		void CloseHighlighter();
		void Log(ITestAction baseAction);
		ITestAction LastAction { get; }
	}
}
