

using System;
using System.Windows;
using System.Windows.Threading;
using UsAcRe.Core.Services;

namespace UsAcRe.Services {
	public class WindowsFormsService : IWindowsFormsService {
		public void BeginInvoke(Delegate method, params object[] args) {
			var mainWindow = GetMainWindow();
			mainWindow?.Dispatcher.BeginInvoke(method, args);
		}

		public Window GetMainWindow() {
			return Application.Current.MainWindow;
		}

	}
}
