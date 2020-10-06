using System;
using System.Windows.Forms;
using UsAcRe.Core.Services;

namespace UsAcRe.Services {
	public class WindowsFormsService : IWindowsFormsService {
		public void BeginInvoke(Delegate method, params object[] args) {
			var mainForm = GetMainForm();
			mainForm?.BeginInvoke(method, args);
		}

		public Form GetMainForm() {
			return Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
		}
	}
}
