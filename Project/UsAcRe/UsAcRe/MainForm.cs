using System;
using System.Threading;
using System.Windows.Forms;
using CommonServiceLocator;
using NLog.Windows.Forms;
using UsAcRe.Highlighter;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		bool stopSearchElement;
		ElementHighlighter elementHighlighter = null;

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
			stopSearchElement = true;
			CloseHighlighter();
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
				var prevPoint = new WinAPI.POINT();
				while(!stopSearchElement) {
					var pt = WinApiService.GetMousePosition();
					if(!pt.WithBoundaries(prevPoint, 10)) {
						lastMouseMoved = DateTime.Now;
						prevPoint = pt;
						moved = true;
						CloseHighlighter();
					} else if(moved && (DateTime.Now - lastMouseMoved).TotalMilliseconds >= 500) {
						try {
							var elementFromPoint = new ElementFromPoint(AutomationElementService, WinApiService, pt);
							BeginInvoke((MethodInvoker)delegate () {
								CloseHighlighter();
								elementHighlighter = BoundingRectangleElementHighLighter.CreateInstance(elementFromPoint);
								elementHighlighter.StartHighlighting();
							});

							logger.Info(elementFromPoint);
						} catch { }
						moved = false;
					}
				}
			}).Start();
		}

		void CloseHighlighter() {
			if(elementHighlighter != null) {
				var highlighter = elementHighlighter;
				BeginInvoke((MethodInvoker)delegate () {
					highlighter.StopHighlighting();
				});
				elementHighlighter = null;
			}
		}
	}
}
