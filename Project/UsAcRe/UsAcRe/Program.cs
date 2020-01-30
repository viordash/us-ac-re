using System;
using System.Windows.Forms;
using NLog;

namespace UsAcRe {
	static class Program {
		static readonly Logger logger = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 
		[STAThread]
		static void Main() {
			AppDomain.CurrentDomain.UnhandledException +=
				(sender, args) => HandleUnhandledException(args.ExceptionObject as Exception);
			Application.ThreadException +=
				(sender, args) => HandleUnhandledException(args.Exception);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void HandleUnhandledException(Exception e) {
			var mainForm = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
			var message = e.GetBaseException().Message;
			MessageBox.Show(message, mainForm != null ? mainForm.Text : "??", MessageBoxButtons.OK, MessageBoxIcon.Error);
			if(mainForm != null) {
				logger.Error(message);
			}
		}
	}
}
