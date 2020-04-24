using System;
using System.Windows.Forms;
using CommonServiceLocator;
using NLog.Windows.Forms;
using UsAcRe.Highlighter;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		ElementHighlighter elementHighlighter = null;
		ElementFromPoint elementFromPoint = null;
		ElementHighlighter mouseClickBlocker = null;

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }

		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if(logger == null) {
				logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			CloseMouseClickBlocker();
			CloseHighlighter();
		}

		private void btnStart_Click(object sender, EventArgs e) {
			if(btnStart.Checked) {
				StartHooks();
			} else {
				StopHooks();
			}
		}

		private void miClearLog_Click(object sender, EventArgs e) {
			txtLog.Clear();
		}

	}
}
