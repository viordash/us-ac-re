using System;
using System.Windows;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UI.Services {
	public class WindowsFormsService : IWindowsFormsService {
		public void BeginInvoke(Delegate method, params object[] args) {
			var mainWindow = GetMainWindow();
			mainWindow?.Dispatcher.BeginInvoke(method, args);
		}

		public Window GetMainWindow() {
			return Application.Current?.MainWindow;
		}
	}
}
