using System.Threading;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		void Start(bool isDryRunMode);
		void Stop();
		void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip);
		void HighlightElement(System.Windows.Rect boundingRectangle);
		void CloseHighlighter();
		void Log(BaseAction baseAction);
		BaseAction LastAction { get; }
		bool IsDryRunMode { get; }
	}
}
