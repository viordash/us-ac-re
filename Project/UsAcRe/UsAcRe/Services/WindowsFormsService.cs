using System.Windows.Forms;

namespace UsAcRe.Services {
	public interface IWindowsFormsService {
		Form GetMainForm();
	}
	public class WindowsFormsService : IWindowsFormsService {
		public Form GetMainForm() {
			return Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
		}
	}
}
