using System;
using System.Drawing;
using System.Windows.Forms;
using NLog.Windows.Forms;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		WinAPI.POINT prevMousePoint;
		DateTime lastMouseMovedTimestamp;

		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);
			WinAPI.GetCursorPos(out prevMousePoint);
		}

		private void btnStart_Click(object sender, EventArgs e) {
			logger.Info("Start");
			timer1.Enabled = true;
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if(logger == null) {
				logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
			}
		}

		private void timer1_Tick(object sender, EventArgs e) {
			if(!WinAPI.GetCursorPos(out WinAPI.POINT pt)) {
				return;
			}
			bool mouseInMoving = Math.Abs(pt.x - prevMousePoint.x) > 20 || Math.Abs(pt.y - prevMousePoint.y) > 20;
			prevMousePoint = pt;
			if(mouseInMoving) {
				lastMouseMovedTimestamp = DateTime.Now;
				return;
			}
			if(lastMouseMovedTimestamp != DateTime.MinValue && (DateTime.Now - lastMouseMovedTimestamp).TotalMilliseconds > 1000) {
				var elementFromPoint = new ElementFromPoint(new Point(pt.x, pt.y));
				logger.Info(elementFromPoint);
				lastMouseMovedTimestamp = DateTime.MinValue;
			}
		}
	}
}
