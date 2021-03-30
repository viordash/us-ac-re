using System;
using System.Windows;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UI.Services {
	public class WindowsFormsService : IWindowsFormsService {
		public void BeginInvoke(Delegate method, params object[] args) {
			Application.Current?.Dispatcher.BeginInvoke(method, args);
		}
	}
}
