using System.Threading;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		void Start();
		void Stop();
		void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip);
		void OpenHighlighter(ElementFromPoint elementFromPoint);
		void CloseHighlighter();
		void Log(BaseAction baseAction);
		BaseAction LastAction { get; }
	}
}
