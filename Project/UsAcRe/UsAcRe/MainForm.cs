using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using NLog.Windows.Forms;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		bool stopSearchElement;
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
			stopSearchElement = true;
		}

		private void btnStart_Click(object sender, EventArgs e) {
			if(btnStart.Checked) {
				logger.Warn("Start");
				stopSearchElement = false;
				SearchElement();
			} else {
				stopSearchElement = true;
				logger.Warn("Stop");
			}
		}

		void SearchElement() {
			new Thread(delegate () {
				var lastMouseMoved = DateTime.Now;
				bool moved = false;
				WinAPI.POINT pt;
				WinAPI.POINT prevPoint = new WinAPI.POINT();
				while(WinAPI.GetCursorPos(out pt) && !stopSearchElement) {
					if(!pt.WithBoundaries(prevPoint, 20)) {
						lastMouseMoved = DateTime.Now;
						prevPoint = pt;
						moved = true;
					} else if(moved && (DateTime.Now - lastMouseMoved).TotalMilliseconds >= 500) {
						var elementFromPoint = new ElementFromPoint(new Point(pt.x, pt.y));
						logger.Info(elementFromPoint);
						moved = false;
					}
				}

			}).Start();
		}
	}
}
