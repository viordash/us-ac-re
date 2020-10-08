using System.Windows;
using Microsoft.Win32;

namespace UsAcRe.Core.UI.Services {
	public interface IDialogService {
		string OpenFileDialog(string filter);
		string SaveFileDialog(string filter);
		void ShowMessage(string message, MessageBoxImage icon);
		MessageBoxResult Confirmation(string messageBoxText, string caption, MessageBoxButton button);
	}

	public class DialogService : IDialogService {

		public string OpenFileDialog(string filter) {
			var fileDialog = new OpenFileDialog {
				Filter = filter
			};
			if(fileDialog.ShowDialog() == true) {
				return fileDialog.FileName;
			}
			return null;
		}

		public string SaveFileDialog(string filter) {
			var fileDialog = new SaveFileDialog {
				Filter = filter
			};
			if(fileDialog.ShowDialog() == true) {
				return fileDialog.FileName;
			}
			return null;
		}

		public void ShowMessage(string message, MessageBoxImage icon) {
			MessageBox.Show(message);
		}
		public MessageBoxResult Confirmation(string messageBoxText, string caption, MessageBoxButton button) {
			return MessageBox.Show(messageBoxText, caption, button, MessageBoxImage.Question);
		}
	}
}
