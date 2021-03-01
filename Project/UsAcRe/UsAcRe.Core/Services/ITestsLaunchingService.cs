using System;
using System.Collections.Generic;
using System.Threading;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		ITestLaunchScope Start();
		ITestLaunchScope Examine();
		void Record();
		void Stop();
		void OpenHighlighter(System.Windows.Rect boundingRectangle, string toolTip);
		void HighlightElement(System.Windows.Rect boundingRectangle);
		void CloseHighlighter();
		BaseAction LastAction { get; }
		List<BaseAction> ExecutedActions { get; }
		bool Examination { get; }

		void BeforeExecuteAction(BaseAction testAction);
		void AfterExecuteAction(BaseAction testAction);

		event ExecuteActionEventHandler OnBeforeExecuteAction;
		event ExecuteActionEventHandler OnAfterExecuteAction;
	}
}
