using System;
using System.Windows.Forms;
using NLog;
using NLog.Windows.Forms;

namespace UsAcRe {
	public partial class MainForm : Form {
		static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);
		}

		private void btnStart_Click(object sender, EventArgs e) {
			logger.Info("Start");
			logger.Trace("Start trace");
		}
	}
}
