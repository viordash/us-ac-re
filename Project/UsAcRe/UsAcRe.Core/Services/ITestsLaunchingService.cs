using System;
using System.Collections.Generic;
using System.Threading;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		IDisposable Start(bool isDryRunMode);
		void Record();
		void Stop();
		void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip);
		void HighlightElement(System.Windows.Rect boundingRectangle);
		void CloseHighlighter();
		void Log(BaseAction baseAction);
		BaseAction LastAction { get; }
		IEnumerable<BaseAction> ExecutedActions { get; }
		bool IsDryRunMode { get; }
	}
}
