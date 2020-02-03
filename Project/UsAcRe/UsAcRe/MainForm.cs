using System;
using System.Windows.Forms;
using NLog.Windows.Forms;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;

		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);
		}

		private void btnStart_Click(object sender, EventArgs e) {
			logger.Info("Start");
			logger.Trace("Start trace");
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if(logger == null) {
				logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
			}
		}
	}
}
